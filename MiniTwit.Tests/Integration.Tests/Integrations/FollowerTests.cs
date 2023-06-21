using System.Net;
using System.Net.Http.Json;
using MiniTwit.Core.Error;
using static MiniTwit.Core.Error.Errors;

namespace MiniTwit.Tests.Integration.Tests.Integrations;

public class FollowerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public FollowerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task FollowUser_given_valid_username_and_userId_with_same_claim_UserId_returns_Created()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithNameIdentifier("000000000000000000000001");

        // Act
        var actual = await _factory.CreateClient(claimsProvider).PostAsync("/Follower/Victor/follow?userId=000000000000000000000001", null);

        // Assert
        Assert.Equal(HttpStatusCode.Created, actual.StatusCode);
    }

    [Fact]
    public async Task FollowUser_given_invalid_userId_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = INVALID_USER_ID };
        var claimsProvider = TestClaimsProvider.WithNameIdentifier("000000000000000000000000");

        // Act
        var actual = await _factory.CreateClient(claimsProvider).PostAsync("/Follower/Gustav/follow?userId=000000000000000000000000", null);
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task FollowUser_given_invalid_username_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = INVALID_USERNAME };
        var claimsProvider = TestClaimsProvider.WithNameIdentifier("000000000000000000000004");

        // Act
        var actual = await _factory.CreateClient(claimsProvider).PostAsync("/Follower/Test/follow?userId=000000000000000000000004", null);
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task FollowUser_given_same_userId_and_username_returns_BadRequest()
    {
        // Arrange
        var expected = new APIError { Status = 400, ErrorMsg = FOLLOW_SELF };
        var claimsProvider = TestClaimsProvider.WithNameIdentifier("000000000000000000000001");

        // Act
        var actual = await _factory.CreateClient(claimsProvider).PostAsync("/Follower/Gustav/follow?userId=000000000000000000000001", null);
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task FollowUser_given_different_UserId_in_claims_returns_Forbidden()
    {
        // Arrange
        var expected = new APIError { Status = 403, ErrorMsg = FORBIDDEN_OPERATION };
        var claimsProvider = TestClaimsProvider.WithNameIdentifier("000000000000000000000000");

        // Act
        var actual = await _factory.CreateClient(claimsProvider).PostAsync("/Follower/Gustav/follow?userId=000000000000000000000001", null);
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task UnfollowUser_given_valid_username_and_userId_with_same_claim_UserId_returns_NoContent()
    {
        // Arrange
        var claimsProvider = TestClaimsProvider.WithNameIdentifier("000000000000000000000004");

        // Act
        var actual = await _factory.CreateClient(claimsProvider).DeleteAsync("/Follower/Gustav/unfollow?userId=000000000000000000000004");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, actual.StatusCode);
    }

    [Fact]
    public async Task UnfollowUser_given_invalid_userId_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = INVALID_USER_ID };
        var claimsProvider = TestClaimsProvider.WithNameIdentifier("000000000000000000000000");

        // Act
        var actual = await _factory.CreateClient(claimsProvider).DeleteAsync("/Follower/Gustav/unfollow?userId=000000000000000000000000");
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task UnfollowUser_given_invalid_username_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = INVALID_USERNAME };
        var claimsProvider = TestClaimsProvider.WithNameIdentifier("000000000000000000000004");

        // Act
        var actual = await _factory.CreateClient(claimsProvider).DeleteAsync("/Follower/Test/unfollow?userId=000000000000000000000004");
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task UnfollowUser_given_same_userId_and_username_returns_BadRequest()
    {
        // Arrange
        var expected = new APIError { Status = 400, ErrorMsg = UNFOLLOW_SELF };
        var claimsProvider = TestClaimsProvider.WithNameIdentifier("000000000000000000000001");

        // Act
        var actual = await _factory.CreateClient(claimsProvider).DeleteAsync("/Follower/Gustav/unfollow?userId=000000000000000000000001");
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task UnfollowUser_given_different_UserId_in_claims_returns_Forbidden()
    {
        // Arrange
        var expected = new APIError { Status = 403, ErrorMsg = FORBIDDEN_OPERATION };
        var claimsProvider = TestClaimsProvider.WithNameIdentifier("000000000000000000000000");

        // Act
        var actual = await _factory.CreateClient(claimsProvider).DeleteAsync("/Follower/Gustav/unfollow?userId=000000000000000000000001");
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, actual.StatusCode);
        Assert.Equal(expected, content);
    }
}