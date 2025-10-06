using Catalog.Infrastructure.Models;
using Catalog.Infrastructure.Repositories;
using FluentResults;

namespace Catalog.Application.Services;

public class CatalogService(MongoRepository<CatalogItem> repository)
{
    public async Task<Result<IEnumerable<CatalogItem>>> GetAllAsync()
    {
        var result = await repository.GetAllAsync();

        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok(result.Value);
    }

    public async Task<Result<CatalogItem>> GetByIdAsync(int id)
    {
        var result = await repository.GetByIdAsync(id);
        
        return result.IsFailed ? Result.Fail<CatalogItem>(result.Errors) : Result.Ok(result.Value);
    }

    public async Task<Result> CreateAsync(CatalogItem catalogItem)
    {
        var result = await repository.CreateAsync(catalogItem);
        
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok();
    }

    public async Task<Result> UpdateAsync(CatalogItem catalogItem)
    {
        var result = await repository.UpdateAsync(catalogItem);
        
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var result = await repository.DeleteAsync(id);
        
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok();
    }
}