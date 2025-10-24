namespace Catalog.Common.Dtos.Type;

public class UpdateCatalogTypeDto
{
    public int? Id { get; set; }
    public string? MongoId { get; set; }
    public required string Name { get; set; }
}