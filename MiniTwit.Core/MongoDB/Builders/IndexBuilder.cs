using System.Linq.Expressions;
using MongoDB.Driver;

namespace MiniTwit.Core.MongoDB.Builders;

public class IndexBuilder<T> where T : class
{
    private readonly Expression<Func<T, object>> _indexTarget;
    public IndexKeysDefinition<T> Index { get; private set; } = null!;

    public IndexBuilder(Expression<Func<T, object>> indexTargetAction)
    {
        _indexTarget = indexTargetAction;
    }

    public void Ascending()
    {
        Index = Builders<T>.IndexKeys.Ascending(_indexTarget);
    }

    public void Descending()
    {
        Index = Builders<T>.IndexKeys.Descending(_indexTarget);
    }
}