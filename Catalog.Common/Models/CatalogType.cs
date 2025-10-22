using Catalog.Common.Models.Base;

namespace Catalog.Common.Models;

public class CatalogType : BaseModel
{
    public required string Name { get; init; }
}