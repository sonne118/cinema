namespace Cinema.Domain.Common.Models;






public abstract class AggregateRoot<TId> : Entity<TId>, IHasDomainEvents
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public int DebugEventsCount => _domainEvents.Count;

    protected AggregateRoot(TId id) : base(id)
    {
    }

    
    
    
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    
    
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

#pragma warning disable CS8618
    protected AggregateRoot() { }
#pragma warning restore CS8618
}
