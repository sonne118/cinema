using Cinema.Application.Common.Interfaces.Queries;
using Cinema.Application.Common.Models;
using MongoDB.Driver;

namespace Cinema.Infrastructure.Persistence.Read.Repositories;





public class ShowtimeReadRepository : IShowtimeReadRepository
{
    private readonly IMongoCollection<ShowtimeReadModel> _collection;

    public ShowtimeReadRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<ShowtimeReadModel>("showtimes");
    }

    public async Task<ShowtimeReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ShowtimeReadModel>.Filter.Eq(s => s.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<ShowtimeReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(Builders<ShowtimeReadModel>.Filter.Empty)
            .SortBy(s => s.ScreeningTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ShowtimeReadModel>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<ShowtimeReadModel>.Filter.And(
            Builders<ShowtimeReadModel>.Filter.Gte(s => s.ScreeningTime, startDate),
            Builders<ShowtimeReadModel>.Filter.Lte(s => s.ScreeningTime, endDate)
        );

        return await _collection
            .Find(filter)
            .SortBy(s => s.ScreeningTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddOrUpdateAsync(ShowtimeReadModel model, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ShowtimeReadModel>.Filter.Eq(s => s.Id, model.Id);
        var options = new ReplaceOptions { IsUpsert = true };
        
        await _collection.ReplaceOneAsync(filter, model, options, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<ShowtimeReadModel>.Filter.Eq(s => s.Id, id);
        await _collection.DeleteOneAsync(filter, cancellationToken);
    }
}
