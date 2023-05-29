using MiniTwit.Core.Entities;
using MongoDB.Driver;

namespace MiniTwit.Core;

public interface IMiniTwitContext
{
    IMongoCollection<User> Users { get; init; }
    IMongoCollection<Follower> Followers { get; init; }
    IMongoCollection<Tweet> Tweets { get; init; }
}