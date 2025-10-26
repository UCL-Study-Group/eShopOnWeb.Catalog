namespace Catalog.Common.Dtos.Item;

public class UpdateCatalogItemDto
{
    public int? Id { get; init; }
    public string? MongoId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public decimal? Price { get; init; }
    public string? PictureUri { get; init; }
    public int? CatalogTypeId { get; init; }
    public int? CatalogBrandId { get; init; }
}