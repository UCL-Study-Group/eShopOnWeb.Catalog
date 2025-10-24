using Catalog.Common.Models;

namespace Catalog.Common.Dtos.Type;

public class GetCatalogTypeDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public static GetCatalogTypeDto FromModel(CatalogType model)
    {
        return new GetCatalogTypeDto
        {
            Id = model.Id!.Value,
            Name = model.Name,
        };
    }
}