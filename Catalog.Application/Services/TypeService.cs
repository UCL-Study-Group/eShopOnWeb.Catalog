using Catalog.Common.Dtos;
using Catalog.Common.Models;
using Catalog.Infrastructure.Repositories.Interfaces;
using FluentResults;

namespace Catalog.Application.Services;

public class TypeService
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
            Type = type.Name
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

        return response.IsFailed ? string.Empty : response.Value.Type;
    }
}