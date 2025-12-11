using Cinema.MasterNode.Core.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cinema.MasterNode.Core.Services;

public class MasterNodeWorker : BackgroundService
{
    private readonly IOutboxProcessor _outboxProcessor;
    private readonly ILogger<MasterNodeWorker> _logger;

    public MasterNodeWorker(
        IOutboxProcessor outboxProcessor,
        ILogger<MasterNodeWorker> logger)
    {
        _outboxProcessor = outboxProcessor;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Master Node Worker starting...");
        
        // Start outbox processor
        await _outboxProcessor.StartProcessingAsync(stoppingToken);
    }
}
