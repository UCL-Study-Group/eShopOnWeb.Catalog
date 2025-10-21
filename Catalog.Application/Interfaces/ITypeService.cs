using Catalog.Common.Dtos;
using Catalog.Common.Models;
using FluentResults;

namespace Catalog.Application.Interfaces;

public interface ITypeService
{
    Task<CatalogType?> CreateTypeAsync(CreateCatalogTypeDto type);
    Task<IEnumerable<CatalogType>?> GetTypesAsync();
    Task<CatalogType?> GetTypeAsync(string id);
    Task<string> GetTypeNameAsync(string id);
    Task<Result> UpdateBrandAsync(UpdateCatalogTypeDto type);
    Task<Result> DeleteBrandAsync(string id);
}