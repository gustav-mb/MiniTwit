using MiniTwit.Core.Responses;
using MiniTwit.Infrastructure.Repositories;
using MongoDB.Driver;
using static MiniTwit.Core.Error.DBError;

namespace MiniTwit.Tests.Infrastructure.Tests.Repositories;

public class FollowerRepositoryTests : RepositoryTests
{
    private readonly FollowerRepository _repository;

    public FollowerRepositoryTests()
    {
        _repository = new FollowerRepository(Context);
    }

    [Fact]
    public async Task CreateAsync_given_invalid_sourceId_returns_InvalidUserId()
    {
        // Arrange
        var expected = new DBResult
        {
            DBError = INVALID_USER_ID
        };

        // Act
        var actual = await _repository.CreateAsync("000000000000000000000000", "Gustav");
        var actualFollower = await Context.Followers.Find(follower => follower.WhoId == "000000000000000000000000").FirstOrDefaultAsync();

        // Assert
        Assert.Equal(expected, actual);
        Assert.Null(actualFollower);
    }

    [Fact]
    public async Task CreateAsync_given_invalid_targetUsername_returns_InvalidUsername()
    {
        // Arrange
        var expected = new DBResult
        {
            DBError = INVALID_USERNAME
        };

        // Act
        var actual = await _repository.CreateAsync("000000000000000000000001", "test");
        var actualFollower = await Context.Followers.Find(follower => follower.WhoId == "000000000000000000000000").FirstOrDefaultAsync();

        // Assert
        Assert.Equal(expected, actual);
        Assert.Null(actualFollower);
    }

    [Fact]
    public async Task CreateAsync_given_same_sourceId_and_username_returns_FollowSelf()
    {
        // Arrange
        var expected = new DBResult
        {
            DBError = FOLLOW_SELF
        };

        // Act
        var actual = await _repository.CreateAsync("000000000000000000000001", "Gustav");
        var actualFollower = await Context.Followers.Find(follower => follower.WhoId == "000000000000000000000000").FirstOrDefaultAsync();

        // Assert
        Assert.Equal(expected, actual);
        Assert.Null(actualFollower);
    }

    [Fact]
    public async Task CreateAsync_given_valid_sourceId_and_targetUsername_returns_no_error_and_creates_Follower()
    {
        // Arrange
        var expected = new DBResult
        {
            DBError = null
        };

        // Act
        var actual = await _repository.CreateAsync("000000000000000000000001", "Victor");
        var actualFollower = await Context.Followers.Find(follower => follower.WhoId == "000000000000000000000001" && follower.WhomId == "000000000000000000000004").FirstOrDefaultAsync();

        // Assert
        Assert.Equal(expected, actual);
        Assert.NotNull(actualFollower);
        Assert.Equal("000000000000000000000001", actualFollower.WhoId);
        Assert.Equal("000000000000000000000004", actualFollower.WhomId);
    }

    [Fact]
    public async Task DeleteAsync_given_invalid_sourceId_returns_InvalidUserId()
    {
        // Arrange
        var expected = new DBResult
        {
            DBError = INVALID_USER_ID
        };

        // Act
        var actual = await _repository.DeleteAsync("000000000000000000000000", "Gustav");

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task DeleteAsync_given_invalid_targetUsername_returns_InvalidUsername()
    {
        // Arrange
        var expected = new DBResult
        {
            DBError = INVALID_USERNAME
        };

        // Act
        var actual = await _repository.CreateAsync("000000000000000000000001", "test");

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task DeleteAsync_given_same_sourceId_and_username_returns_UnfollowSelf()
    {
        // Arrange
        var expected = new DBResult
        {
            DBError = UNFOLLOW_SELF
        };

        // Act
        var actual = await _repository.DeleteAsync("000000000000000000000001", "Gustav");

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task DeleteAsync_given_valid_sourceId_and_targetUsername_returns_no_error()
    {
        // Arrange
        var expected = new DBResult
        {
            DBError = null
        };

        // Act
        var actual = await _repository.DeleteAsync("000000000000000000000001", "Victor");
        var actualFollower = await Context.Followers.Find(follower => follower.WhoId == "000000000000000000000001" && follower.WhomId == "000000000000000000000004").FirstOrDefaultAsync();

        // Assert
        Assert.Equal(expected, actual);
        Assert.Null(actualFollower);
    }
}