using Catalog.Infrastructure.Context;
using Catalog.Infrastructure.Interfaces;
using Catalog.Infrastructure.Models;
using FluentResults;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

/// <summary>
/// A generic repository which is implemented to use MongoDb. It provides us with
/// the classic CRUD operations. Though I've hardcoded, so the only real type it
/// accepts is the CatalogItem
/// </summary>
/// <typeparam name="T"></typeparam>
public class MongoRepository<T> : MongoDbContext, IRepository<T> where T : CatalogItem
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository()
    {
        _collection = Database.GetCollection<T>(typeof(T).Name);
    }
    
    /// <summary>
    /// Retrieves all entities of the provided type from the database
    /// </summary>
    /// <returns>A collection of entities</returns>
    public async Task<Result<IEnumerable<T>>> GetAllAsync()
    {
        try
        {
            // Since we already have defined the connection and collection
            // we can simply retrievve the entities with Find()
            var items = await _collection.Find(_ => true).ToListAsync();

            return Result.Ok(items.AsEnumerable());
        }
        catch (Exception)
        {
            return Result.Fail("Could not retrieve collection");
        }
    }

    /// <summary>
    /// Retrieves the first entity with the provided id.
    /// </summary>
    /// <param name="id">The id of the type</param>
    /// <returns>The entity if found</returns>
    public async Task<Result<T>> GetByIdAsync(int id)
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

    /// <summary>
    /// Creates a new entity of the provided type and saves it
    /// </summary>
    /// <param name="entity">The entity that is to be created</param>
    /// <returns>A result indicating success or failure</returns>
    public async Task<Result> CreateAsync(T entity)
    {
        try
        {
            await _collection.InsertOneAsync(entity);

            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail("Couldn't create provided item");
        }
    }

    /// <summary>
    /// Updates an already existing entity with provided one
    /// </summary>
    /// <param name="entity">The entity which should be updated</param>
    /// <returns>A result indicating success or failure</returns>
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

    /// <summary>
    /// Removes the entity matching the id provided 
    /// </summary>
    /// <param name="id">The id of the entity to be deleted</param>
    /// <returns>A result indicating success or failure</returns>
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