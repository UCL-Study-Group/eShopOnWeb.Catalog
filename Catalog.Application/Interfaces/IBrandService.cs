using Catalog.Common.Dtos;
using Catalog.Common.Dtos.Brand;
using Catalog.Common.Models;
using FluentResults;

namespace Catalog.Application.Interfaces;

public interface IBrandService : ICatalogService<CatalogBrand, CreateCatalogBrandDto, UpdateCatalogBrandDto, CatalogBrand>
{
    Task<string> GetNameAsync(string id);
}