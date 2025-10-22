using Catalog.Application.Interfaces;
using Catalog.Infrastructure.Repositories.Interfaces;
using FluentResults;

namespace Catalog.Application.Services;

public class RedisCacheService : ICacheService
{
    private readonly ICacheRepository _cacheRepository;

    public RedisCacheService(ICacheRepository cacheRepository)
    {
        _cacheRepository = cacheRepository;
    }

    public async Task<Result<T>> GetCacheAsync<T>(string key)
    {
        return await _cacheRepository.GetAsync<T>(key);
    }

    public async Task<Result<string>> GetCacheAsync(string key)
    {
        return await _cacheRepository.GetAsync(key);
    }

    public async Task<Result<T>> InsertCacheAsync<T>(string key, T value)
    {
        return await _cacheRepository.SetAsync(key, value);
    }

    public async Task<Result<string>> InsertCacheAsync(string key, string value)
    {
        return await _cacheRepository.SetAsync(key, value);
    }

    public async Task<Result> DeleteCacheAsync(string key)
    {
        return await _cacheRepository.DeleteAsync(key);
    }

    public async Task<Result> FlushCacheAsync()
    {
        return await _cacheRepository.FlushAsync();
    }
}