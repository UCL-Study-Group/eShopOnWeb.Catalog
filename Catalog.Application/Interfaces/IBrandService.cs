using Catalog.Common.Dtos;
using Catalog.Common.Models;
using FluentResults;

namespace Catalog.Application.Interfaces;

public interface IBrandService
{
    Task<CatalogBrand?> CreateBrandAsync(CreateCatalogBrandDto brand);
    Task<IEnumerable<CatalogBrand>?> GetBrandsAsync();
    Task<CatalogBrand?> GetBrandAsync(string id);
    Task<string> GetBrandNameAsync(string id);
    Task<Result> UpdateBrandAsync(UpdateCatalogBrandDto brand);
    Task<Result> DeleteBrandAsync(string id);
}