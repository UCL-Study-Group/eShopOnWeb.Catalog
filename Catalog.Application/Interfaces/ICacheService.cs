using FluentResults;

namespace Catalog.Application.Interfaces;

public interface ICacheService
{
    Task<Result<T>> GetCacheAsync<T>(string key);
    Task<Result<string>> GetCacheAsync(string key);
    Task<Result<T>> InsertCacheAsync<T>(string key, T value);
    Task<Result<string>> InsertCacheAsync(string key, string value);
    Task<Result> DeleteCacheAsync(string key);
    Task<Result> FlushCacheAsync(string pattern = "cache:*");
}