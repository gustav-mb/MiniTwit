namespace MiniTwit.Core.DTOs;

public record TokenDTO
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}