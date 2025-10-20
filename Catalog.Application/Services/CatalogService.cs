using Catalog.Common.Dtos;
using Catalog.Common.Models;
using Catalog.Infrastructure.Repositories.Interfaces;
using FluentResults;

namespace Catalog.Application.Services;

public class CatalogService
{
    private readonly IRepository<CatalogItem> _itemRepository;
    private readonly BrandService _brandService;
    private readonly TypeService _typeService;

    public CatalogService(
        IRepository<CatalogItem> itemRepository, 
        BrandService brandService, TypeService typeService)
    {
        _itemRepository = itemRepository;
        _brandService = brandService;
        _typeService = typeService;
    }
    
    public async Task<IEnumerable<GetCatalogItemDto>?> GetAllAsync()
    {
        var results = await _itemRepository.GetAllAsync();

        if (results.IsFailed)
            return null;
        
        var mappedTask = results.Value.Select(async r => new GetCatalogItemDto()
        {
            Id = r.Id,
            MongoId = r.MongoId,
            Name = r.Name,
            Price = r.Price,
            ImageUrl = r.ImageUrl,
            Description = r.Description,
            CatalogBrand = await _brandService.GetBrandNameAsync(r.CatalogBrandId),
            CatalogType = await _typeService.GetTypeNameAsync(r.CatalogTypeId)
        });
        
        var mapped = await Task.WhenAll(mappedTask);

        return mapped;
    }

    public async Task<GetCatalogItemDto?> GetByIdAsync(string id)
    {
        var result = await _itemRepository.GetByIdAsync(id);

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
            CatalogBrand = await _brandService.GetBrandNameAsync(result.Value.CatalogBrandId),
            CatalogType = await _typeService.GetTypeNameAsync(result.Value.CatalogTypeId)
        };
    }

    public async Task<CatalogItem?> CreateAsync(CreateCatalogItemDto catalogItem)
    {
        var result = await _itemRepository.CreateAsync(new CatalogItem
        {
            Name = catalogItem.Name,
            Price = catalogItem.Price,
            ImageUrl = catalogItem.ImageUrl,
            Description = catalogItem.Description,
            CatalogBrandId = catalogItem.CatalogBrand,
            CatalogTypeId = catalogItem.CatalogType,
        });

        return result.IsFailed ? null : result.Value;
    }

    public async Task<Result> UpdateAsync(UpdateCatalogItemDto catalogItem)
    {
        if (catalogItem.Id is null && string.IsNullOrEmpty(catalogItem.MongoId))
            return Result.Fail("You need to provide an ID");
        
        Result<CatalogItem> existingBrand;

        if (catalogItem.Id is not null)
            existingBrand = await _itemRepository.GetByLegacyIdAsync(catalogItem.Id.Value);
        else
            existingBrand = await _itemRepository.GetByIdAsync(catalogItem.MongoId!);

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
        
        var result = await _itemRepository.UpdateAsync(updatedItem);
        
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok();
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var result = await _itemRepository.DeleteAsync(id);
        
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok();
    }
}