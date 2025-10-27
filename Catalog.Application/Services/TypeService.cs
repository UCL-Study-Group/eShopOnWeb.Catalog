using System.Text.Json;
using Catalog.Application.Interfaces;
using Catalog.Common.Dtos;
using Catalog.Common.Dtos.Type;
using Catalog.Common.Models;
using Catalog.Infrastructure.Repositories.Interfaces;
using FluentResults;
using Mapster;

namespace Catalog.Application.Services;

public class TypeService : ITypeService
{
    private readonly IDbRepository<CatalogType, GetCatalogTypeDto> _dbRepository;
    private readonly ICacheService _cacheService;

    public TypeService(IDbRepository<CatalogType, GetCatalogTypeDto> dbRepository, ICacheService cacheService)
    {
        _dbRepository = dbRepository;
        _cacheService = cacheService;
    }
    
    public async Task<Result<GetCatalogTypeDto>> CreateAsync(CreateCatalogTypeDto type)
    {
        var response = await _dbRepository.CreateAsync(new CatalogType()
        {
            Id = type.Id,
            Name = type.Name
        });
        
        await _cacheService.FlushCacheAsync("cache:/api/catalog-types");

        return response;
    }

    public async Task<Result<IEnumerable<GetCatalogTypeDto>>> GetAllAsync(int? pageSize, int? pageIndex)
    {
        var response = await _dbRepository.GetAllAsync(pageSize, pageIndex);

        return response;
    }

    public async Task<Result<GetCatalogTypeDto>> GetAsync(string id)
    {
        Result<GetCatalogTypeDto> response;

        if (int.TryParse(id, out var idInt))
            response = await _dbRepository.GetByLegacyIdAsync(idInt);
        else 
            response = await _dbRepository.GetByIdAsync(id);

        return response;
    }

    public async Task<string> GetNameAsync(string id)
    {
        Result<GetCatalogTypeDto> response;

        if (int.TryParse(id, out var idInt))
            response = await _dbRepository.GetByLegacyIdAsync(idInt);
        else 
            response = await _dbRepository.GetByIdAsync(id);

        return response.IsFailed ? string.Empty : response.Value.Name;
    }
    
    public async Task<Result<GetCatalogTypeDto>> UpdateAsync(UpdateCatalogTypeDto type)
    {
        if (type.Id is null && string.IsNullOrEmpty(type.MongoId))
            return Result.Fail("You need to provide an ID");

        Result<GetCatalogTypeDto> existingBrand;

        if (type.Id is not null)
            existingBrand = await _dbRepository.GetByLegacyIdAsync(type.Id.Value);
        else
            existingBrand = await _dbRepository.GetByIdAsync(type.MongoId!);

        if (existingBrand.IsFailed || existingBrand.Value is null)
            return Result.Fail("Type not found");

        var updatedBrand = new CatalogType()
        {
            Id = existingBrand.Value.Id,
            Name = type.Name,
        };
        
        var response = await _dbRepository.UpdateAsync(updatedBrand);
        
        if (response.IsFailed)
            return Result.Fail(response.Errors);
        
        await _cacheService.FlushCacheAsync("cache:/api/catalog-types");

        return response;
    }

    public async Task<Result> DeleteAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return Result.Fail("You much provide an id");

        var response = await _dbRepository.DeleteAsync(id);
        
        await _cacheService.FlushCacheAsync("cache:/api/catalog-types");
        
        return response.IsFailed ? Result.Fail(response.Errors) : Result.Ok();
    }
}