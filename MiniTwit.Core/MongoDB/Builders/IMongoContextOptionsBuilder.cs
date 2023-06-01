using MongoDB.Driver;

namespace MiniTwit.Core.MongoDB.Builders;

public interface IMongoContextOptionsBuilder
{
    IMongoDatabase Database { get; }
    void Configure(MongoContext context, Action<IMongoContextOptionsBuilder> onConfiguring);
    EntityTypeBuilder<TEntity> Entity<TEntity>(Action<EntityTypeBuilder<TEntity>> buildAction) where TEntity : class;
}