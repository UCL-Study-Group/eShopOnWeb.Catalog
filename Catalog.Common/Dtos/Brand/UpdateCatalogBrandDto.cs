namespace Catalog.Common.Dtos.Brand;

public class UpdateCatalogBrandDto
{
    public int? Id { get; set; }
    public string? MongoId { get; set; }
    public required string Name { get; set; }
}