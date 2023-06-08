using FluentAssertions;
using FluentAssertions.Extensions;
using MiniTwit.Core.Entities;
using MiniTwit.Core.Responses;
using MiniTwit.Infrastructure.Repositories;
using MongoDB.Driver;
using static MiniTwit.Core.Error.Errors;

namespace MiniTwit.Tests.Infrastructure.Tests.Repositories;

public class TokenRepositoryTests : RepositoryTests
{
    private readonly TokenRepository _repository;

    public TokenRepositoryTests()
    {
        _repository = new TokenRepository(Context);
    }

    [Fact]
    public async Task CreateAsync_given_invalid_userId_returns_InvalidUserId()
    {
        // Arrange
        var expected = new DBResult
        {
            DBError = INVALID_USER_ID
        };

        // Act
        var actual = await _repository.CreateAsync("0", "000000000000000000000000", "00000000000000000000000000000000", DateTime.UtcNow);
        var actualRefreshToken = await Context.RefreshTokens.Find(token => token.Token == "00000000000000000000000000000000").FirstOrDefaultAsync();

        // Assert
        Assert.Equal(expected, actual);
        Assert.Null(actualRefreshToken);
    }

    [Fact]
    public async Task CreateAsync_given_valid_userId_returns_no_error_and_creates_RefreshToken()
    {
        // Arrange
        var expiryTime = DateTime.Parse("01/01/2023 12:00:00").ToUniversalTime();
        var expected = new DBResult
        {
            DBError = null
        };

        // Act
        var actual = await _repository.CreateAsync("f22daf55-ff4f-43e8-afdb-06cc484f06ac", "000000000000000000000001", "00000000000000000000000000000000", expiryTime);
        var actualRefreshToken = await Context.RefreshTokens.Find(token => token.Token == "00000000000000000000000000000000").FirstOrDefaultAsync();

        // Assert
        Assert.Equal(expected, actual);
        Assert.NotNull(actualRefreshToken);
        Assert.Equal("f22daf55-ff4f-43e8-afdb-06cc484f06ac", actualRefreshToken.JwtId);
        Assert.Equal("000000000000000000000001", actualRefreshToken.UserId);
        Assert.Equal("00000000000000000000000000000000", actualRefreshToken.Token);
        Assert.Equal(expiryTime, actualRefreshToken.ExpiryTime);
        Assert.False(actualRefreshToken.Used);
        Assert.False(actualRefreshToken.Invalidated);
    }

    [Fact]
    public async Task DeleteAllExpiredAsync_deletes_all_expired_tokens()
    {
        // Act
        var actual = await _repository.DeleteAllExpiredAsync();
        var numTokens = Context.RefreshTokens.CountDocuments(_ => true);
        var actualToken = await Context.RefreshTokens.Find(token => token.Token == "3a096f60-2ab0-4ecb-9627-1f02a22cccbd").FirstOrDefaultAsync();

        // Assert
        Assert.Equal(1, numTokens);
        Assert.Null(actualToken);
    }

    [Fact]
    public async Task GetAsync_given_invalid_refreshToken_returns_InvalidToken()
    {
        // Arrange
        var expected = new DBResult<RefreshToken>
        {
            DBError = INVALID_TOKEN,
            Model = null
        };

        // Act
        var actual = await _repository.GetAsync("0");

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetAsync_given_valid_refreshToken_returns_no_error_and_RefreshToken()
    {
        // Arrange
        var expected = new RefreshToken { JwtId = "3fc85c49-6dce-4e66-abad-e3b7c1aea29e", Token = "00000000000000000000000000000002", UserId = "000000000000000000000001", ExpiryTime = DateTime.UtcNow.AddYears(1), Used = false, Invalidated = false };

        // Act
        var actual = await _repository.GetAsync("00000000000000000000000000000002");

        // Assert
        Assert.Null(actual.DBError);
        Assert.NotNull(actual.Model);
        Assert.Equal(expected.JwtId, actual.Model.JwtId);
        Assert.Equal(expected.UserId, actual.Model.UserId);
        Assert.Equal(expected.Token, actual.Model.Token);
        actual.Model.ExpiryTime.Should().BeCloseTo(expected.ExpiryTime, 10.Seconds());
        Assert.False(actual.Model.Used);
        Assert.False(actual.Model.Invalidated);
    }

    [Fact]
    public async Task UpdateUsedAsync_given_invalid_refreshToken_returns_InvalidToken()
    {
        // Arrange
        var expected = new DBResult
        {
            DBError = INVALID_TOKEN
        };

        // Act
        var actual = await _repository.UpdateUsedAsync("0", true);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UpdateUsedAsync_given_valid_refreshToken_updates_used()
    {
        // Arrange
        var expected = new RefreshToken { JwtId = "3fc85c49-6dce-4e66-abad-e3b7c1aea29e", Token = "00000000000000000000000000000002", UserId = "000000000000000000000001", ExpiryTime = DateTime.UtcNow.AddYears(1), Used = true, Invalidated = false };

        // Act
        var actual = await _repository.UpdateUsedAsync("00000000000000000000000000000002", true);
        var actualRefreshToken = await Context.RefreshTokens.Find(token => token.Token == "00000000000000000000000000000002").FirstOrDefaultAsync();

        // Assert
        Assert.Null(actual.DBError);
        Assert.NotNull(actualRefreshToken);
        Assert.Equal(expected.JwtId, actualRefreshToken.JwtId);
        Assert.Equal(expected.UserId, actualRefreshToken.UserId);
        Assert.Equal(expected.Token, actualRefreshToken.Token);
        actualRefreshToken.ExpiryTime.Should().BeCloseTo(expected.ExpiryTime, 10.Seconds());
        Assert.True(actualRefreshToken.Used);
        Assert.False(actualRefreshToken.Invalidated);
    }
}