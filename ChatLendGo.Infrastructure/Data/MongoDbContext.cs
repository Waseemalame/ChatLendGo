using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ChatLendGo.Infrastructure.Data;

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
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}