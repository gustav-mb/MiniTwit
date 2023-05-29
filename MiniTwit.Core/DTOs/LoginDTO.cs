namespace MiniTwit.Core.DTOs;

public record LoginDTO
{
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
}