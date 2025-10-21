using Catalog.Application.Interfaces;
using Catalog.Common.Dtos;
using Catalog.Common.Models;
using Catalog.Infrastructure.Repositories.Interfaces;
using FluentResults;

namespace Catalog.Application.Services;

public class BrandService : IBrandService
{
    private readonly IRepository<CatalogBrand> _repository;

    public BrandService(IRepository<CatalogBrand> repository)
    {
        _repository = repository;
    }

    public async Task<string> GetBrandNameAsync(string id)
    {
        Result<CatalogBrand> response; 
        
        if (int.TryParse(id, out var idInt))
            response = await _repository.GetByLegacyIdAsync(idInt);
        else
            response = await _repository.GetByIdAsync(id);

        return response.IsFailed ? string.Empty : response.Value.Name;
    }

    public async Task<CatalogBrand?> CreateAsync(CreateCatalogBrandDto dto)
    {
        var response = await _repository.CreateAsync(new CatalogBrand()
        {
            Id = dto.Id,
            Name = dto.Name
        });

        return response.IsFailed ? null : response.Value;
    }

    public async Task<CatalogBrand?> GetAsync(string id)
    {
        Result<CatalogBrand> response; 
        
        if (int.TryParse(id, out var idInt))
            response = await _repository.GetByLegacyIdAsync(idInt);
        else
            response = await _repository.GetByIdAsync(id);

        return response.IsFailed ? null : response.Value;
    }

    public async Task<IEnumerable<CatalogBrand>?> GetAllAsync()
    {
        var response = await _repository.GetAllAsync();

        return response.IsFailed ? null : response.ValueOrDefault;
    }

    public async Task<Result> UpdateAsync(UpdateCatalogBrandDto dto)
    {
        if (dto.Id is null && string.IsNullOrEmpty(dto.MongoId))
            return Result.Fail("You need to provide an ID");

        Result<CatalogBrand> existingBrand;

        if (dto.Id is not null)
            existingBrand = await _repository.GetByLegacyIdAsync(dto.Id.Value);
        else
            existingBrand = await _repository.GetByIdAsync(dto.MongoId!);

        if (existingBrand.IsFailed || existingBrand.Value is null)
            return Result.Fail("Brand not found");

        var updatedBrand = new CatalogBrand()
        {
            Id = existingBrand.Value.Id,
            Name = dto.Name,
            MongoId = existingBrand.Value.MongoId
        };
        
        var response = await _repository.UpdateAsync(updatedBrand);
        
        return response.IsFailed ? Result.Fail(response.Errors) : Result.Ok();
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var result = await _repository.DeleteAsync(id);
        
        return result.IsFailed ? Result.Fail(result.Errors) : Result.Ok();
    }

    public async Task<string> GetNameAsync(string id)
    {
        Result<CatalogBrand> response;

        if (int.TryParse(id, out var idInt))
            response = await _repository.GetByLegacyIdAsync(idInt);
        else 
            response = await _repository.GetByIdAsync(id);

        return response.IsFailed ? string.Empty : response.Value.Name;
    }
}