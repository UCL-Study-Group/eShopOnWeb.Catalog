using System.Text.Json.Serialization;

namespace Catalog.Common.Dtos;

public class UpdateCatalogItemDto
{
    public int? Id { get; init; }
    public string? MongoId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public decimal? Price { get; init; }
    public string? ImageUrl { get; init; }
    public string? CatalogType { get; init; }
    public string? CatalogBrand { get; init; }
}