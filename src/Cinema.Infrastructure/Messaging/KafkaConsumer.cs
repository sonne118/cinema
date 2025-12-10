using System.Reflection;
using Cinema.Application.Common.Interfaces.Persistence;
using Cinema.Application.Common.Interfaces.Queries;
using Cinema.Application.Common.Models;
using Cinema.Domain.Common.Models;
using Cinema.Domain.ReservationAggregate.Events;
using Cinema.Domain.ShowtimeAggregate.Events;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cinema.Infrastructure.Messaging;





public class KafkaConsumer : BackgroundService
{
    private readonly KafkaSettings _settings;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<KafkaConsumer> _logger;
    private IConsumer<string, string> _consumer;

    public KafkaConsumer(
        IOptions<KafkaSettings> settings,
        IServiceScopeFactory scopeFactory,
        ILogger<KafkaConsumer> logger)
    {
        _settings = settings.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.ConsumerGroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer starting...");
        _logger.LogInformation("Kafka Bootstrap Servers: {BootstrapServers}", _settings.BootstrapServers);
        _logger.LogInformation("Kafka Group ID: {GroupId}", _settings.ConsumerGroupId);
        
        try
        {
            _consumer.Subscribe(_settings.DomainEventsTopic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);

                    _logger.LogInformation("Received message key: {Key}", consumeResult.Message.Key);

                    await HandleMessageAsync(consumeResult.Message.Value, stoppingToken);

                    _consumer.Commit(consumeResult);
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, "Error consuming Kafka message");
                    
                    await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in Kafka consumer");
                }
            }
        }
        finally
        {
            _consumer.Close();
            _consumer.Dispose();
        }
    }

    private async Task HandleMessageAsync(string messageValue, CancellationToken cancellationToken)
    {
        try
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                ContractResolver = new PrivateSetterContractResolver()
            };

            var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(messageValue, settings);

            if (domainEvent == null)
            {
                _logger.LogWarning("Could not deserialize message to IDomainEvent");
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            
            switch (domainEvent)
            {
                case ShowtimeCreatedEvent showtimeCreated:
                    await HandleShowtimeCreated(scope, showtimeCreated, cancellationToken);
                    break;
                    
                case ReservationCreatedEvent reservationCreated:
                    await HandleReservationCreated(scope, reservationCreated, cancellationToken);
                    break;
                    
                case ReservationConfirmedEvent reservationConfirmed:
                    await HandleReservationConfirmed(scope, reservationConfirmed, cancellationToken);
                    break;
                    
                default:
                    _logger.LogDebug("Unhandled event type: {EventType}", domainEvent.GetType().Name);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message: {Message}", messageValue);
            throw; 
        }
    }

    private async Task HandleShowtimeCreated(
        IServiceScope scope, 
        ShowtimeCreatedEvent @event, 
        CancellationToken cancellationToken)
    {
        var repository = scope.ServiceProvider.GetRequiredService<IShowtimeReadRepository>();

        var readModel = new ShowtimeReadModel
        {
            Id = @event.ShowtimeId.Value,
            MovieImdbId = @event.MovieDetails.ImdbId,
            MovieTitle = @event.MovieDetails.Title,
            PosterUrl = @event.MovieDetails.PosterUrl,
            ReleaseYear = @event.MovieDetails.ReleaseYear,
            ScreeningTime = @event.ScreeningTime.Value,
            AuditoriumId = @event.AuditoriumId,
            Status = "Scheduled", 
            CreatedAt = @event.OccurredOnUtc,
            AvailableSeats = 100 
        };

        await repository.AddOrUpdateAsync(readModel, cancellationToken);
        _logger.LogInformation("Updated Showtime Read Model: {Id}", readModel.Id);
    }

    private async Task HandleReservationCreated(
        IServiceScope scope, 
        ReservationCreatedEvent @event, 
        CancellationToken cancellationToken)
    {
        var repository = scope.ServiceProvider.GetRequiredService<IReservationReadRepository>();
        var showtimeRepository = scope.ServiceProvider.GetRequiredService<IShowtimeReadRepository>();

        
        
        
        
        var showtime = await showtimeRepository.GetByIdAsync(@event.ShowtimeId.Value, cancellationToken);
        var movieTitle = showtime?.MovieTitle ?? "Unknown Movie";

        var readModel = new ReservationReadModel
        {
            Id = @event.ReservationId.Value,
            ShowtimeId = @event.ShowtimeId.Value,
            MovieTitle = movieTitle,
            ScreeningTime = showtime?.ScreeningTime ?? DateTime.MinValue,
            Seats = @event.Seats.Select(s => new SeatReadModel { Row = s.Row, Number = s.Number }).ToList(),
            Status = "Pending",
            CreatedAt = @event.OccurredOnUtc,
            ExpiresAt = @event.ExpiresAt
        };

        await repository.AddOrUpdateAsync(readModel, cancellationToken);
        _logger.LogInformation("Updated Reservation Read Model: {Id}", readModel.Id);
    }

    private async Task HandleReservationConfirmed(
        IServiceScope scope, 
        ReservationConfirmedEvent @event, 
        CancellationToken cancellationToken)
    {
        var repository = scope.ServiceProvider.GetRequiredService<IReservationReadRepository>();
        
        var reservation = await repository.GetByIdAsync(@event.ReservationId.Value, cancellationToken);
        
        if (reservation != null)
        {
            reservation.Status = "Confirmed";
            reservation.ConfirmedAt = @event.ConfirmedAt;
            await repository.AddOrUpdateAsync(reservation, cancellationToken);
            _logger.LogInformation("Confirmed Reservation Read Model: {Id}", reservation.Id);
        }
    }
}

public class PrivateSetterContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var prop = base.CreateProperty(member, memberSerialization);
        if (!prop.Writable)
        {
            var property = member as PropertyInfo;
            if (property != null)
            {
                var hasPrivateSetter = property.GetSetMethod(true) != null;
                prop.Writable = hasPrivateSetter;
            }
        }
        return prop;
    }
}
