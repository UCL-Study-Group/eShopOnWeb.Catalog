using Catalog.Common.Dtos;
using Catalog.Common.Models;
using FluentResults;

namespace Catalog.Application.Interfaces;

public interface ITypeService : ICatalogService<CatalogType, CreateCatalogTypeDto, UpdateCatalogTypeDto, CatalogType>
{
    Task<string> GetNameAsync(string id);
}