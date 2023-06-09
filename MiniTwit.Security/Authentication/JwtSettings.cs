namespace MiniTwit.Security.Authentication;

public class JwtSettings
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Key { get; set; } = null!;
    public int TokenExpiryMin { get; set; }
    public int RefreshTokenExpiryMin { get; set; }
}