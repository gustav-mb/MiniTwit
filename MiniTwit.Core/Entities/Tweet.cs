using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MiniTwit.Core.Entities;

public class Tweet
{
    [BsonId]
    [BsonElement("id")]
    [BsonRepresentation(BsonType.ObjectId)]
    string Id { get; set; } = null!;
    [BsonRepresentation(BsonType.ObjectId)]
    string AuthorId { get; set; } = null!;
    string Text { get; set; } = null!;
    DateTime PubDate { get; set; }
    bool Flagged { get; set; }
}