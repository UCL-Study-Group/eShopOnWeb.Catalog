using Catalog.Common.Dtos;
using Catalog.Common.Models;
using FluentResults;

namespace Catalog.Application.Interfaces;

public interface IItemService : ICatalogService<CatalogItem, CreateCatalogItemDto, UpdateCatalogItemDto, GetCatalogItemDto>
{
}