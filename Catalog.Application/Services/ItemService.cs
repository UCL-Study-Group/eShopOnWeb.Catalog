using System.Text.Json;
using Catalog.Application.Interfaces;
using Catalog.Common.Dtos.Item;
using Catalog.Common.Models;
using Catalog.Infrastructure.Repositories.Interfaces;
using FluentResults;
using Mapster;

namespace Catalog.Application.Services;

public class ItemService : IItemService
{
    private readonly IDbRepository<CatalogItem, GetCatalogItemDto> _itemDbRepository;
    private readonly ICacheService _cacheService;

    public ItemService(
        IDbRepository<CatalogItem, GetCatalogItemDto> itemDbRepository, 
        ICacheService cacheService)
    {
        _itemDbRepository = itemDbRepository;
        _cacheService = cacheService;
    }
    
    public async Task<Result<IEnumerable<GetCatalogItemDto>>> GetAllAsync(
        int? pageSize, 
        int? pageIndex
        )
    {
        var results = await _itemDbRepository.GetAllAsync(pageSize, pageIndex);

        if (results.IsFailed)
            return Result.Fail(results.Errors);
        
        return Result.Ok(results.Adapt<IEnumerable<GetCatalogItemDto>>());
    }
    
    public async Task<Result<IEnumerable<GetCatalogItemDto>>> GetAllAsync(
        int? pageSize, 
        int? pageIndex,
        string? brandId,
        string? typeId
    )
    {
        var results = await _itemDbRepository.GetAllAsync(pageSize, pageIndex, brandId, typeId);

        if (results.IsFailed)
            return Result.Fail(results.Errors);
        
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
        
        return Result.Ok(results.Value.Adapt<IEnumerable<GetCatalogItemDto>>());
    }

    public async Task<Result<GetCatalogItemDto>> GetAsync(string id)
    {
        var result = await _itemDbRepository.GetByIdAsync(id);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        return Result.Ok(result.Adapt<GetCatalogItemDto>());
    }

    public async Task<Result<GetCatalogItemDto>> CreateAsync(CreateCatalogItemDto catalogItem)
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

        return Result.Ok(result.Adapt<GetCatalogItemDto>());
    }

    public async Task<Result<GetCatalogItemDto>> UpdateAsync(UpdateCatalogItemDto catalogItem)
    {
        if (catalogItem.Id is null && string.IsNullOrEmpty(catalogItem.MongoId))
            return Result.Fail("You need to provide an ID");
        
        Result<GetCatalogItemDto> existingBrand;

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

        return response;
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var result = await _itemDbRepository.DeleteAsync(id);
        
        await _cacheService.FlushCacheAsync("cache:/api/catalog-items");
        
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok();
    }
}