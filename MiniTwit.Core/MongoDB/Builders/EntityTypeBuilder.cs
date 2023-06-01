using System.Linq.Expressions;
using MongoDB.Driver;

namespace MiniTwit.Core.MongoDB.Builders;

public class EntityTypeBuilder
{
    protected readonly IMongoDatabase Database;

    public EntityTypeBuilder(IMongoDatabase database)
    {
        Database = database;
    }

    protected internal virtual void ConfigureIndexes() { }
}

public class EntityTypeBuilder<T> : EntityTypeBuilder where T : class
{
    private readonly List<Action<IMongoCollection<T>>> _createIndexActions = new();
    public IMongoCollection<T> Collection { get; private set; } = null!;

    public EntityTypeBuilder(IMongoDatabase database) : base(database) { }

    public IndexBuilder<T> HasIndex(Expression<Func<T, object>> getCollectionActions)
    {
        var indexBuilder = new IndexBuilder<T>(getCollectionActions);
        _createIndexActions.Add((collection) => collection.Indexes.CreateOne(new CreateIndexModel<T>(indexBuilder.Index)));

        return indexBuilder;
    }

    public void ToCollection(string collectionName, MongoCollectionSettings? settings = null)
    {
        Collection = Database.GetCollection<T>(collectionName, settings);
    }

    protected internal override void ConfigureIndexes()
    {
        foreach (var createIndexAction in _createIndexActions)
        {
            createIndexAction.Invoke(Collection);
        }
    }
}