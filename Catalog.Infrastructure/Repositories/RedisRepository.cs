using System.Text.Json;
using Catalog.Infrastructure.Repositories.Interfaces;
using FluentResults;
using StackExchange.Redis;

namespace Catalog.Infrastructure.Repositories;

public class RedisRepository : ICacheRepository
{
    private readonly IDatabase _database;
    private readonly IServer _server;
    private readonly TimeSpan _defaultExpiration;

    public RedisRepository(IConnectionMultiplexer connection, TimeSpan? defaultExpiration)
    {
        _database = connection.GetDatabase();
        _server = connection.GetServer(connection.GetEndPoints().First());
        _defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(10);
    }
    
    /// <summary>
    /// Retrieves the object(s) corresponding to the provided key
    /// </summary>
    /// <param name="key">Key to the cached object</param>
    /// <typeparam name="T">The type for the object</typeparam>
    /// <returns>The result with the object(s) from the provided key, if it exists</returns>
    public async Task<Result<T>> GetAsync<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
            return Result.Fail("You must provide a key");

        var response = await _database.StringGetAsync(key);

        if (string.IsNullOrEmpty(response))
            return Result.Fail("Found no cache for object");
        
        var deserialized = JsonSerializer.Deserialize<T>(response.ToString());
        
        return deserialized is null ? Result.Fail("Failed to return object") : Result.Ok(deserialized);
    }

    /// <summary>
    /// Retrieves the object(s) corresponding to the provided key
    /// </summary>
    /// <param name="key">Key to the cached object</param>
    /// <returns>The result with the object(s) from the provided key, if it exists</returns>
    public async Task<Result<string>> GetAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            return Result.Fail("You must provide a key");
        
        var response = await _database.StringGetAsync(key);

        if (string.IsNullOrEmpty(response))
            return Result.Fail("Found no cache for object");
        
        return !response.HasValue ? Result.Fail("Failed to return object") : Result.Ok(response.ToString());
    }

    /// <summary>
    /// Creates object(s) in the cache with key as its key value
    /// </summary>
    /// <param name="key">Key to store the value with</param>
    /// <param name="value">The object(s) which to cache</param>
    /// <typeparam name="T">The type for the object</typeparam>
    /// <returns>The result as well as the object(s) cached</returns>
    public async Task<Result> SetAsync<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key))
            return Result.Fail("You must provide a key");
        
        var serialized = JsonSerializer.Serialize(value);
        
        var response = await _database.StringSetAsync(key, serialized, _defaultExpiration);
        
        return response ? Result.Ok() : Result.Fail("Failed to insert object to cache");
    }

    /// <summary>
    /// Creates object(s) in the cache with key as its key value
    /// </summary>
    /// <param name="key">Key to store the value with</param>
    /// <param name="value">The object(s) which to cache</param>
    /// <returns>The result as well as the object(s) cached</returns>
    public async Task<Result> SetAsync(string key, string value)
    {
        if (string.IsNullOrEmpty(key))
            return Result.Fail("You must provide a key");
        
        var response = await _database.StringSetAsync(key, value, _defaultExpiration);
        
        return response ? Result.Ok() : Result.Fail("Failed to insert object to cache");
    }

    /// <summary>
    /// Removes the object(s) in the cache which matches the key
    /// </summary>
    /// <param name="key">The key to the object(s)</param>
    /// <returns>The result of the removal</returns>
    public async Task<Result> DeleteAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
            return Result.Fail("You must provide a key");

        var response = await _database.KeyDeleteAsync(key);
        
        return !response ? Result.Fail("Failed to delete key") : Result.Ok();
    }

    /// <summary>
    /// Flushes every key which matches the provided pattern.
    /// Is none is provided it just flushes the entire database
    ///
    /// NOTE: Providing no pattern will wipe EVERYTHING, MWAHHAHA
    /// </summary>
    /// <param name="pattern">The pattern which is to be deleted, defaults to "cache:*"</param>
    /// <returns>The result of the flush</returns>
    public async Task<Result> FlushAsync(string pattern = "cache:*")
    {
        var keys = _server.Keys(pattern: pattern);

        foreach (var key in keys)
            await _database.KeyDeleteAsync(key);
        
        return Result.Ok();
    }
}