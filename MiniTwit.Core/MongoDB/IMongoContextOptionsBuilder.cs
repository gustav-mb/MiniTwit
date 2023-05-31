using MongoDB.Driver;

namespace MiniTwit.Core.MongoDB;

public interface IMongoContextOptionsBuilder
{
    IMongoDatabase Database { get; }
    void Configure(MongoContext context, Action onConfiguring);
}