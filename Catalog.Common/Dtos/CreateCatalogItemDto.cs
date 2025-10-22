namespace Catalog.Common.Dtos;

public class CreateCatalogItemDto
{
    public int? Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required string ImageUrl { get; init; }
    public required string CatalogType { get; init; }
    public required string CatalogBrand { get; init; }
}