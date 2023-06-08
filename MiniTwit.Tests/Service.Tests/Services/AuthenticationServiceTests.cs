using Moq;
using MiniTwit.Core.DTOs;
using MiniTwit.Core.Responses;
using MiniTwit.Security.Authentication.TokenGenerators;
using MiniTwit.Security.Hashing;
using MiniTwit.Core.IRepositories;
using MiniTwit.Service.Services;
using MiniTwit.Core.Entities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using static MiniTwit.Core.Error.DBError;
using static MiniTwit.Core.Responses.HTTPResponse;

namespace MiniTwit.Tests.Service.Tests.Services;

public class AuthenticationServiceTests
{
    private readonly Mock<IHasher> _hasher;
    private readonly Mock<ITokenGenerator> _tokenGenerator;
    private readonly CancellationToken _ct;

    public AuthenticationServiceTests()
    {
        _tokenGenerator = new Mock<ITokenGenerator>();
        _hasher = new Mock<IHasher>();
        _ct = new CancellationToken();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task AuthenticateAsync_given_null_or_empty_Username_returns_Unauthorized_with_UsernameMissing(string username)
    {
        // Arrange
        var expected = new APIResponse<TokenDTO>(Unauthorized, null, USERNAME_MISSING);

        var userRepository = new Mock<IUserRepository>();
        var tokenRepository = new Mock<ITokenRepository>();
        var service = new AuthenticationService(userRepository.Object, tokenRepository.Object, _hasher.Object, _tokenGenerator.Object);

        // Act
        var actual = await service.AuthenticateAsync(new LoginDTO { Username = username, Password = "password" });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task AuthenticateAsync_given_null_or_empty_Password_returns_Unauthorized_with_PasswordMissing(string password)
    {
        // Arrange
        var expected = new APIResponse<TokenDTO>(Unauthorized, null, PASSWORD_MISSING);

        var userRepository = new Mock<IUserRepository>();
        var tokenRepository = new Mock<ITokenRepository>();
        var service = new AuthenticationService(userRepository.Object, tokenRepository.Object, _hasher.Object, _tokenGenerator.Object);

        // Act
        var actual = await service.AuthenticateAsync(new LoginDTO { Username = "Gustav", Password = password });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task AuthenticateAsync_given_invalid_Username_returns_Unauthorized_with_InvalidUsername()
    {
        // Arrange
        var expected = new APIResponse<TokenDTO>(Unauthorized, null, INVALID_USERNAME);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(ur => ur.GetByUsernameAsync("Test", _ct)).ReturnsAsync(new DBResult<User> { DBError = INVALID_USERNAME, Model = null });

        var tokenRepository = new Mock<ITokenRepository>();
        var service = new AuthenticationService(userRepository.Object, tokenRepository.Object, _hasher.Object, _tokenGenerator.Object);

        // Act
        var actual = await service.AuthenticateAsync(new LoginDTO { Username = "Test", Password = "password" });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task AuthenticateAsync_given_valid_Username_and_invalid_Password_returns_Unauthorized_with_InvalidPassword()
    {
        // Arrange
        var expected = new APIResponse<TokenDTO>(Unauthorized, null, INVALID_PASSWORD);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(ur => ur.GetByUsernameAsync("Gustav", _ct)).ReturnsAsync(new DBResult<User> { DBError = null, Model = new User { Id = "000000000000000000000001", Username = "Gustav", Email = "test@test.com", Password = "password", Salt = "salt" } });

        var tokenRepository = new Mock<ITokenRepository>();
        _hasher.Setup(h => h.VerifyHashAsync("wrong", "password", "salt")).ReturnsAsync(false);

        var service = new AuthenticationService(userRepository.Object, tokenRepository.Object, _hasher.Object, _tokenGenerator.Object);

        // Act
        var actual = await service.AuthenticateAsync(new LoginDTO { Username = "Gustav", Password = "wrong" });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task AuthenticateAsync_given_valid_Username_and_Password_returns_Ok_and_TokenDTO()
    {
        // Arrange
        var expected = new APIResponse<TokenDTO>(Ok, new TokenDTO { AccessToken = "access", RefreshToken = "refresh" }, null);
        var exiryTime = DateTime.UtcNow;

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(ur => ur.GetByUsernameAsync("Gustav", _ct)).ReturnsAsync(new DBResult<User> { DBError = null, Model = new User { Id = "000000000000000000000001", Username = "Gustav", Email = "test@test.com", Password = "password", Salt = "salt" } });

        var tokenRepository = new Mock<ITokenRepository>();
        tokenRepository.Setup(tr => tr.CreateAsync(It.IsAny<string>(), "000000000000000000000001", "refresh", exiryTime)).ReturnsAsync(new DBResult { DBError = null });
        tokenRepository.Setup(tr => tr.DeleteAllExpiredAsync()).ReturnsAsync(new DBResult { DBError = null });

        _hasher.Setup(h => h.VerifyHashAsync("password", "password", "salt")).ReturnsAsync(true);

        _tokenGenerator.Setup(tg => tg.GenerateAccessToken(It.IsAny<string>(), "000000000000000000000001", "Gustav", "test@test.com")).Returns("access");
        _tokenGenerator.Setup(tg => tg.GenerateRefreshToken()).Returns(("refresh", exiryTime));

        var service = new AuthenticationService(userRepository.Object, tokenRepository.Object, _hasher.Object, _tokenGenerator.Object);

        // Act
        var actual = await service.AuthenticateAsync(new LoginDTO { Username = "Gustav", Password = "password" });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RefreshTokenAsync_given_null_identifier_from_access_token_returns_Unauthorized_with_InvalidToken()
    {
        // Arrange
        var expected = new APIResponse<TokenDTO>(Unauthorized, null, INVALID_TOKEN);

        var userRepository = new Mock<IUserRepository>();
        var tokenRepository = new Mock<ITokenRepository>();

        _tokenGenerator.Setup(tg => tg.GetClaimFromAccessToken(ClaimTypes.NameIdentifier, "access")).Returns((string?)null);

        var service = new AuthenticationService(userRepository.Object, tokenRepository.Object, _hasher.Object, _tokenGenerator.Object);

        // Act
        var actual = await service.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RefreshTokenAsync_given_access_token_has_not_expired_returns_Unauthorized_with_NoToken()
    {
        // Arrange
        var expected = new APIResponse<TokenDTO>(Unauthorized, null, TOKEN_NOT_EXPIRED);

        var userRepository = new Mock<IUserRepository>();
        var tokenRepository = new Mock<ITokenRepository>();

        _tokenGenerator.Setup(tg => tg.GetClaimFromAccessToken(ClaimTypes.NameIdentifier, "access")).Returns("000000000000000000000001");
        _tokenGenerator.Setup(tg => tg.GetExpirationTimeFromAccessToken("access")).Returns(DateTime.UtcNow.AddMinutes(5));

        var service = new AuthenticationService(userRepository.Object, tokenRepository.Object, _hasher.Object, _tokenGenerator.Object);

        // Act
        var actual = await service.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RefreshTokenAsync_given_refresh_token_does_not_exist_returns_Unauthorized_with_InvalidToken()
    {
        // Arrange
        var expected = new APIResponse<TokenDTO>(Unauthorized, null, INVALID_TOKEN);

        var userRepository = new Mock<IUserRepository>();
        var tokenRepository = new Mock<ITokenRepository>();
        tokenRepository.Setup(tr => tr.GetAsync("refresh")).ReturnsAsync(new DBResult<RefreshToken> { DBError = INVALID_TOKEN, Model = null });

        _tokenGenerator.Setup(tg => tg.GetClaimFromAccessToken(ClaimTypes.NameIdentifier, "access")).Returns("000000000000000000000001");
        _tokenGenerator.Setup(tg => tg.GetExpirationTimeFromAccessToken("access")).Returns(DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5)));

        var service = new AuthenticationService(userRepository.Object, tokenRepository.Object, _hasher.Object, _tokenGenerator.Object);

        // Act
        var actual = await service.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RefreshTokenAsync_given_expired_refresh_token_returns_Unauthorized_with_TokenExpired()
    {
        // Arrange
        var expected = new APIResponse<TokenDTO>(Unauthorized, null, TOKEN_EXPIRED);

        var userRepository = new Mock<IUserRepository>();
        var tokenRepository = new Mock<ITokenRepository>();
        tokenRepository.Setup(tr => tr.GetAsync("refresh")).ReturnsAsync(new DBResult<RefreshToken> { DBError = null, Model = new RefreshToken { JwtId = "0", Token = "refresh", UserId = "000000000000000000000001", ExpiryTime = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(60)), Used = false, Invalidated = false } });

        _tokenGenerator.Setup(tg => tg.GetClaimFromAccessToken(ClaimTypes.NameIdentifier, "access")).Returns("000000000000000000000001");
        _tokenGenerator.Setup(tg => tg.GetExpirationTimeFromAccessToken("access")).Returns(DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5)));

        var service = new AuthenticationService(userRepository.Object, tokenRepository.Object, _hasher.Object, _tokenGenerator.Object);

        // Act
        var actual = await service.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RefreshTokenAsync_given_invalidated_refresh_token_returns_Unauthorized_with_TokenInvalidated()
    {
        // Arrange
        var expected = new APIResponse<TokenDTO>(Unauthorized, null, TOKEN_INVALIDATED);
        var expirationTime = DateTime.UtcNow.AddMinutes(60);

        var userRepository = new Mock<IUserRepository>();
        var tokenRepository = new Mock<ITokenRepository>();
        tokenRepository.Setup(tr => tr.GetAsync("refresh")).ReturnsAsync(new DBResult<RefreshToken> { DBError = null, Model = new RefreshToken { JwtId = "0", Token = "refresh", UserId = "000000000000000000000001", ExpiryTime = expirationTime, Used = false, Invalidated = true } });

        _tokenGenerator.Setup(tg => tg.GetClaimFromAccessToken(ClaimTypes.NameIdentifier, "access")).Returns("000000000000000000000001");
        _tokenGenerator.Setup(tg => tg.GetExpirationTimeFromAccessToken("access")).Returns(DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5)));

        var service = new AuthenticationService(userRepository.Object, tokenRepository.Object, _hasher.Object, _tokenGenerator.Object);

        // Act
        var actual = await service.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RefreshTokenAsync_given_used_refresh_token_returns_Unauthorized_with_TokenUsed()
    {
        // Arrange
        var expected = new APIResponse<TokenDTO>(Unauthorized, null, TOKEN_USED);
        var expirationTime = DateTime.UtcNow.AddMinutes(60);

        var userRepository = new Mock<IUserRepository>();
        var tokenRepository = new Mock<ITokenRepository>();
        tokenRepository.Setup(tr => tr.GetAsync("refresh")).ReturnsAsync(new DBResult<RefreshToken> { DBError = null, Model = new RefreshToken { JwtId = "0", Token = "refresh", UserId = "000000000000000000000001", ExpiryTime = expirationTime, Used = true, Invalidated = false } });

        _tokenGenerator.Setup(tg => tg.GetClaimFromAccessToken(ClaimTypes.NameIdentifier, "access")).Returns("000000000000000000000001");
        _tokenGenerator.Setup(tg => tg.GetExpirationTimeFromAccessToken("access")).Returns(DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5)));

        var service = new AuthenticationService(userRepository.Object, tokenRepository.Object, _hasher.Object, _tokenGenerator.Object);

        // Act
        var actual = await service.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RefreshTokenAsync_given_refresh_token_not_belonging_to_access_token_returns_Unauthorized_with_InvalidToken()
    {
        // Arrange
        var expected = new APIResponse<TokenDTO>(Unauthorized, null, INVALID_TOKEN);
        var expirationTime = DateTime.UtcNow.AddMinutes(60);

        var userRepository = new Mock<IUserRepository>();
        var tokenRepository = new Mock<ITokenRepository>();
        tokenRepository.Setup(tr => tr.GetAsync("refresh")).ReturnsAsync(new DBResult<RefreshToken> { DBError = null, Model = new RefreshToken { JwtId = "0", Token = "refresh", UserId = "000000000000000000000001", ExpiryTime = expirationTime, Used = false, Invalidated = false } });

        _tokenGenerator.Setup(tg => tg.GetClaimFromAccessToken(ClaimTypes.NameIdentifier, "access")).Returns("000000000000000000000001");
        _tokenGenerator.Setup(tg => tg.GetClaimFromAccessToken(JwtRegisteredClaimNames.Jti, "acesss")).Returns("1");
        _tokenGenerator.Setup(tg => tg.GetExpirationTimeFromAccessToken("access")).Returns(DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5)));

        var service = new AuthenticationService(userRepository.Object, tokenRepository.Object, _hasher.Object, _tokenGenerator.Object);

        // Act
        var actual = await service.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RefreshTokenAsync_given_invalid_userId_returns_Unauthorized_with_InvalidUserId()
    {
        // Arrange
        var expected = new APIResponse<TokenDTO>(Unauthorized, null, INVALID_USER_ID);
        var expirationTime = DateTime.UtcNow.AddMinutes(60);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(ur => ur.GetByUserIdAsync("000000000000000000000000", _ct)).ReturnsAsync(new DBResult<User> { DBError = INVALID_USER_ID, Model = null });

        var tokenRepository = new Mock<ITokenRepository>();
        tokenRepository.Setup(tr => tr.GetAsync("refresh")).ReturnsAsync(new DBResult<RefreshToken> { DBError = null, Model = new RefreshToken { JwtId = "0", Token = "refresh", UserId = "000000000000000000000000", ExpiryTime = expirationTime, Used = false, Invalidated = false } });

        _tokenGenerator.Setup(tg => tg.GetClaimFromAccessToken(ClaimTypes.NameIdentifier, "access")).Returns("000000000000000000000000");
        _tokenGenerator.Setup(tg => tg.GetClaimFromAccessToken(JwtRegisteredClaimNames.Jti, "access")).Returns("0");
        _tokenGenerator.Setup(tg => tg.GetExpirationTimeFromAccessToken("access")).Returns(DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5)));

        var service = new AuthenticationService(userRepository.Object, tokenRepository.Object, _hasher.Object, _tokenGenerator.Object);

        // Act
        var actual = await service.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RefreshTokenAsync_given_valid_access_and_refresh_token_returns_Ok_and_TokenDTO()
    {
        // Arrange
        var expected = new APIResponse<TokenDTO>(Ok, new TokenDTO { AccessToken = "newAccess", RefreshToken = "newRefresh" }, null);
        var expirationTime = DateTime.UtcNow.AddMinutes(60);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(ur => ur.GetByUserIdAsync("000000000000000000000001", _ct)).ReturnsAsync(new DBResult<User> { DBError = null, Model = new User { Id = "000000000000000000000001", Username = "Gustav", Email = "test@test.com", Password = "password", Salt = "salt" } });

        var tokenRepository = new Mock<ITokenRepository>();
        tokenRepository.Setup(tr => tr.GetAsync("refresh")).ReturnsAsync(new DBResult<RefreshToken> { DBError = null, Model = new RefreshToken { JwtId = "0", Token = "refresh", UserId = "000000000000000000000001", ExpiryTime = expirationTime, Used = false, Invalidated = false } });
        tokenRepository.Setup(tr => tr.UpdateUsedAsync("refresh", true)).ReturnsAsync(new DBResult { DBError = null });
        tokenRepository.Setup(tr => tr.CreateAsync(It.IsAny<string>(), "000000000000000000000001", "newRefresh", expirationTime)).ReturnsAsync(new DBResult { DBError = null });
        tokenRepository.Setup(tr => tr.DeleteAllExpiredAsync()).ReturnsAsync(new DBResult { DBError = null });

        _tokenGenerator.Setup(tg => tg.GetClaimFromAccessToken(ClaimTypes.NameIdentifier, "access")).Returns("000000000000000000000001");       // Id claim is valid
        _tokenGenerator.Setup(tg => tg.GetClaimFromAccessToken(JwtRegisteredClaimNames.Jti, "access")).Returns("0");                            // Jti from refresh belongs to access
        _tokenGenerator.Setup(tg => tg.GetExpirationTimeFromAccessToken("access")).Returns(DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5)));  // Access token has expired
        _tokenGenerator.Setup(tg => tg.GenerateAccessToken(It.IsAny<string>(), "000000000000000000000001", "Gustav", "test@test.com")).Returns("newAccess");
        _tokenGenerator.Setup(tg => tg.GenerateRefreshToken()).Returns(("newRefresh", expirationTime));

        var service = new AuthenticationService(userRepository.Object, tokenRepository.Object, _hasher.Object, _tokenGenerator.Object);

        // Act
        var actual = await service.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" });

        // Assert
        Assert.Equal(expected, actual);
    }
}