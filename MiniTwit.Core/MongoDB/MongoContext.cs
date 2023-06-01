using MiniTwit.Core.MongoDB.Builders;
using MongoDB.Driver;

namespace MiniTwit.Core.MongoDB;

public class MongoContext
{
    private readonly IMongoContextOptionsBuilder _optionsBuilder;
    public IMongoDatabase Database => _optionsBuilder.Database;

    protected MongoContext(IMongoContextOptionsBuilder optionsBuilder)
    {
        _optionsBuilder = optionsBuilder;
        optionsBuilder.Configure(this, (_) => this.OnConfiguring(_optionsBuilder));
    }

    protected virtual void OnConfiguring(IMongoContextOptionsBuilder builder) { }
}