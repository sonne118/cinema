using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Cinema.Infrastructure.Persistence.Read;





public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}




public class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://root:password123@localhost:27017";
    public string DatabaseName { get; set; } = "CinemaReadDb";
}
