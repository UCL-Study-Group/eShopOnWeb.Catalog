namespace Catalog.Common.Dtos;

public class UpdateCatalogBrandDto
{
    public int? Id { get; set; }
    public string? MongoId { get; set; }
    public required string Name { get; set; }
}