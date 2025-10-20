using Catalog.Common.Models.Base;

namespace Catalog.Common.Models;

public class CatalogItem : BaseModel
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required string ImageUrl { get; init; }
    public required int CatalogTypeId { get; init; }
    public required int CatalogBrandId { get; init; }
}