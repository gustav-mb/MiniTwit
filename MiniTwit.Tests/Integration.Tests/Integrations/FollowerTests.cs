using System.Net;
using System.Net.Http.Json;
using MiniTwit.Core.Error;

namespace MiniTwit.Tests.Integration.Tests.Integrations;

public class FollowerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public FollowerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task FollowUser_given_valid_username_and_userId_returns_Created()
    {
        // Act
        var actual = await _factory.CreateClient().PostAsync("/Follower/Gustav/follow?userId=000000000000000000000004", null);

        // Assert
        Assert.Equal(HttpStatusCode.Created, actual.StatusCode);
    }

    [Fact]
    public async Task FollowUser_given_invalid_userId_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = "Invalid user id" };

        // Act
        var actual = await _factory.CreateClient().PostAsync("/Follower/Gustav/follow?userId=000000000000000000000000", null);
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task FollowUser_given_invalid_username_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = "Invalid username" };

        // Act
        var actual = await _factory.CreateClient().PostAsync("/Follower/Test/follow?userId=000000000000000000000004", null);
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task FollowUser_given_same_userId_and_username_returns_BadRequest()
    {
        // Arrange
        var expected = new APIError { Status = 400, ErrorMsg = "Can't follow yourself" };

        // Act
        var actual = await _factory.CreateClient().PostAsync("/Follower/Gustav/follow?userId=000000000000000000000001", null);
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task UnfollowUser_given_valid_username_and_userId_returns_NoContent()
    {
        // Act
        var actual = await _factory.CreateClient().DeleteAsync("/Follower/Gustav/unfollow?userId=000000000000000000000004");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, actual.StatusCode);
    }

    [Fact]
    public async Task UnfollowUser_given_invalid_userId_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = "Invalid user id" };

        // Act
        var actual = await _factory.CreateClient().DeleteAsync("/Follower/Gustav/unfollow?userId=000000000000000000000000");
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task UnfollowUser_given_invalid_username_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = "Invalid username" };

        // Act
        var actual = await _factory.CreateClient().DeleteAsync("/Follower/Test/unfollow?userId=000000000000000000000004");
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task UnfollowUser_given_same_userId_and_username_returns_BadRequest()
    {
        // Arrange
        var expected = new APIError { Status = 400, ErrorMsg = "Can't unfollow yourself" };

        // Act
        var actual = await _factory.CreateClient().DeleteAsync("/Follower/Gustav/unfollow?userId=000000000000000000000001");
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, actual.StatusCode);
        Assert.Equal(expected, content);
    }
}