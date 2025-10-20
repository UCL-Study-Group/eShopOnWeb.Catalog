using Catalog.Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Context;

/// <summary>
/// Most of this code is taken from a previous project I worked on
/// </summary>
public abstract class MongoDbContext
{
    protected readonly IMongoDatabase Database;

    protected MongoDbContext(IConfiguration configuration)
    {
        // I've decided to place the configuration files inside a config.json
        // since it should make it easier to configure while debugging
        var config = configuration ?? throw new ArgumentNullException(nameof(configuration));

        // Here I bind my credentials to the DbCredentials Object
        // so we can use it to create the client and database connection
        DbCredentials credentials = new();
        config.GetSection("DocumentDb").Bind(credentials);
        
        var client = new MongoClient(credentials.ConnectionString);
        
        Database = client.GetDatabase(credentials.DatabaseName);
    }
    
    /// <summary>
    /// Used to retrieve a collection (e.g. a model) from the database
    /// </summary>
    /// <param name="collectionName">The name of collection we want to retrieve</param>
    /// <typeparam name="T">Type of model that's being fetched from the database</typeparam>
    /// <returns></returns>
    public IMongoCollection<T> GetCollection<T>(string collectionName) => Database.GetCollection<T>(collectionName);
}