using Catalog.Common.Models;

namespace Catalog.Common.Dtos.Brand;

public class GetCatalogBrandDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}

public class CatalogBrandResponse
{
    public required GetCatalogBrandDto CatalogBrand { get; set; }
}

public class CatalogBrandListResponse
{
    public IEnumerable<GetCatalogBrandDto> CatalogBrands { get; set; } = [];
}