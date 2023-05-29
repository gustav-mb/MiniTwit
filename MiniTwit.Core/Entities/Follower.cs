using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MiniTwit.Core.Entities;

public class Follower
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string WhoId { get; set; } = null!;
    [BsonRepresentation(BsonType.ObjectId)]
    public string WhomId { get; set; } = null!;
}