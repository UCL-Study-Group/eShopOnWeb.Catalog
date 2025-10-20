using Catalog.Common.Models.Base;

namespace Catalog.Common.Models;

public class CatalogBrand : BaseModel
{
    public required string Brand { get; init; }
}