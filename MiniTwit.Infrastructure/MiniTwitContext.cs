using MiniTwit.Core;
using MiniTwit.Core.Entities;
using MiniTwit.Core.MongoDB;
using MongoDB.Driver;

namespace MiniTwit.Infrastructure;

public class MiniTwitContext : MongoContext, IMiniTwitContext
{
    public IMongoCollection<User> Users { get; set; } = null!;
    public IMongoCollection<Follower> Followers { get; set; } = null!;
    public IMongoCollection<Tweet> Tweets { get; set; } = null!;

    public MiniTwitContext(IMongoContextOptionsBuilder optionsBuilder) : base(optionsBuilder) { }
}