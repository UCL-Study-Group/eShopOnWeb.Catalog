using Catalog.Common.Models.Base;
using Catalog.Infrastructure.Models;
using Catalog.Infrastructure.Repositories.Interfaces;
using FluentResults;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

/// <summary>
/// A generic repository which is implemented to use MongoDb. It provides us with
/// the classic CRUD operations. Though I've hardcoded, so the only real type it
/// accepts is the CatalogItem
/// </summary>
/// <typeparam name="T"></typeparam>
public class MongoRepository<T> : IDbRepository<T> where T : BaseModel
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IConfiguration configuration)
    {
        var config = configuration ?? throw new ArgumentNullException(nameof(configuration));

        // Bind credentials from configuration
        DbCredentials credentials = new();
        config.GetSection("DocumentDb").Bind(credentials);
        
        // Create MongoDB client and get database
        var client = new MongoClient(credentials.ConnectionString);
        var database = client.GetDatabase(credentials.DatabaseName);
        
        // Get the collection based on the type name
        _collection = database.GetCollection<T>(typeof(T).Name);
    }
    
    public async Task<Result<IEnumerable<T>>> GetAllAsync()
    {
        try
        {
            // Since we already have defined the connection and collection
            // we can simply retrieve the entities with Find()
            var items = await _collection.Find(_ => true).ToListAsync();

            return Result.Ok(items.AsEnumerable());
        }
        catch (Exception)
        {
            return Result.Fail("Could not retrieve collection");
        }
    }
    
    public async Task<Result<T>> GetByIdAsync(string id)
    {
        try
        {
            // Again, since the collection is defined we just need to 
            // specify which entity to retrieve
            var query = await _collection.FindAsync(e => e.MongoId == id);

            return Result.Ok(query.FirstOrDefault());
        }
        catch (Exception)
        {
            return Result.Fail("Couldn't find an item with the specified id");
        }
    }
    
    public async Task<Result<T>> GetByLegacyIdAsync(int id)
    {
        try
        {
            // Again, since the collection is defined we just need to 
            // specify which entity to retrieve
            var query = await _collection.FindAsync(e => e.Id == id);

            return Result.Ok(query.FirstOrDefault());
        }
        catch (Exception)
        {
            return Result.Fail("Couldn't find an item with the specified id");
        }
    }

    public async Task<Result<T>> CreateAsync(T entity)
    {
        try
        {
            await _collection.InsertOneAsync(entity);

            return Result.Ok(entity);
        }
        catch (Exception)
        {
            return Result.Fail("Couldn't create provided item");
        }
    }

    public async Task<Result> UpdateAsync(T entity)
    {
        try
        {
            FilterDefinition<T> filter;
            
            if (entity.Id is not null)
                filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            else
                filter = Builders<T>.Filter.Eq(e => e.MongoId, entity.MongoId);
            
            await _collection.ReplaceOneAsync(filter, entity);
            
            return Result.Ok();
        } catch (Exception)
        {
            return Result.Fail("Couldn't update provided item");
        }
    }

    public async Task<Result> DeleteAsync(string id)
    {
        try
        {
            if (int.TryParse(id, out var idInt))
                await _collection.DeleteOneAsync(e => e.Id == idInt);
            else
                await _collection.DeleteOneAsync(e => e.MongoId == id);
            
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail("Failed to delete provided id");
        }
    }
}