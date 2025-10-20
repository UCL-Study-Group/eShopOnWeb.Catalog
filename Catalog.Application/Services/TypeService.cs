using Catalog.Common.Dtos;
using Catalog.Common.Models;
using Catalog.Infrastructure.Repositories.Interfaces;

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
            Type = type.Name
        });
        
        return response.IsFailed ? null : response.Value;
    }

    public async Task<IEnumerable<CatalogType>?> GetTypesAsync()
    {
        var response = await _repository.GetAllAsync();
        
        return response.IsFailed ? [] : response.Value;
    }

    public async Task<CatalogType?> GetTypeAsync(int id)
    {
        var response = await _repository.GetByIdAsync(id);
        
        return response.IsFailed ? null : response.Value;
    }

    public async Task<string> GetTypeNameAsync(int id)
    {
        var response = await _repository.GetByIdAsync(id);

        return response.IsFailed ? string.Empty : response.Value.Type;
    }
}