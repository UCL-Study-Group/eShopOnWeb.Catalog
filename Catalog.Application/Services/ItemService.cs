using Catalog.Application.Interfaces;
using Catalog.Common.Dtos;
using Catalog.Common.Dtos.Item;
using Catalog.Common.Models;
using Catalog.Infrastructure.Repositories.Interfaces;
using FluentResults;

namespace Catalog.Application.Services;

public class ItemService : IItemService
{
    private readonly IDbRepository<CatalogItem> _itemDbRepository;
    private readonly ICacheService _cacheService;

    public ItemService(
        IDbRepository<CatalogItem> itemDbRepository, 
        ICacheService cacheService)
    {
        _itemDbRepository = itemDbRepository;
        _cacheService = cacheService;
    }
    
    public async Task<IEnumerable<GetCatalogItemDto>?> GetAllAsync(
        int? pageSize, 
        int? pageIndex
        )
    {
        var results = await _itemDbRepository.GetAllAsync(pageSize, pageIndex);

        if (results.IsFailed)
            return null;
        
        var mapped = results.Value.Select(r => new GetCatalogItemDto()
        {
            Id = r.Id,
            MongoId = r.MongoId,
            Name = r.Name,
            Price = r.Price,
            ImageUrl = r.ImageUrl,
            Description = r.Description,
            CatalogBrandId = r.CatalogBrandId,
            CatalogTypeId = r.CatalogTypeId
        });
        
        return mapped;
    }
    
    public async Task<IEnumerable<GetCatalogItemDto>?> GetAllAsync(
        int? pageSize, 
        int? pageIndex,
        string? brandId,
        string? typeId
    )
    {
        var results = await _itemDbRepository.GetAllAsync(pageSize, pageIndex, brandId, typeId);

        if (results.IsFailed)
            return null;
        
        var mapped = results.Value.Select( r => new GetCatalogItemDto()
        {
            Id = r.Id,
            MongoId = r.MongoId,
            Name = r.Name,
            Price = r.Price,
            ImageUrl = r.ImageUrl,
            Description = r.Description,
            CatalogBrandId = r.CatalogBrandId,
            CatalogTypeId = r.CatalogTypeId
        });
        
        return mapped;
    }

    public async Task<GetCatalogItemDto?> GetAsync(string id)
    {
        var result = await _itemDbRepository.GetByIdAsync(id);

        if (result.IsFailed)
            return null;

        return new GetCatalogItemDto()
        {
            Id = result.Value.Id,
            MongoId = result.Value.MongoId,
            Name = result.Value.Name,
            Price = result.Value.Price,
            ImageUrl = result.Value.ImageUrl,
            Description = result.Value.Description,
            CatalogBrandId = result.Value.CatalogBrandId,
            CatalogTypeId = result.Value.CatalogTypeId
        };
    }

    public async Task<CatalogItem?> CreateAsync(CreateCatalogItemDto catalogItem)
    {
        var result = await _itemDbRepository.CreateAsync(new CatalogItem
        {
            Name = catalogItem.Name,
            Price = catalogItem.Price,
            ImageUrl = catalogItem.ImageUrl,
            Description = catalogItem.Description,
            CatalogBrandId = catalogItem.CatalogBrand,
            CatalogTypeId = catalogItem.CatalogType,
        });

        await _cacheService.FlushCacheAsync("cache:/api/catalog-items");

        return result.IsFailed ? null : result.Value;
    }

    public async Task<Result> UpdateAsync(UpdateCatalogItemDto catalogItem)
    {
        if (catalogItem.Id is null && string.IsNullOrEmpty(catalogItem.MongoId))
            return Result.Fail("You need to provide an ID");
        
        Result<CatalogItem> existingBrand;

        if (catalogItem.Id is not null)
            existingBrand = await _itemDbRepository.GetByLegacyIdAsync(catalogItem.Id.Value);
        else
            existingBrand = await _itemDbRepository.GetByIdAsync(catalogItem.MongoId!);

        var updatedItem = new CatalogItem
        {
            Id = existingBrand.Value.Id,
            MongoId = existingBrand.Value.MongoId,
            Name = catalogItem.Name ?? existingBrand.Value.Name,
            Price = catalogItem.Price ?? existingBrand.Value.Price,
            ImageUrl = catalogItem.ImageUrl ?? existingBrand.Value.ImageUrl,
            Description = catalogItem.Description ?? existingBrand.Value.Description,
            CatalogBrandId = catalogItem.CatalogBrand ?? existingBrand.Value.CatalogBrandId,
            CatalogTypeId = catalogItem.CatalogType ?? existingBrand.Value.CatalogTypeId,
        };
        
        var result = await _itemDbRepository.UpdateAsync(updatedItem);
        
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok();
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var result = await _itemDbRepository.DeleteAsync(id);
        
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok();
    }
}