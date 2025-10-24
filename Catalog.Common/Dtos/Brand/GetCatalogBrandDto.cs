using Catalog.Common.Models;

namespace Catalog.Common.Dtos.Brand;

public class GetCatalogBrandDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    public static GetCatalogBrandDto FromModel(CatalogBrand model)
    {
        return new GetCatalogBrandDto
        {
            Id = model.Id!.Value,
            Name = model.Name,
        };
    }
}