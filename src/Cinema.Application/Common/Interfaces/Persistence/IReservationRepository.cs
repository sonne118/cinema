using Cinema.Domain.ReservationAggregate;
using Cinema.Domain.ReservationAggregate.ValueObjects;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;

namespace Cinema.Application.Common.Interfaces.Persistence;




public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(ReservationId id, CancellationToken cancellationToken = default);
    Task<List<Reservation>> GetByShowtimeIdAsync(ShowtimeId showtimeId, CancellationToken cancellationToken = default);
    Task<List<Reservation>> GetExpiredReservationsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    void Update(Reservation reservation);
    void Remove(Reservation reservation);
}
