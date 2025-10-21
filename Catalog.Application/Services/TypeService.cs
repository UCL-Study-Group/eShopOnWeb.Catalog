using Catalog.Application.Interfaces;
using Catalog.Common.Dtos;
using Catalog.Common.Models;
using Catalog.Infrastructure.Repositories.Interfaces;
using FluentResults;

namespace Catalog.Application.Services;

public class TypeService : ITypeService
{
    private readonly IRepository<CatalogType> _repository;

    public TypeService(IRepository<CatalogType> repository)
    {
        _repository = repository;
    }
    
    public async Task<CatalogType?> CreateTypeAsync(CreateCatalogTypeDto type)
    {
        var response = await _repository.CreateAsync(new CatalogType()
        {
            Id = type.Id,
            Name = type.Name
        });
        
        return response.IsFailed ? null : response.Value;
    }

    public async Task<IEnumerable<CatalogType>?> GetTypesAsync()
    {
        var response = await _repository.GetAllAsync();
        
        return response.IsFailed ? [] : response.Value;
    }

    public async Task<CatalogType?> GetTypeAsync(string id)
    {
        Result<CatalogType> response;

        if (int.TryParse(id, out var idInt))
            response = await _repository.GetByLegacyIdAsync(idInt);
        else 
            response = await _repository.GetByIdAsync(id);

        return response.IsFailed ? null : response.Value;
    }

    public async Task<string> GetTypeNameAsync(string id)
    {
        Result<CatalogType> response;

        if (int.TryParse(id, out var idInt))
            response = await _repository.GetByLegacyIdAsync(idInt);
        else 
            response = await _repository.GetByIdAsync(id);

        return response.IsFailed ? string.Empty : response.Value.Name;
    }
    
    public async Task<Result> UpdateBrandAsync(UpdateCatalogTypeDto type)
    {
        if (type.Id is null && string.IsNullOrEmpty(type.MongoId))
            return Result.Fail("You need to provide an ID");

        Result<CatalogType> existingBrand;

        if (type.Id is not null)
            existingBrand = await _repository.GetByLegacyIdAsync(type.Id.Value);
        else
            existingBrand = await _repository.GetByIdAsync(type.MongoId!);

        if (existingBrand.IsFailed || existingBrand.Value is null)
            return Result.Fail("Type not found");

        var updatedBrand = new CatalogType()
        {
            Id = existingBrand.Value.Id,
            Name = type.Name,
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