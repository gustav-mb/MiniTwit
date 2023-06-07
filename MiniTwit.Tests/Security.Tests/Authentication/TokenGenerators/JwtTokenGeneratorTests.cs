using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MiniTwit.Security.Authentication;
using MiniTwit.Security.Authentication.TokenGenerators;

namespace MiniTwit.Tests.Security.Tests.Authentication.TokenGenerators;

public class JwtTokenGeneratorTests
{
    private readonly JwtSettings _settings;
    private readonly JwtTokenGenerator _tokenGenerator;

    public JwtTokenGeneratorTests()
    {
        IdentityModelEventSource.ShowPII = true;
        _settings = new JwtSettings
        {
            Issuer = "test",
            Audience = "test",
            Key = "bdbdf608-b1a3-455b-a5ce-e0e0dfa68ebb",
            TokenExpiryMin = 5,
            RefreshTokenExpiryMin = 1,
        };

        _tokenGenerator = new JwtTokenGenerator(Options.Create<JwtSettings>(_settings));
    }

    [Fact]
    public void GenerateAccessToken_generates_access_token_with_provided_claims()
    {
        // Arrange
        var parameters = new TokenValidationParameters()
        {
            RequireExpirationTime = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key)),
            ClockSkew = TimeSpan.Zero
        };
        var tokenHandler = new JwtSecurityTokenHandler();

        // Act
        var accessToken = _tokenGenerator.GenerateAccessToken("0", "000000000000000000000001", "Gustav", "test@test.com");
        var actual = tokenHandler.ValidateToken(accessToken, parameters, out SecurityToken _);

        // Assert
        Assert.Equal("0", actual.FindFirstValue(JwtRegisteredClaimNames.Jti));
        Assert.Equal("000000000000000000000001", actual.FindFirstValue(ClaimTypes.NameIdentifier));
        Assert.Equal("Gustav", actual.FindFirstValue(ClaimTypes.Name));
        Assert.Equal("test@test.com", actual.FindFirstValue(ClaimTypes.Email));
    }

    [Fact]
    public void GenerateRefreshToken_generates_refresh_token_with_ExpiryTime_of_JwtSettings_RefreshTokenExpiryMin()
    {
        // Arrange
        var expected = DateTime.UtcNow.AddMinutes(_settings.RefreshTokenExpiryMin);

        // Act
        var actual = _tokenGenerator.GenerateRefreshToken();

        // Assert
        actual.ExpiryTime.Should().BeCloseTo(expected, 1.Milliseconds());
    }

    [Fact]
    public void GetExpirationTimeFromAccessToken_given_no_encoded_expiration_time_returns_null()
    {
        // Act
        var actual = _tokenGenerator.GetExpirationTimeFromAccessToken("corrupted_token");

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public void GetExpirationTimeFromAccessToken_given_encoded_expiration_time_returns_expiration_time()
    {
        // Arrange
        var expected = DateTime.UtcNow.AddMinutes(_settings.TokenExpiryMin);
        var accessToken = CreateAccessToken("0", "000000000000000000000001", "Gustav", "test@test.com");

        // Act
        var actual = _tokenGenerator.GetExpirationTimeFromAccessToken(accessToken);

        // Assert
        actual.Should().BeCloseTo(expected, 5.Seconds());
    }

    [Fact]
    public void GetClaimFromAccessToken_given_invalid_claim_returns_null()
    {
        // Arrange
        var expected = DateTime.UtcNow.AddMinutes(_settings.TokenExpiryMin);
        var accessToken = CreateAccessToken("0", "000000000000000000000001", "Gustav", "test@test.com");

        // Act
        var actual = _tokenGenerator.GetClaimFromAccessToken(ClaimTypes.GivenName, accessToken);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public void GetClaimFromAccessToken_given_corrupted_access_token_returns_null()
    {
        // Act
        var actual = _tokenGenerator.GetClaimFromAccessToken(ClaimTypes.GivenName, "corrupted");

        // Assert
        Assert.Null(actual);
    }

    [Theory]
    [InlineData("0", JwtRegisteredClaimNames.Jti)]
    [InlineData("000000000000000000000001", ClaimTypes.NameIdentifier)]
    [InlineData("Gustav", ClaimTypes.Name)]
    [InlineData("test@test.com", ClaimTypes.Email)]
    public void GetClaimFromAccessToken_given_valid_claim_returns_value(string expected, string claimType)
    {
        // Arrange
        var accessToken = CreateAccessToken("0", "000000000000000000000001", "Gustav", "test@test.com");

        // Act
        var actual = _tokenGenerator.GetClaimFromAccessToken(claimType, accessToken);

        // Assert
        Assert.Equal(expected, actual);
    }

    private string CreateAccessToken(string jwtId, string userId, string username, string email)
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
}