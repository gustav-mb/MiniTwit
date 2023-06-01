using System.Linq.Expressions;
using MongoDB.Driver;

namespace MiniTwit.Core.MongoDB.Builders;

public class EntityTypeBuilder<T> where T : class
{
    private readonly IMongoDatabase _database;
    private readonly List<Action<IMongoCollection<T>>> _createIndexActions = new();
    public IMongoCollection<T> Collection { get; private set; } = null!;

    public EntityTypeBuilder(IMongoDatabase database)
    {
        _database = database;
    }

    public IndexBuilder<T> HasIndex(Expression<Func<T, object>> getCollectionActions)
    {
        var indexBuilder = new IndexBuilder<T>(getCollectionActions);
        _createIndexActions.Add((collection) => collection.Indexes.CreateOne(new CreateIndexModel<T>(indexBuilder.Index)));

        return indexBuilder;
    }

    internal void ConfigureIndexes()
    {
        foreach (var createIndexAction in _createIndexActions)
        {
            createIndexAction.Invoke(Collection);
        }
    }
}