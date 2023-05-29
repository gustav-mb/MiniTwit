using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MiniTwit.Core.Entities;

public class User
{
    [BsonId]
    [BsonElement("id")]
    [BsonRepresentation(BsonType.ObjectId)]
    string Id { get; set; } = null!;
    string Username { get; set; } = null!;
    string Email { get; set; } = null!;
    string Password { get; set; } = null!;
    string Salt { get; set; } = null!;
}