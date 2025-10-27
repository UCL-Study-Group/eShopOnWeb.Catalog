using System.Diagnostics.Metrics;
using System.Text.Json;
using Catalog.Common.Models;
using Catalog.Common.Models.Base;
using Catalog.Infrastructure.Models;
using Catalog.Infrastructure.Repositories.Interfaces;
using FluentResults;
using Mapster;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

/// <summary>
/// A generic repository which is implemented to use MongoDb. It provides us with
/// the classic CRUD operations. Though I've hardcoded, so the only real type it
/// accepts is the CatalogItem
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TGet"></typeparam>
public class MongoRepository<TEntity, TGet> : IDbRepository<TEntity, TGet> where TEntity : BaseModel
{
    private readonly IMongoCollection<TEntity> _collection;
    private readonly IMongoCollection<Counter> _counterCollection;

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
        _collection = database.GetCollection<TEntity>(typeof(TEntity).Name);
        
        // For the counter collection
        _counterCollection = database.GetCollection<Counter>(nameof(Counter));
        
        // Create unique index on Id field
        var indexKeys = Builders<TEntity>.IndexKeys.Ascending(e => e.Id);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<TEntity>(indexKeys, indexOptions);
        _collection.Indexes.CreateOneAsync(indexModel).Wait();
    }
    
    public async Task<Result<IEnumerable<TGet>>> GetAllAsync(int? pageSize, int? pageIndex, string? brandId = null, string? typeId = null)
    {
        try
        {
            var filterBuilder = Builders<TEntity>.Filter;
            var filter = filterBuilder.Empty;
            
            if (!string.IsNullOrEmpty(brandId))
                filter &= filterBuilder.Eq("CatalogBrandId", brandId);
            
            if (!string.IsNullOrEmpty(typeId))
                filter &= filterBuilder.Eq("CatalogTypeId", typeId);
            
            // Since we already have defined the connection and collection
            // we can simply retrieve the entities with Find()
            var items = await _collection
                .Find(filter)
                .SortBy(i => i.Id)
                .Skip(pageIndex is null ? 0 : (pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return Result.Ok(items.AsEnumerable().Adapt<IEnumerable<TGet>>());
        }
        catch (Exception)
        {
            return Result.Fail("Could not retrieve collection");
        }
    }
    
    public async Task<Result<TGet>> GetByIdAsync(string id)
    {
        try
        {
            // Again, since the collection is defined we just need to 
            // specify which entity to retrieve
            var query = await _collection.FindAsync(e => e.MongoId == id);

            return Result.Ok(query.FirstOrDefault().Adapt<TGet>());
        }
        catch (Exception)
        {
            return Result.Fail("Couldn't find an item with the specified id");
        }
    }
    
    public async Task<Result<TGet>> GetByLegacyIdAsync(int id)
    {
        try
        {
            // Again, since the collection is defined we just need to 
            // specify which entity to retrieve
            var query = await _collection.FindAsync(e => e.Id == id);

            return Result.Ok(query.FirstOrDefault().Adapt<TGet>());
        }
        catch (Exception)
        {
            return Result.Fail("Couldn't find an item with the specified id");
        }
    }

    public async Task<Result<TGet>> CreateAsync(TEntity entity)
    {
        try
        {
            var idResult = await GetNextIdAsync(entity.Id);
            if (idResult.IsFailed)
                return Result.Fail("Failed to get ID");
    
            entity.Id = idResult.Value;
        
            // Check if item with this ID already exists
            var existing = await _collection.Find(e => e.Id == entity.Id).FirstOrDefaultAsync();
            if (existing != null)
                return Result.Fail($"Item with ID {entity.Id} already exists");
        
            await _collection.InsertOneAsync(entity);
            
            return Result.Ok(entity.Adapt<TGet>());
        }
        catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            return Result.Fail("Item with this ID already exists");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Couldn't create item: {ex.Message}");
        }
    }

    public async Task<Result<TGet>> UpdateAsync(TEntity entity)
    {
        try
        {
            FilterDefinition<TEntity> filter;
            
            if (entity.Id is not null)
                filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
            else
                filter = Builders<TEntity>.Filter.Eq(e => e.MongoId, entity.MongoId);
            
            var updatedEntity = await _collection.FindOneAndReplaceAsync(filter, entity);
            
            return Result.Ok(updatedEntity.Adapt<TGet>());
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

    private async Task<Result<int>> GetNextIdAsync(int? requestedId = null)
    {
        try
        {
            var filter = Builders<Counter>.Filter.Eq(c => c.Id, typeof(TEntity).Name);
        
            if (requestedId.HasValue)
            {
                var update = Builders<Counter>.Update.Max(c => c.Sequence, requestedId.Value);
                await _counterCollection.UpdateOneAsync(
                    filter, 
                    update, 
                    new UpdateOptions { IsUpsert = true });
            
                return Result.Ok(requestedId.Value);
            }
            else
            {
                var update = Builders<Counter>.Update.Inc(c => c.Sequence, 1);
                var options = new FindOneAndUpdateOptions<Counter>
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                };
            
                var counter = await _counterCollection.FindOneAndUpdateAsync(filter, update, options);
                return Result.Ok(counter.Sequence);
            }
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to get next ID: {ex.Message}");
        }
    }
}