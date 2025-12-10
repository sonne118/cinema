using Cinema.Domain.ShowtimeAggregate;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;

namespace Cinema.Application.Common.Interfaces.Persistence;




public interface IShowtimeRepository
{
    Task<Showtime?> GetByIdAsync(ShowtimeId id, CancellationToken cancellationToken = default);
    Task<List<Showtime>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Showtime showtime, CancellationToken cancellationToken = default);
    void Update(Showtime showtime);
    void Remove(Showtime showtime);
}
