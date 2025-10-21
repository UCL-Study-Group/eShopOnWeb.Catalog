using FluentResults;

namespace Catalog.Infrastructure.Repositories.Interfaces;

public interface ICacheRepository
{
    Task<Result<T>> GetAsync<T>(string key);
    Task<Result<string>> GetAsync(string key);
    Task<Result> SetAsync<T>(string key, T value);
    Task<Result> SetAsync(string key, string value);
    Task<Result> DeleteAsync(string key);
    Task<Result> FlushAsync(string pattern = "cache:*");
}