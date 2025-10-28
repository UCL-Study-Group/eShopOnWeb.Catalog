using Catalog.Common.Dtos;
using Catalog.Common.Dtos.Type;
using Catalog.Common.Models;
using FluentResults;

namespace Catalog.Application.Interfaces;

public interface ITypeService : ICatalogService<CreateCatalogTypeDto, UpdateCatalogTypeDto, GetCatalogTypeDto>
{
    Task<string> GetNameAsync(string id);
}