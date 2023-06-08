using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MiniTwit.Core.DTOs;
using MiniTwit.Core.Error;
using MiniTwit.Security.Authentication;
using MiniTwit.Security.Authentication.TokenGenerators;
using Xunit.Priority;

namespace MiniTwit.Tests.Integration.Tests.Integrations;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
[DefaultPriority(0)]
public class AuthenticationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthenticationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _tokenGenerator = new JwtTokenGenerator(Options.Create<JwtSettings>(_factory.JwtSettings));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Login_given_null_or_empty_username_returns_Unauthorized(string username)
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Username is missing" };

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Authentication/login", new LoginDTO { Username = username, Password = "password" });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Login_given_null_or_empty_password_returns_Unauthorized(string password)
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Password is missing" };

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Authentication/login", new LoginDTO { Username = "Filled", Password = password });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task Login_given_invalid_username_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Invalid username" };

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Authentication/login", new LoginDTO { Username = "Test", Password = "password" });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task Login_given_valid_username_and_invalid_password_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Invalid password" };

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Authentication/login", new LoginDTO { Username = "Gustav", Password = "wrong" });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    [Priority(1)]
    public async Task Login_given_valid_username_and_password_returns_Ok()
    {
        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Authentication/login", new LoginDTO { Username = "Gustav", Password = "password" });
        var content = await actual.Content.ReadFromJsonAsync<TokenDTO>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.NotNull(content);
    }

    [Fact]
    public async Task RefreshToken_given_access_token_with_no_NameIdentifier_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Invalid token" };

        var accessToken = CreateAccessTokenWithoutNameIdentifier(Guid.NewGuid().ToString(), "Gustav", "test@test.com");
        var refresh = _tokenGenerator.GenerateRefreshToken();

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Authentication/refresh-token", new TokenDTO { AccessToken = accessToken, RefreshToken = refresh.RefreshToken });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task RefreshToken_given_unexpired_access_token_returns_Unathorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Token has not expired yet" };

        var accessToken = _tokenGenerator.GenerateAccessToken(Guid.NewGuid().ToString(), "000000000000000000000001", "Gustav", "test@test.com");
        var refresh = _tokenGenerator.GenerateRefreshToken();

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Authentication/refresh-token", new TokenDTO { AccessToken = accessToken, RefreshToken = refresh.RefreshToken });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task RefreshToken_given_non_existing_refresh_token_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Invalid token" };

        var accessToken = CreateExpiredAccessToken(Guid.NewGuid().ToString(), "000000000000000000000001", "Gustav", "test@test.com");
        var refresh = _tokenGenerator.GenerateRefreshToken();

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Authentication/refresh-token", new TokenDTO { AccessToken = accessToken, RefreshToken = refresh.RefreshToken });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task RefreshToken_given_existing_expired_refresh_token_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Token expired" };

        var accessToken = CreateExpiredAccessToken("3a096f60-2ab0-4ecb-9627-1f02a22cccbd", "000000000000000000000001", "Gustav", "test@test.com");
        var refreshToken = "00000000000000000000000000000001";

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Authentication/refresh-token", new TokenDTO { AccessToken = accessToken, RefreshToken = refreshToken });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task RefreshToken_given_invalidated_existing_nonexpired_refresh_token_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Token has been invalidated" };

        var accessToken = CreateExpiredAccessToken("940890ef-5aff-4946-94b6-d6336979dabf", "000000000000000000000001", "Gustav", "test@test.com");
        var refreshToken = "00000000000000000000000000000003";

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Authentication/refresh-token", new TokenDTO { AccessToken = accessToken, RefreshToken = refreshToken });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task RefreshToken_given_used_existing_nonexpired_refresh_token_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Token has already been used" };

        var accessToken = CreateExpiredAccessToken("b8a21733-a66b-4c01-829a-aafe09638fa7", "000000000000000000000001", "Gustav", "test@test.com");
        var refreshToken = "00000000000000000000000000000004";

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Authentication/refresh-token", new TokenDTO { AccessToken = accessToken, RefreshToken = refreshToken });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task RefreshToken_given_refresh_token_not_belonging_to_access_token_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Invalid token" };

        var accessToken = CreateExpiredAccessToken("b8a21733-a66b-4c01-829a-aafe09638fa7", "000000000000000000000001", "Gustav", "test@test.com");
        var refreshToken = "00000000000000000000000000000002";

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Authentication/refresh-token", new TokenDTO { AccessToken = accessToken, RefreshToken = refreshToken });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task RefreshToken_given_access_token_with_invalid_userId_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Invalid user id" };

        var accessToken = CreateExpiredAccessToken("3fc85c49-6dce-4e66-abad-e3b7c1aea29e", "000000000000000000000000", "Gustav", "test@test.com");
        var refreshToken = "00000000000000000000000000000002";

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Authentication/refresh-token", new TokenDTO { AccessToken = accessToken, RefreshToken = refreshToken });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    [Priority(2)]
    public async Task RefreshToken_given_expired_access_token_and_non_expired_refresh_token_with_correct_credentials_returns_Ok()
    {
        // Arrange
        var accessToken = CreateExpiredAccessToken("3fc85c49-6dce-4e66-abad-e3b7c1aea29e", "000000000000000000000001", "Gustav", "test@test.com");
        var refreshToken = "00000000000000000000000000000002";

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Authentication/refresh-token", new TokenDTO { AccessToken = accessToken, RefreshToken = refreshToken });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.NotNull(content);
    }

    private string CreateAccessToken(Claim[] claims, DateTime expires, DateTime notBefore)
    {
        var key = Encoding.UTF8.GetBytes(_factory.JwtSettings.Key);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

        var subject = new ClaimsIdentity(claims);

         var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Issuer = _factory.JwtSettings.Issuer,
            Audience = _factory.JwtSettings.Audience,
            SigningCredentials = signingCredentials,
            Expires = expires,
            NotBefore = notBefore
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private string CreateExpiredAccessToken(string jwtId, string userId, string username, string email)
    {
        var key = Encoding.UTF8.GetBytes(_factory.JwtSettings.Key);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, jwtId),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email),
        };

        var expires = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5));
        var notBefore = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(6));

        return CreateAccessToken(claims, expires, notBefore);
    }

    private string CreateAccessTokenWithoutNameIdentifier(string jwtId, string username, string email)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, jwtId),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email),
        };

        var expires = DateTime.UtcNow.AddMinutes(_factory.JwtSettings.TokenExpiryMin);
        var notBefore = DateTime.UtcNow;

        return CreateAccessToken(claims, expires, notBefore);
    }
}