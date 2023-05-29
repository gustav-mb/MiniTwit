namespace MiniTwit.Core.Entities;

public class User
{
    string Id { get; set; } = null!;
    string Username { get; set; } = null!;
    string Email { get; set; } = null!;
    string Password { get; set; } = null!;
    string Salt { get; set; } = null!;
}