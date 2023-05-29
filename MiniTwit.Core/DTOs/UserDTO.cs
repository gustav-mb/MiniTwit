using System.ComponentModel.DataAnnotations;

namespace MiniTwit.Core.DTOs;

public record UserDTO
{
    public string Id { get; init; } = null!;
    public string Username { get; init; } = null!;
    [EmailAddress]
    public string Email { get; init; } = null!;
}

public record UserCreateDTO
{
    public string Username { get; init; } = null!;
    [EmailAddress]
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}