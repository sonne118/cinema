namespace Cinema.MasterNode.Domain.Models;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime OccurredOnUtc { get; set; }
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }
}

public class OutboxBatch
{
    public List<OutboxMessage> Messages { get; set; } = new();
    public DateTime ClaimedAt { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
}
