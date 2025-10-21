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

    public async Task<CatalogBrand?> CreateBrandAsync(CreateCatalogBrandDto brand)
    {
        var response = await _repository.CreateAsync(new CatalogBrand()
        {
            Id = brand.Id,
            Name = brand.Name
        });

        return response.IsFailed ? null : response.Value;
    }

    public async Task<IEnumerable<CatalogBrand>?> GetBrandsAsync()
    {
        var response = await _repository.GetAllAsync();

        return response.IsFailed ? null : response.ValueOrDefault;
    }

    public async Task<CatalogBrand?> GetBrandAsync(string id)
    {
        Result<CatalogBrand> response; 
        
        if (int.TryParse(id, out var idInt))
            response = await _repository.GetByLegacyIdAsync(idInt);
        else
            response = await _repository.GetByIdAsync(id);

        return response.IsFailed ? null : response.Value;
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

    public async Task<Result> UpdateBrandAsync(UpdateCatalogBrandDto brand)
    {
        if (brand.Id is null && string.IsNullOrEmpty(brand.MongoId))
            return Result.Fail("You need to provide an ID");

        Result<CatalogBrand> existingBrand;

        if (brand.Id is not null)
            existingBrand = await _repository.GetByLegacyIdAsync(brand.Id.Value);
        else
           existingBrand = await _repository.GetByIdAsync(brand.MongoId!);

        if (existingBrand.IsFailed || existingBrand.Value is null)
            return Result.Fail("Brand not found");

        var updatedBrand = new CatalogBrand()
        {
            Id = existingBrand.Value.Id,
            Name = brand.Name,
            MongoId = existingBrand.Value.MongoId
        };
        
        var response = await _repository.UpdateAsync(updatedBrand);
        
        return response.IsFailed ? Result.Fail(response.Errors) : Result.Ok();
    }

    public async Task<Result> DeleteBrandAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return Result.Fail("You much provide an id");

        var response = await _repository.DeleteAsync(id);
        
        return response.IsFailed ? Result.Fail(response.Errors) : Result.Ok();
    }
}