using Cinema.Application.Common.Interfaces.Messaging;
using Cinema.Domain.Common.Models;
using Cinema.Infrastructure.Persistence.Write;
using Cinema.Infrastructure.Persistence.Write.Outbox;
using Cinema.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

namespace Cinema.Infrastructure.BackgroundJobs;






[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob : IJob
{
    private readonly CinemaDbContext _dbContext;
    private readonly IEventBus _eventBus;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    public ProcessOutboxMessagesJob(
        CinemaDbContext dbContext,
        IEventBus eventBus,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _dbContext = dbContext;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Processing outbox messages...");

        
        var messages = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        if (!messages.Any())
        {
            _logger.LogDebug("No outbox messages to process");
            return;
        }

        _logger.LogInformation("Found {Count} outbox messages to process", messages.Count);

        
        foreach (var message in messages)
        {
            try
            {
                
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    message.Content,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        ContractResolver = new PrivateSetterContractResolver()
                    });

                if (domainEvent is null)
                {
                    _logger.LogWarning("Failed to deserialize message {MessageId}", message.Id);
                    message.Error = "Failed to deserialize";
                    message.ProcessedOnUtc = DateTime.UtcNow;
                    continue;
                }

                
                await _eventBus.PublishAsync(domainEvent, context.CancellationToken);

                
                message.ProcessedOnUtc = DateTime.UtcNow;

                _logger.LogInformation(
                    "Successfully processed outbox message {MessageId} of type {Type}",
                    message.Id,
                    message.Type);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing outbox message {MessageId} of type {Type}",
                    message.Id,
                    message.Type);

                message.Error = ex.Message;
                message.ProcessedOnUtc = DateTime.UtcNow; 
            }
        }

        
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Finished processing {Count} outbox messages", messages.Count);
    }
}
