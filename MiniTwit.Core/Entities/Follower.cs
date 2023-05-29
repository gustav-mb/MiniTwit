using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MiniTwit.Core.Entities;

public class Follower
{
    [BsonRepresentation(BsonType.ObjectId)]
    string WhoId { get; set; } = null!;
    [BsonRepresentation(BsonType.ObjectId)]
    string WhomId { get; set; } = null!;
}