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
    
    public async Task<IEnumerable<CatalogItemCreateDto>?> GetAllAsync()
    {
        var results = await _itemRepository.GetAllAsync();

        if (results.IsFailed)
            return null;
        
        var mappedTask = results.Value.Select(async r => new CatalogItemCreateDto()
        {
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

    public async Task<CatalogItemCreateDto?> GetByIdAsync(int id)
    {
        var result = await _itemRepository.GetByIdAsync(id);

        if (result.IsFailed)
            return null;

        return new CatalogItemCreateDto()
        {
            Name = result.Value.Name,
            Price = result.Value.Price,
            ImageUrl = result.Value.ImageUrl,
            Description = result.Value.Description,
            CatalogBrand = await _brandService.GetBrandNameAsync(result.Value.CatalogBrandId),
            CatalogType = await _typeService.GetTypeNameAsync(result.Value.CatalogTypeId)
        };
    }

    public async Task<CatalogItemCreateDto?> CreateAsync(CatalogItem catalogItem)
    {
        var result = await _itemRepository.CreateAsync(catalogItem);

        if (result.IsFailed)
            return null;
        
        return new CatalogItemCreateDto()
        {
            Name = result.Value.Name,
            Price = result.Value.Price,
            ImageUrl = result.Value.ImageUrl,
            Description = result.Value.Description,
            CatalogBrand = await _brandService.GetBrandNameAsync(result.Value.CatalogBrandId),
            CatalogType = await _typeService.GetTypeNameAsync(result.Value.CatalogTypeId)
        };
    }

    public async Task<Result> UpdateAsync(CatalogItem catalogItem)
    {
        var result = await _itemRepository.UpdateAsync(catalogItem);
        
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var result = await _itemRepository.DeleteAsync(id);
        
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok();
    }
}