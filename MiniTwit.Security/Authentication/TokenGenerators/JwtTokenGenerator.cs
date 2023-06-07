using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MiniTwit.Security.Authentication.TokenGenerators;

public class JwtTokenGenerator : ITokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public string GenerateAccessToken(string jwtId, string userId, string username, string email)
    {
        var key = Encoding.UTF8.GetBytes(_settings.Key);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

        var subject = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, jwtId),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email),
        });

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = signingCredentials,
            Expires = DateTime.UtcNow.AddMinutes(_settings.TokenExpiryMin),
            NotBefore = DateTime.UtcNow,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public (string RefreshToken, DateTime ExpiryTime) GenerateRefreshToken()
    {
        var buffer = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(buffer);
        }

        return (Convert.ToBase64String(buffer), DateTime.UtcNow.AddMinutes(_settings.RefreshTokenExpiryMin));
    }

    public DateTime? GetExpirationTimeFromAccessToken(string accessToken)
    {
        var expirationTime = GetClaimFromAccessToken(JwtRegisteredClaimNames.Exp, accessToken);

        if (expirationTime == null)
        {
            return null;
        }

        return DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationTime)).UtcDateTime;
    }

    public string? GetClaimFromAccessToken(string claimType, string token)
    {
        var key = Encoding.UTF8.GetBytes(_settings.Key);

        var parameters = new TokenValidationParameters()
        {
            RequireExpirationTime = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, parameters, out SecurityToken securityToken);
            JwtSecurityToken? jwtToken = securityToken as JwtSecurityToken;

            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            string? value = principal.FindFirstValue(claimType);

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return value;
        }
        catch
        {
            return null;
        }
    }
}