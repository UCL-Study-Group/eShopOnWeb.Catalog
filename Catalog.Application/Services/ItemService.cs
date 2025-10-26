using System.Text.Json;
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
            PictureUri = r.PictureUri,
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
            PictureUri = r.PictureUri,
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
            PictureUri = result.Value.PictureUri,
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
            PictureUri = catalogItem.PictureUri,
            Description = catalogItem.Description,
            CatalogBrandId = catalogItem.CatalogBrandId,
            CatalogTypeId = catalogItem.CatalogTypeId,
        });

        await _cacheService.FlushCacheAsync("cache:/api/catalog-items");

        return result.IsFailed ? null : result.Value;
    }

    public async Task<Result<GetCatalogItemsListDto>> UpdateAsync(UpdateCatalogItemDto catalogItem)
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
            PictureUri = catalogItem.PictureUri ?? existingBrand.Value.PictureUri,
            Description = catalogItem.Description ?? existingBrand.Value.Description,
            CatalogBrandId = catalogItem.CatalogBrandId ?? existingBrand.Value.CatalogBrandId,
            CatalogTypeId = catalogItem.CatalogTypeId ?? existingBrand.Value.CatalogTypeId,
        };
        
        var response = await _itemDbRepository.UpdateAsync(updatedItem);

        if (response.IsFailed)
            return Result.Fail(response.Errors);
        
        await _cacheService.FlushCacheAsync("cache:/api/catalog-items");
        
        var deserialized = JsonSerializer.Deserialize<GetCatalogItemDto>(response.Value);
        
        if (deserialized is null)
            return Result.Fail("Failed to deserialize brand");
        
        return Result.Ok(new GetCatalogItemsListDto()
        {
            CatalogItems = [deserialized]
        });
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var result = await _itemDbRepository.DeleteAsync(id);
        
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok();
    }
}