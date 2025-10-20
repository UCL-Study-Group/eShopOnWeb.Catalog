namespace Catalog.Common.Dtos;

public class GetCatalogItemDto
{
    public int? Id { get; set; }
    public required string MongoId { get; set; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required string ImageUrl { get; init; }
    public required string CatalogType { get; init; }
    public required string CatalogBrand { get; init; }
}