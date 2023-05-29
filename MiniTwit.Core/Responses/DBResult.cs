using Mapster;
using MiniTwit.Core.Error;

namespace MiniTwit.Core.Responses;

public record DBResult
{
    public DBError? DBError { get; init; }
}

public record DBResult<T> : DBResult
{
    public T? Model { get; init; }

    public TTarget ConvertModelTO<TTarget>()
    {
        if (Model == null)
        {
            throw new InvalidOperationException($"Cannot convert {null} to {typeof(TTarget)}!");
        }

        return Model.Adapt<TTarget>();
    }
}