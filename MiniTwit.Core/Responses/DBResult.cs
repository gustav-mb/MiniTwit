using Mapster;

namespace MiniTwit.Core.Responses;

public record DBResult
{
    public string? DBError { get; init; }
}

public record DBResult<T> : DBResult
{
    public T? Model { get; init; }

    public TTarget ConvertModelTo<TTarget>()
    {
        if (Model == null)
        {
            throw new InvalidOperationException($"Cannot convert {null} to {typeof(TTarget)}!");
        }

        return Model.Adapt<TTarget>();
    }
}