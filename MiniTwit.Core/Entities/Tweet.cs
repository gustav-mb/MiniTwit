using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MiniTwit.Core.Entities;

public class Tweet
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonRepresentation(BsonType.ObjectId)]
    public string AuthorId { get; set; } = null!;
    public string Text { get; set; } = null!;
    public DateTime PubDate { get; set; }
    public bool Flagged { get; set; }
}