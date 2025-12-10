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
        try
        {
            var dbContext = eventData.Context;

            if (dbContext is null)
            {
                return await base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            
            var entries = dbContext.ChangeTracker.Entries().ToList();
            var aggregatesWithEvents = new List<object>();
            
            foreach (var entry in entries)
            {
                try 
                {
                    var entity = entry.Entity;
                    var prop = entity.GetType().GetProperty("DomainEvents");
                    if (prop != null)
                    {
                        var events = prop.GetValue(entity) as IEnumerable<object>;
                        if (events != null && events.Any())
                        {
                            aggregatesWithEvents.Add(entity);
                        }
                    }
                }
                catch {}
            }

            
            var outboxMessages = new List<OutboxMessage>();
            
            foreach (var aggregate in aggregatesWithEvents)
            {
                 var prop = aggregate.GetType().GetProperty("DomainEvents");
                 var events = prop.GetValue(aggregate) as IEnumerable<object>;
                 
                 if (events == null) continue;

                 foreach (var domainEvent in events)
                 {
                     try
                     {
                         
                         var occurredOn = DateTime.UtcNow;
                         try { occurredOn = (DateTime)((dynamic)domainEvent).OccurredOnUtc; } catch {}

                         
                         
                         
                         
                         
                         var json = "TEST";
                         
                         outboxMessages.Add(new OutboxMessage 
                         { 
                            Id = Guid.NewGuid(),
                            Type = domainEvent.GetType().Name,
                            Content = json,
                            OccurredOnUtc = occurredOn
                         });
                     }
                     catch
                     {
                         
                     }
                 }
                 
                 
             
             
             
             
             
             
        }

        
        
        
        
        }
        catch (Exception ex)
        {
            Console.WriteLine($"INTERCEPTOR CRASH: {ex}");
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
