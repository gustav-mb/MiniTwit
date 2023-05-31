using MongoDB.Driver;

namespace MiniTwit.Core.MongoDB;

public class MongoContext
{
    protected readonly IMongoContextOptionsBuilder OptionsBuilder;
    public IMongoDatabase Database => OptionsBuilder.Database;

    protected MongoContext(IMongoContextOptionsBuilder optionsBuilder)
    {
        OptionsBuilder = optionsBuilder;
        optionsBuilder.Configure(this, this.OnConfiguring);
    }

    protected virtual void OnConfiguring() { }
}