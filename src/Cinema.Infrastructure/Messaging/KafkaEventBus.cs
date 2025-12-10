using Cinema.Application.Common.Interfaces.Messaging;
using Cinema.Domain.Common.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Cinema.Infrastructure.Messaging;





public class KafkaEventBus : IEventBus
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaSettings _settings;
    private readonly ILogger<KafkaEventBus> _logger;

    public KafkaEventBus(
        IOptions<KafkaSettings> settings,
        ILogger<KafkaEventBus> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            ClientId = "cinema-api-producer",
            Acks = Acks.All,
            EnableIdempotence = true,
            MaxInFlight = 1,
            MessageSendMaxRetries = 3
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IDomainEvent
    {
        var eventType = @event.GetType().Name;
        var topic = _settings.DomainEventsTopic;

        var message = new Message<string, string>
        {
            Key = @event.EventId.ToString(),
            Value = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            }),
            Headers = new Headers
            {
                { "event-type", System.Text.Encoding.UTF8.GetBytes(eventType) },
                { "occurred-on", System.Text.Encoding.UTF8.GetBytes(@event.OccurredOnUtc.ToString("O")) }
            }
        };

        try
        {
            var result = await _producer.ProduceAsync(topic, message, cancellationToken);
            
            _logger.LogInformation(
                "Published event {EventType} to Kafka topic {Topic} at offset {Offset}",
                eventType,
                topic,
                result.Offset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Error publishing event {EventType} to Kafka", eventType);
            throw;
        }
    }

    public async Task PublishBatchAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : IDomainEvent
    {
        var tasks = events.Select(e => PublishAsync(e, cancellationToken));
        await Task.WhenAll(tasks);
    }
}




public class KafkaSettings
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string DomainEventsTopic { get; set; } = "cinema.domain.events";
    public string IntegrationEventsTopic { get; set; } = "cinema.integration.events";
    public string ConsumerGroupId { get; set; } = "cinema-api-consumer-group";
}
