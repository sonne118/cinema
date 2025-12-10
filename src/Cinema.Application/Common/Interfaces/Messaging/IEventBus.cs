using Cinema.Domain.Common.Models;

namespace Cinema.Application.Common.Interfaces.Messaging;




public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}
