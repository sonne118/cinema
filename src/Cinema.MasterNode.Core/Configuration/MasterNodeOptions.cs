namespace Cinema.MasterNode.Core.Configuration;

public class MasterNodeOptions
{
    public int PollingIntervalMs { get; set; } = 1000;
    public int BatchSize { get; set; } = 1000;
    public int MaxConcurrentBatches { get; set; } = 8;
    public int ProcessorThreads { get; set; } = 4;
    public int MaxRetryAttempts { get; set; } = 3;
    public int LockTimeoutSeconds { get; set; } = 30;
}
