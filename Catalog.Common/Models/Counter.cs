using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Common.Models;

public class Counter
{
    [BsonId]
    public required string Id { get; set; }
    
    public int Sequence { get; set; }
}