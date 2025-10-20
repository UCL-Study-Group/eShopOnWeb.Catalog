using Catalog.Common.Dtos;
using Catalog.Common.Models;
using Catalog.Infrastructure.Repositories.Interfaces;
using FluentResults;

namespace Catalog.Application.Services;

public class BrandService
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
            Brand = brand.Name
        });
        
        return response.IsFailed ? null : response.Value;
    }

    public async Task<IEnumerable<CatalogBrand>?> GetBrandsAsync()
    {
        var response = await _repository.GetAllAsync();

        return response.IsFailed ? null : response.ValueOrDefault;
    }

    public async Task<CatalogBrand?> GetBrandAsync(int id)
    {
        var response = await _repository.GetByIdAsync(id);
        
        return response.IsFailed ? null : response.Value;
    }

    public async Task<string> GetBrandNameAsync(int id)
    {
        var response = await _repository.GetByIdAsync(id);

        return response.IsFailed ? string.Empty : response.Value.Brand;
    }
}