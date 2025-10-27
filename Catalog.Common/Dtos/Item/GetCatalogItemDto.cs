namespace Catalog.Common.Dtos.Item;

public class GetCatalogItemDto
{
    public int? Id { get; set; }
    public required string MongoId { get; set; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public string? PictureUri { get; init; }
    public required int CatalogTypeId { get; init; }
    public required int CatalogBrandId { get; init; }
}

public class GetCatalogItemObjectDto
{
    public required GetCatalogItemDto CatalogItems { get; set; }
}