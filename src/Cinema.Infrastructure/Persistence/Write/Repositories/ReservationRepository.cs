using Cinema.Application.Common.Interfaces.Persistence;
using Cinema.Domain.ReservationAggregate;
using Cinema.Domain.ReservationAggregate.ValueObjects;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;
using Cinema.Infrastructure.Persistence.Write;
using Microsoft.EntityFrameworkCore;
using Cinema.Domain.Common.Models;
using Cinema.Infrastructure.Persistence.Write.Outbox;
using Newtonsoft.Json;
using System.Linq;

namespace Cinema.Infrastructure.Persistence.Write.Repositories;




public class ReservationRepository : IReservationRepository
{
    private readonly CinemaDbContext _dbContext;

    public ReservationRepository(CinemaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Reservation?> GetByIdAsync(ReservationId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reservations
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<List<Reservation>> GetByShowtimeIdAsync(
        ShowtimeId showtimeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reservations
            .Where(r => r.ShowtimeId == showtimeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Reservation>> GetRecentReservationsByShowtimeAsync(
        ShowtimeId showtimeId,
        int minutes,
        CancellationToken cancellationToken = default)
    {
        var cutoffTime = DateTime.UtcNow.AddMinutes(-minutes);

        return await _dbContext.Reservations
            .Where(r => r.ShowtimeId == showtimeId && r.CreatedAt >= cutoffTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Reservation>> GetExpiredReservationsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        
        return await _dbContext.Reservations
            .Where(r => r.Status.IsPending && r.ExpiresAt < now)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await _dbContext.Reservations.AddAsync(reservation, cancellationToken);
    }

    public void Update(Reservation reservation)
    {
        _dbContext.Reservations.Update(reservation);
    }

    public void Remove(Reservation reservation)
    {
        _dbContext.Reservations.Remove(reservation);
    }
}




public class UnitOfWork : IUnitOfWork
{
    private readonly CinemaDbContext _dbContext;

    public UnitOfWork(CinemaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ConvertDomainEventsToOutboxMessages();
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private void ConvertDomainEventsToOutboxMessages()
    {
        var outboxMessages = _dbContext.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(x => x.Entity)
            .SelectMany(aggregate =>
            {
                var domainEvents = aggregate.DomainEvents.ToList();
                aggregate.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    })
            })
            .ToList();

        if (outboxMessages.Any())
        {
            _dbContext.Set<OutboxMessage>().AddRange(outboxMessages);
        }
    }
}
