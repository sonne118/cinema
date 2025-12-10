using Cinema.Application.Common.Interfaces.Persistence;
using Cinema.Domain.ShowtimeAggregate;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;
using Cinema.Infrastructure.Persistence.Write;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Infrastructure.Persistence.Write.Repositories;




public class ShowtimeRepository : IShowtimeRepository
{
    private readonly CinemaDbContext _dbContext;

    public ShowtimeRepository(CinemaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Showtime?> GetByIdAsync(ShowtimeId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Showtimes
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<Showtime>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Showtimes
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Showtime showtime, CancellationToken cancellationToken = default)
    {
        await _dbContext.Showtimes.AddAsync(showtime, cancellationToken);
    }

    public void Update(Showtime showtime)
    {
        _dbContext.Showtimes.Update(showtime);
    }

    public void Remove(Showtime showtime)
    {
        _dbContext.Showtimes.Remove(showtime);
    }

    public async Task<bool> ExistsAsync(ShowtimeId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Showtimes
            .AnyAsync(s => s.Id == id, cancellationToken);
    }
}
