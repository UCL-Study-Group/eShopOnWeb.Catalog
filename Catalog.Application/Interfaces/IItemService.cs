using Catalog.Common.Dtos;
using Catalog.Common.Dtos.Item;
using Catalog.Common.Models;
using FluentResults;

namespace Catalog.Application.Interfaces;

public interface IItemService : ICatalogService<CreateCatalogItemDto, UpdateCatalogItemDto, GetCatalogItemDto>
{
    Task<Result<IEnumerable<GetCatalogItemDto>>> GetAllAsync(int? pageSize = null, int? pageIndex = null, string? brandId = null, string? typeId = null);
}