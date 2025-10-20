using Catalog.Common.Models;
using Catalog.Common.Models.Base;
using Catalog.Infrastructure.Context;
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
public class MongoRepository<T> : MongoDbContext, IRepository<T> where T : BaseModel
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IConfiguration configuration) : base(configuration)
    {
        _collection = Database.GetCollection<T>(typeof(T).Name);
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
            // This one stands out, since ReplaceOneAsync() requires
            // us to define a filter, for it to get the entity.
            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            
            await _collection.ReplaceOneAsync(filter, entity);
            
            return Result.Ok();
        } catch (Exception)
        {
            return Result.Fail("Couldn't update provided item");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            await _collection.DeleteOneAsync(e => e.Id == id);
            
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail("Failed to delete provided id");
        }
    }
}