namespace MiniTwit.Security.Authentication.TokenGenerators;

public interface ITokenGenerator
{
    string GenerateAccessToken(string jwtId, string userId, string username, string email);
    (string RefreshToken, DateTime ExpiryTime) GenerateRefreshToken();
    string? GetClaimFromAccessToken(string claimType, string token);
    DateTime? GetExpirationTimeFromAccessToken(string accessToken);
}