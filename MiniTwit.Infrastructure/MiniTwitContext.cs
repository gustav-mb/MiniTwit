using Microsoft.Extensions.Options;
using MiniTwit.Core.Data;
using MiniTwit.Core.Entities;
using MongoDB.Driver;

namespace MiniTwit.Infrastructure;

public class MiniTwitContext : IMiniTwitContext
{
    public IMongoCollection<User> Users { get; init; }
    public IMongoCollection<Follower> Followers { get; init; }
    public IMongoCollection<Tweet> Tweets { get; init; }

    public MiniTwitContext(IOptions<MiniTwitDatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

        Users = mongoDatabase.GetCollection<User>(databaseSettings.Value.UsersCollectionName);
        Followers = mongoDatabase.GetCollection<Follower>(databaseSettings.Value.FollowersCollectionName);
        Tweets = mongoDatabase.GetCollection<Tweet>(databaseSettings.Value.TweetsCollectionName);
    }
}