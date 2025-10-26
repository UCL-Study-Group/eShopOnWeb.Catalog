using System.Text.Json;
using Catalog.Application.Interfaces;
using Catalog.Common.Dtos;
using Catalog.Common.Dtos.Brand;
using Catalog.Common.Models;
using Catalog.Infrastructure.Repositories.Interfaces;
using FluentResults;

namespace Catalog.Application.Services;

public class BrandService : IBrandService
{
    private readonly IDbRepository<CatalogBrand> _dbRepository;
    private readonly ICacheService _cacheService;

    public BrandService(
        IDbRepository<CatalogBrand> dbRepository, 
        ICacheService cacheService)
    {
        _dbRepository = dbRepository;
        _cacheService = cacheService;
    }

    public async Task<string> GetBrandNameAsync(string id)
    {
        Result<CatalogBrand> response; 
        
        if (int.TryParse(id, out var idInt))
            response = await _dbRepository.GetByLegacyIdAsync(idInt);
        else
            response = await _dbRepository.GetByIdAsync(id);

        return response.IsFailed ? string.Empty : response.Value.Name;
    }

    public async Task<CatalogBrand?> CreateAsync(CreateCatalogBrandDto dto)
    {
        var response = await _dbRepository.CreateAsync(new CatalogBrand()
        {
            Id = dto.Id,
            Name = dto.Name
        });
        
        await _cacheService.FlushCacheAsync("cache:/api/catalog-brands");

        return response.IsFailed ? null : response.Value;
    }

    public async Task<CatalogBrand?> GetAsync(string id)
    {
        Result<CatalogBrand> response; 
        
        if (int.TryParse(id, out var idInt))
            response = await _dbRepository.GetByLegacyIdAsync(idInt);
        else
            response = await _dbRepository.GetByIdAsync(id);

        return response.IsFailed ? null : response.Value;
    }

    public async Task<IEnumerable<CatalogBrand>?> GetAllAsync(int? pageSize, int? pageIndex)
    {
        var response = await _dbRepository.GetAllAsync(pageSize, pageIndex);

        return response.IsFailed ? null : response.ValueOrDefault;
    }

    public async Task<Result<GetCatalogBrandsListDto>> UpdateAsync(UpdateCatalogBrandDto dto)
    {
        if (dto.Id is null && string.IsNullOrEmpty(dto.MongoId))
            return Result.Fail("You need to provide an ID");

        Result<CatalogBrand> existingBrand;

        if (dto.Id is not null)
            existingBrand = await _dbRepository.GetByLegacyIdAsync(dto.Id.Value);
        else
            existingBrand = await _dbRepository.GetByIdAsync(dto.MongoId!);

        if (existingBrand.IsFailed || existingBrand.Value is null)
            return Result.Fail("Brand not found");

        var updatedBrand = new CatalogBrand
        {
            Id = existingBrand.Value.Id,
            Name = dto.Name,
            MongoId = existingBrand.Value.MongoId
        };
        
        var response = await _dbRepository.UpdateAsync(updatedBrand);

        if (response.IsFailed)
            return Result.Fail(response.Errors);
        
        await _cacheService.FlushCacheAsync("cache:/api/catalog-brands");
        
        var deserialized = JsonSerializer.Deserialize<GetCatalogBrandDto>(response.Value);
        
        if (deserialized is null)
            return Result.Fail("Failed to deserialize brand");
        
        return Result.Ok(new GetCatalogBrandsListDto
        {
            CatalogBrands = [deserialized]
        });
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var result = await _dbRepository.DeleteAsync(id);
        
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok();
    }

    public async Task<string> GetNameAsync(string id)
    {
        Result<CatalogBrand> response;

        if (int.TryParse(id, out var idInt))
            response = await _dbRepository.GetByLegacyIdAsync(idInt);
        else 
            response = await _dbRepository.GetByIdAsync(id);

        return response.IsFailed ? string.Empty : response.Value.Name;
    }
}