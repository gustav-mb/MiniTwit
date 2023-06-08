using MiniTwit.Core.Entities;
using MongoDB.Driver;

namespace MiniTwit.Core;

public interface IMiniTwitContext
{
    IMongoCollection<User> Users { get; }
    IMongoCollection<Follower> Followers { get; }
    IMongoCollection<Tweet> Tweets { get; }
    IMongoCollection<RefreshToken> RefreshTokens { get; }
}