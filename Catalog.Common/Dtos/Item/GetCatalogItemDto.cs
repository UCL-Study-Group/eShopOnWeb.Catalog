namespace Catalog.Common.Dtos.Item;

public class GetCatalogItemDto
{
    public int? Id { get; set; }
    public required string MongoId { get; set; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required string ImageUrl { get; init; }
    public required string CatalogTypeId { get; init; }
    public required string CatalogBrandId { get; init; }
}

public class GetCatalogItemsListDto
{
    public required IEnumerable<GetCatalogItemDto> CatalogItems { get; set; }
}