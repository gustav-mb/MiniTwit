using Moq;
using MiniTwit.Core.Responses;
using MiniTwit.Service.Services;
using MiniTwit.Core.IRepositories;
using static MiniTwit.Core.Responses.HTTPResponse;
using static MiniTwit.Core.Error.Errors;

namespace MiniTwit.Tests.Service.Tests.Services;

public class FollowerServiceTests
{
    [Fact]
    public async Task FollowUserAsync_given_invalid_userId_returns_NotFound_with_InvalidUserId()
    {
        // Arrange
        var expected = new APIResponse(NotFound, INVALID_USER_ID);

        var repository = new Mock<IFollowerRepository>();
        repository.Setup(r => r.CreateAsync("000000000000000000000000", "Test")).ReturnsAsync(new DBResult{ DBError = INVALID_USER_ID });
        var service = new FollowerService(repository.Object);

        // Act
        var actual = await service.FollowUserAsync("000000000000000000000000", "Test");
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task FollowUserAsync_given_invalid_targetUsername_returns_NotFound_with_InvalidUSername()
    {
        // Arrange
        var expected = new APIResponse(NotFound, INVALID_USERNAME);

        var repository = new Mock<IFollowerRepository>();
        repository.Setup(r => r.CreateAsync("000000000000000000000001", "Test")).ReturnsAsync(new DBResult{ DBError = INVALID_USERNAME });
        var service = new FollowerService(repository.Object);

        // Act
        var actual = await service.FollowUserAsync("000000000000000000000001", "Test");
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task FollowUserAsync_given_same_userId_and_targetUsername_returns_BadRequest_with_FollowSelf()
    {
        // Arrange
        var expected = new APIResponse(BadRequest, FOLLOW_SELF);

        var repository = new Mock<IFollowerRepository>();
        repository.Setup(r => r.CreateAsync("000000000000000000000001", "Gustav")).ReturnsAsync(new DBResult{ DBError = FOLLOW_SELF });
        var service = new FollowerService(repository.Object);

        // Act
        var actual = await service.FollowUserAsync("000000000000000000000001", "Gustav");
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task FollowUserAsync_given_valid_userId_and_targetUsername_returns_Created()
    {
        // Arrange
        var expected = new APIResponse(Created);

        var repository = new Mock<IFollowerRepository>();
        repository.Setup(r => r.CreateAsync("000000000000000000000001", "Simon")).ReturnsAsync(new DBResult{ DBError = null });
        var service = new FollowerService(repository.Object);

        // Act
        var actual = await service.FollowUserAsync("000000000000000000000001", "Simon");
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UnfollowUserAsync_given_invalid_userId_returns_NotFound_with_InvalidUserId()
    {
        // Arrange
        var expected = new APIResponse(NotFound, INVALID_USER_ID);

        var repository = new Mock<IFollowerRepository>();
        repository.Setup(r => r.DeleteAsync("000000000000000000000000", "Test")).ReturnsAsync(new DBResult{ DBError = INVALID_USER_ID });
        var service = new FollowerService(repository.Object);

        // Act
        var actual = await service.UnfollowUserAsync("000000000000000000000000", "Test");
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UnfollowUserAsync_given_invalid_targetUsername_returns_NotFound_with_InvalidUSername()
    {
        // Arrange
        var expected = new APIResponse(NotFound, INVALID_USERNAME);

        var repository = new Mock<IFollowerRepository>();
        repository.Setup(r => r.DeleteAsync("000000000000000000000001", "Test")).ReturnsAsync(new DBResult{ DBError = INVALID_USERNAME });
        var service = new FollowerService(repository.Object);

        // Act
        var actual = await service.UnfollowUserAsync("000000000000000000000001", "Test");
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UnfollowUserAsync_given_same_userId_and_targetUsername_returns_BadRequest_with_UnfollowSelf()
    {
        // Arrange
        var expected = new APIResponse(BadRequest, UNFOLLOW_SELF);

        var repository = new Mock<IFollowerRepository>();
        repository.Setup(r => r.DeleteAsync("000000000000000000000001", "Gustav")).ReturnsAsync(new DBResult{ DBError = UNFOLLOW_SELF });
        var service = new FollowerService(repository.Object);

        // Act
        var actual = await service.UnfollowUserAsync("000000000000000000000001", "Gustav");
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UnfollowUserAsync_given_valid_userId_and_targetUsername_returns_NoContent()
    {
        // Arrange
        var expected = new APIResponse(NoContent);

        var repository = new Mock<IFollowerRepository>();
        repository.Setup(r => r.DeleteAsync("000000000000000000000001", "Simon")).ReturnsAsync(new DBResult{ DBError = null });
        var service = new FollowerService(repository.Object);

        // Act
        var actual = await service.UnfollowUserAsync("000000000000000000000001", "Simon");
        
        // Assert
        Assert.Equal(expected, actual);
    }
}