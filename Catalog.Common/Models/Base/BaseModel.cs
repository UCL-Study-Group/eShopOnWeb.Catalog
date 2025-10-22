using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Common.Models.Base;

public abstract class BaseModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string MongoId { get; set; } = ObjectId.GenerateNewId().ToString();
    
    [BsonIgnoreIfNull]
    public int? Id { get; set; }
}