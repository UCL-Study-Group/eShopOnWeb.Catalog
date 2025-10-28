using Catalog.Common.Dtos.Item;
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

public class CatalogTypeResponse
{
    public required GetCatalogTypeDto CatalogTypes { get; set; }
}

public class CatalogTypeListResponse
{
    public IEnumerable<GetCatalogTypeDto> CatalogTypes { get; set; } = [];
}