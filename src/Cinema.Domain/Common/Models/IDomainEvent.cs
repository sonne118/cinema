namespace Cinema.Domain.Common.Models;





public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
    Guid EventId { get; }
}




public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
