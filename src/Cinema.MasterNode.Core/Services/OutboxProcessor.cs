using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Cinema.MasterNode.Core.Configuration;
using Cinema.MasterNode.Domain.Events;
using Cinema.MasterNode.Domain.Models;
using Cinema.MasterNode.Infrastructure.Data;
using Confluent.Kafka;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cinema.MasterNode.Core.Services;

public interface IOutboxProcessor
{
    Task StartProcessingAsync(CancellationToken cancellationToken);
}

public class OutboxProcessor : IOutboxProcessor
{
    private readonly IConfiguration _config;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IProducer<string, string> _kafkaProducer;
    private readonly Channel<OutboxBatch> _channel;
    private readonly string _processorId;
    
    public OutboxProcessor(
        IConfiguration config,
        ILogger<OutboxProcessor> logger,
        IServiceScopeFactory scopeFactory,
        IProducer<string, string> kafkaProducer)
    {
        _config = config;
        _logger = logger;
        _scopeFactory = scopeFactory;
        _kafkaProducer = kafkaProducer;
        _processorId = $"MasterNode-{Environment.MachineName}-{Guid.NewGuid():N}";
        
        var options = config.GetSection("MasterNode").Get<MasterNodeOptions>() ?? new MasterNodeOptions();
        _channel = Channel.CreateBounded<OutboxBatch>(new BoundedChannelOptions(options.MaxConcurrentBatches * 2)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
    }
    
    public async Task StartProcessingAsync(CancellationToken cancellationToken)
    {
        var options = _config.GetSection("MasterNode").Get<MasterNodeOptions>() ?? new MasterNodeOptions();
        
        // Start multiple polling workers (one per Cinema API database)
        // Assuming we have connection strings named "CinemaApi1" and "CinemaApi2"
        // If they are not present, we can fallback or just log warning.
        var pollers = new List<Task>();
        
        if (!string.IsNullOrEmpty(_config.GetConnectionString("CinemaApi1")))
            pollers.Add(PollOutboxAsync("CinemaApi1", cancellationToken));
            
        if (!string.IsNullOrEmpty(_config.GetConnectionString("CinemaApi2")))
            pollers.Add(PollOutboxAsync("CinemaApi2", cancellationToken));

        if (pollers.Count == 0)
        {
            _logger.LogWarning("No CinemaApi connection strings found. Polling disabled.");
        }
        
        // Start multiple batch processors
        var processors = Enumerable.Range(0, options.ProcessorThreads)
            .Select(i => ProcessBatchesAsync(i, cancellationToken))
            .ToArray();
        
        await Task.WhenAll(pollers.Concat(processors));
    }
    
    private async Task PollOutboxAsync(string connectionName, CancellationToken cancellationToken)
    {
        var options = _config.GetSection("MasterNode").Get<MasterNodeOptions>() ?? new MasterNodeOptions();
        var connectionString = _config.GetConnectionString(connectionName);
        
        _logger.LogInformation("Started polling outbox from {ConnectionName}", connectionName);
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);
                
                // Claim a batch of unprocessed messages with SELECT FOR UPDATE pattern
                var batch = await ClaimOutboxBatchAsync(connection, options.BatchSize, cancellationToken);
                
                if (batch.Messages.Any())
                {
                    await _channel.Writer.WriteAsync(batch, cancellationToken);
                    _logger.LogDebug("Claimed {Count} messages from {Source}", 
                        batch.Messages.Count, connectionName);
                }
                else
                {
                    // No messages, wait before polling again
                    await Task.Delay(options.PollingIntervalMs, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error polling outbox from {ConnectionName}", connectionName);
                await Task.Delay(5000, cancellationToken);
            }
        }
    }
    
    private async Task<OutboxBatch> ClaimOutboxBatchAsync(
        IDbConnection connection, 
        int batchSize, 
        CancellationToken cancellationToken)
    {
        var options = _config.GetSection("MasterNode").Get<MasterNodeOptions>() ?? new MasterNodeOptions();
        
        // SQL Server-specific locking query with READPAST hint
        // Adapted to the existing Cinema.Infrastructure OutboxMessage schema
        var sql = @"
            WITH TargetMessages AS (
                SELECT TOP (@BatchSize) 
                    Id, Type, Content, OccurredOnUtc, ProcessedOnUtc, Error
                FROM OutboxMessages WITH (READPAST, UPDLOCK, ROWLOCK)
                WHERE ProcessedOnUtc IS NULL 
                ORDER BY OccurredOnUtc
            )
            UPDATE TargetMessages
            SET ProcessedOnUtc = @Now
            OUTPUT 
                inserted.Id, inserted.Type, inserted.Content, inserted.OccurredOnUtc, inserted.ProcessedOnUtc, inserted.Error
        ";
        
        var messages = (await connection.QueryAsync<OutboxMessage>(sql, new
        {
            BatchSize = batchSize,
            Now = DateTime.UtcNow
        })).ToList();
        
        return new OutboxBatch
        {
            Messages = messages,
            ClaimedAt = DateTime.UtcNow,
            ConnectionString = connection.ConnectionString
        };
    }
    
    private async Task ProcessBatchesAsync(int workerId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Batch processor {WorkerId} started", workerId);
        
        await foreach (var batch in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                await ProcessBatchWithTPLAsync(batch, workerId, cancellationToken);
                
                sw.Stop();
                _logger.LogInformation(
                    "Worker {WorkerId} processed {Count} messages in {ElapsedMs}ms",
                    workerId, batch.Messages.Count, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in batch processor {WorkerId}", workerId);
                await HandleFailedBatchAsync(batch, ex);
            }
        }
    }
    
    private async Task ProcessBatchWithTPLAsync(
        OutboxBatch batch, 
        int workerId, 
        CancellationToken cancellationToken)
    {
        // Group messages by event type for optimized processing
        var grouped = batch.Messages
            .GroupBy(m => m.Type)
            .ToList();
        
        // Process each group in parallel
        await Parallel.ForEachAsync(
            grouped,
            new ParallelOptions 
            { 
                MaxDegreeOfParallelism = 4,
                CancellationToken = cancellationToken 
            },
            async (group, ct) =>
            {
                await ProcessEventGroupAsync(group.ToList(), batch.ConnectionString, ct);
            });
        
        // Mark all messages as processed
        await MarkMessagesAsProcessedAsync(batch, cancellationToken);
    }
    
    private async Task ProcessEventGroupAsync(
        List<OutboxMessage> messages,
        string sourceConnectionString,
        CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        
        foreach (var message in messages)
        {
            // 1. Write to Master SQL Server database
            tasks.Add(WriteToDatabaseAsync(message, cancellationToken));
            
            // 2. Publish to Kafka
            tasks.Add(PublishToKafkaAsync(message, cancellationToken));
        }
        
        await Task.WhenAll(tasks);
    }
    
    private async Task WriteToDatabaseAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
        
        var eventType = message.Type;
        
        try
        {
            // Handle different event types
            // Note: The payload is JSON. We need to deserialize it.
            // The user provided example classes like ReservationCreatedEvent.
            // We need to make sure we have these classes or use JObject.
            
            if (eventType.Contains("ReservationCreated"))
            {
                var reservationEvent = JsonSerializer.Deserialize<ReservationCreatedIntegrationEvent>(message.Content);
                if (reservationEvent != null)
                    await CreateReservationAsync(context, reservationEvent, cancellationToken);
            }
            else if (eventType.Contains("ReservationConfirmed"))
            {
                var confirmedEvent = JsonSerializer.Deserialize<ReservationConfirmedIntegrationEvent>(message.Content);
                if (confirmedEvent != null)
                    await ConfirmReservationAsync(context, confirmedEvent, cancellationToken);
            }
            else if (eventType.Contains("ShowtimeCreated"))
            {
                var showtimeEvent = JsonSerializer.Deserialize<ShowtimeCreatedIntegrationEvent>(message.Content);
                if (showtimeEvent != null)
                    await CreateShowtimeAsync(context, showtimeEvent, cancellationToken);
            }
            
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing message {MessageId} to database", message.Id);
            throw;
        }
    }
    
    private async Task CreateReservationAsync(
        MasterDbContext context, 
        ReservationCreatedIntegrationEvent evt, 
        CancellationToken cancellationToken)
    {
        // Check for idempotency
        var exists = await context.Reservations
            .AnyAsync(r => r.Id == evt.ReservationId, cancellationToken);
        
        if (exists) return; // Already processed
        
        var reservation = new Reservation
        {
            Id = evt.ReservationId,
            ShowtimeId = evt.ShowtimeId,
            CustomerId = evt.CustomerId,
            CreatedAt = evt.OccurredOnUtc,
            ExpiresAt = evt.OccurredOnUtc.AddMinutes(10),
            Status = "Pending",
            TotalPrice = evt.TotalPrice
        };
        
        context.Reservations.Add(reservation);
        
        // Add seats
        if (evt.Seats != null)
        {
            var seats = evt.Seats.Select(s => new ReservationSeat
            {
                Id = Guid.NewGuid(),
                ReservationId = evt.ReservationId,
                SeatRow = s.Row,
                SeatNumber = s.Number,
                CreatedAt = evt.OccurredOnUtc
            });
            
            context.ReservationSeats.AddRange(seats);
        }
    }
    
    private async Task ConfirmReservationAsync(
        MasterDbContext context,
        ReservationConfirmedIntegrationEvent evt,
        CancellationToken cancellationToken)
    {
        await context.Reservations
            .Where(r => r.Id == evt.ReservationId && r.Status == "Pending")
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(r => r.Status, "Confirmed")
                    .SetProperty(r => r.ConfirmedAt, DateTime.UtcNow), // Or evt.ConfirmedAt if available
                cancellationToken);
    }
    
    private async Task CreateShowtimeAsync(
        MasterDbContext context,
        ShowtimeCreatedIntegrationEvent evt,
        CancellationToken cancellationToken)
    {
        var exists = await context.Showtimes
            .AnyAsync(s => s.Id == evt.ShowtimeId, cancellationToken);
        
        if (exists) return;
        
        var showtime = new Showtime
        {
            Id = evt.ShowtimeId,
            MovieImdbId = evt.MovieImdbId,
            ScreeningTime = evt.ScreeningTime,
            AuditoriumId = evt.AuditoriumId,
            CreatedAt = evt.OccurredOnUtc
        };
        
        context.Showtimes.Add(showtime);
    }
    
    private async Task PublishToKafkaAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var topic = GetKafkaTopicFromEventType(message.Type);
            var aggregateId = GetAggregateId(message);
            var aggregateType = GetAggregateType(message);
            
            var kafkaMessage = new Message<string, string>
            {
                Key = aggregateId,
                Value = message.Content,
                Headers = new Headers
                {
                    { "event-id", Encoding.UTF8.GetBytes(message.Id.ToString()) },
                    { "event-type", Encoding.UTF8.GetBytes(message.Type) },
                    { "aggregate-type", Encoding.UTF8.GetBytes(aggregateType) },
                    { "aggregate-id", Encoding.UTF8.GetBytes(aggregateId) },
                    { "created-at", Encoding.UTF8.GetBytes(message.OccurredOnUtc.ToString("O")) }
                }
            };
            
            var result = await _kafkaProducer.ProduceAsync(topic, kafkaMessage, cancellationToken);
            
            _logger.LogDebug("Published message {MessageId} to topic {Topic}, partition {Partition}, offset {Offset}",
                message.Id, topic, result.Partition.Value, result.Offset.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message {MessageId} to Kafka", message.Id);
            throw;
        }
    }
    
    private string GetAggregateId(OutboxMessage message)
    {
        try 
        {
            using var doc = JsonDocument.Parse(message.Content);
            if (doc.RootElement.TryGetProperty("ReservationId", out var resId)) return resId.GetString() ?? Guid.NewGuid().ToString();
            if (doc.RootElement.TryGetProperty("ShowtimeId", out var showId)) return showId.GetString() ?? Guid.NewGuid().ToString();
            if (doc.RootElement.TryGetProperty("Id", out var id)) return id.GetString() ?? Guid.NewGuid().ToString();
            return Guid.NewGuid().ToString();
        }
        catch
        {
            return Guid.NewGuid().ToString();
        }
    }

    private string GetAggregateType(OutboxMessage message)
    {
        if (message.Type.Contains("Reservation")) return "Reservation";
        if (message.Type.Contains("Showtime")) return "Showtime";
        return "Unknown";
    }
    
    private string GetKafkaTopicFromEventType(string eventType)
    {
        return eventType switch
        {
            var t when t.Contains("Reservation") => "cinema.reservations",
            var t when t.Contains("Showtime") => "cinema.showtimes",
            _ => "cinema.domain.events"
        };
    }
    
    private async Task MarkMessagesAsProcessedAsync(OutboxBatch batch, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(batch.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        
        var messageIds = batch.Messages.Select(m => m.Id).ToList();
        
        var sql = @"
            DELETE FROM OutboxMessages
            WHERE Id IN @MessageIds
        ";
        
        await connection.ExecuteAsync(sql, new { MessageIds = messageIds });
    }
    
    private async Task HandleFailedBatchAsync(OutboxBatch batch, Exception ex)
    {
        using var connection = new SqlConnection(batch.ConnectionString);
        await connection.OpenAsync();
        
        var messageIds = batch.Messages.Select(m => m.Id).ToList();
        
        var sql = @"
            UPDATE OutboxMessages
            SET ProcessedOnUtc = NULL,
                Error = @ErrorMessage
            WHERE Id IN @MessageIds
        ";
        
        await connection.ExecuteAsync(sql, new 
        { 
            MessageIds = messageIds,
            ErrorMessage = ex.Message.Substring(0, Math.Min(4000, ex.Message.Length))
        });
        
        _logger.LogWarning("Released {Count} messages for retry", batch.Messages.Count);
    }
}
