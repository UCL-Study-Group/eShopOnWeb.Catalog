using Catalog.Common.Dtos;
using Catalog.Common.Dtos.Brand;
using Catalog.Common.Models;
using FluentResults;

namespace Catalog.Application.Interfaces;

public interface IBrandService : ICatalogService<CreateCatalogBrandDto, UpdateCatalogBrandDto, GetCatalogBrandDto>
{
    Task<string> GetNameAsync(string id);
}