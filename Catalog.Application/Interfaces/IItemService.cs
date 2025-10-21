using Catalog.Common.Dtos;
using Catalog.Common.Models;
using FluentResults;

namespace Catalog.Application.Interfaces;

public interface IItemService
{
    Task<IEnumerable<GetCatalogItemDto>?> GetAllAsync();
    Task<GetCatalogItemDto?> GetByIdAsync(string id);
    Task<CatalogItem?> CreateAsync(CreateCatalogItemDto catalogItem);
    Task<Result> UpdateAsync(UpdateCatalogItemDto catalogItem);
    Task<Result> DeleteAsync(string id);
}