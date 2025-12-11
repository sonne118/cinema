using Cinema.Domain.Common.Models;
using Cinema.Infrastructure.Persistence.Write.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace Cinema.Infrastructure.Persistence.Write.Interceptors;






public class PublishDomainEventsInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;

        if (dbContext is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var events = dbContext.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(x => x.Entity)
            .SelectMany(aggregate =>
            {
                var domainEvents = aggregate.DomainEvents.ToList();
                aggregate.ClearDomainEvents();
                return domainEvents;
            })
            .ToList();

        if (!events.Any())
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var outboxMessages = events.Select(domainEvent => new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredOnUtc = DateTime.UtcNow,
            Type = domainEvent.GetType().Name,
            Content = JsonConvert.SerializeObject(
                domainEvent,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                })
        }).ToList();

        await dbContext.Set<OutboxMessage>().AddRangeAsync(outboxMessages, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
