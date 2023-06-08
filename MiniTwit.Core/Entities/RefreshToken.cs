using MongoDB.Bson.Serialization.Attributes;

namespace MiniTwit.Core.Entities;

public class RefreshToken
{
    [BsonId]
    public string Token { get; set; } = null!;
    public string JwtId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTime ExpiryTime { get; set; }
    public bool Used { get; set; }
    public bool Invalidated { get; set; }
}