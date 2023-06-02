namespace MiniTwit.Security.TokenGeneration;

public interface ITokenGenerator
{
    string GenerateToken(string userId, string username, string email);
}