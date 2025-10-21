using FluentResults;

namespace Catalog.Infrastructure.Repositories.Interfaces;

public interface ICacheRepository
{
    /// <summary>
    /// Retrieves the object(s) corresponding to the provided key
    /// </summary>
    /// <param name="key">Key to the cached object</param>
    /// <typeparam name="T">The type for the object</typeparam>
    /// <returns>The result with the object(s) from the provided key, if it exists</returns>
    Task<Result<T>> GetAsync<T>(string key);
    
    /// <summary>
    /// Retrieves the object(s) corresponding to the provided key
    /// </summary>
    /// <param name="key">Key to the cached object</param>
    /// <returns>The result with the object(s) from the provided key, if it exists</returns>
    Task<Result<string>> GetAsync(string key);
    
    /// <summary>
    /// Creates object(s) in the cache with key as its key value
    /// </summary>
    /// <param name="key">Key to store the value with</param>
    /// <param name="value">The object(s) which to cache</param>
    /// <typeparam name="T">The type for the object</typeparam>
    /// <returns>The result as well as the object(s) cached</returns>
    Task<Result> SetAsync<T>(string key, T value);
    
    /// <summary>
    /// Creates object(s) in the cache with key as its key value
    /// </summary>
    /// <param name="key">Key to store the value with</param>
    /// <param name="value">The object(s) which to cache</param>
    /// <returns>The result as well as the object(s) cached</returns>
    Task<Result> SetAsync(string key, string value);
    
    /// <summary>
    /// Removes the object(s) in the cache which matches the key
    /// </summary>
    /// <param name="key">The key to the object(s)</param>
    /// <returns>The result of the removal</returns>
    Task<Result> DeleteAsync(string key);
    
    /// <summary>
    /// Flushes every key which matches the provided pattern.
    /// Is none is provided it just flushes the entire database
    ///
    /// NOTE: Providing no pattern will wipe EVERYTHING, MWAHHAHA
    /// </summary>
    /// <param name="pattern">The pattern which is to be deleted, defaults to "cache:*"</param>
    /// <returns>The result of the flush</returns>
    Task<Result> FlushAsync(string pattern = "cache:*");
}