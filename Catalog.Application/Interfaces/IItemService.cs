using Catalog.Common.Dtos;
using Catalog.Common.Models;
using FluentResults;

namespace Catalog.Application.Interfaces;

public interface IItemService : ICatalogService<CatalogItem, CreateCatalogItemDto, UpdateCatalogItemDto, GetCatalogItemDto>
{
    Task<IEnumerable<GetCatalogItemDto>?> GetAllAsync(int? pageSize = null, int? pageIndex = null, string? brandId = null, string? typeId = null);
}