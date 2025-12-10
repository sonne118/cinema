using Cinema.Application.Common.Models;

namespace Cinema.Application.Common.Interfaces.Queries;




public interface IShowtimeReadRepository
{
    Task<ShowtimeReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ShowtimeReadModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<ShowtimeReadModel>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task AddOrUpdateAsync(ShowtimeReadModel model, CancellationToken cancellationToken = default);
}




public interface IReservationReadRepository
{
    Task<ReservationReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ReservationReadModel>> GetByShowtimeIdAsync(Guid showtimeId, CancellationToken cancellationToken = default);
    Task AddOrUpdateAsync(ReservationReadModel model, CancellationToken cancellationToken = default);
}
