namespace MiniTwit.Core.Error;

public record APIError
{
    public int Status { get; init; }
    public string ErrorMsg { get; init; } = null!;
}