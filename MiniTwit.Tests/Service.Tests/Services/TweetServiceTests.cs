using Moq;
using MiniTwit.Core.DTOs;
using MiniTwit.Core.IRepositories;
using MiniTwit.Core.Responses;
using MiniTwit.Service.Services;
using MiniTwit.Core.Entities;
using static MiniTwit.Core.Error.Errors;
using static MiniTwit.Core.Responses.HTTPResponse;

namespace MiniTwit.Tests.Service.Tests.Services;

public class TweetServiceTests
{
    private readonly CancellationToken _ct;

    public TweetServiceTests()
    {
        _ct = new CancellationToken();
    }

    [Fact]
    public async Task CreateTweetAsync_given_invalid_authorId_returns_NotFound_with_InvalidUserId()
    {
        // Arrange
        var expected = new APIResponse(NotFound, INVALID_USER_ID);

        var repository = new Mock<ITweetRepository>();
        repository.Setup(r => r.CreateAsync("000000000000000000000000", "text")).ReturnsAsync(new DBResult { DBError = INVALID_USER_ID });
        var service = new TweetService(repository.Object);

        // Act
        var actual = await service.CreateTweetAsync(new TweetCreateDTO { AuthorId = "000000000000000000000000", Text = "text" });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task CreateTweetAsync_given_valid_authorId_returns_Created()
    {
        // Arrange
        var expected = new APIResponse(Created);

        var repository = new Mock<ITweetRepository>();
        repository.Setup(r => r.CreateAsync("000000000000000000000001", "text")).ReturnsAsync(new DBResult { DBError = null });
        var service = new TweetService(repository.Object);

        // Act
        var actual = await service.CreateTweetAsync(new TweetCreateDTO { AuthorId = "000000000000000000000001", Text = "text" });

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetAllNonFlaggedTweetsAsync_returns_Ok_and_Tweets()
    {
        // Arrange
        var expected = new APIResponse<IEnumerable<TweetDTO>>(Ok, Enumerable.Empty<TweetDTO>());

        var repository = new Mock<ITweetRepository>();
        repository.Setup(r => r.GetAllNonFlaggedAsync(null, _ct)).ReturnsAsync(new DBResult<IEnumerable<Tweet>> { Model = Enumerable.Empty<Tweet>(), DBError = null });
        var service = new TweetService(repository.Object);

        // Act
        var actual = await service.GetAllNonFlaggedTweetsAsync(null, _ct);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetUsersAndFollowedNonFlaggedTweetsAsync_given_invalid_userId_returns_NotFound_with_InvalidUserId()
    {
        // Arrange
        var expected = new APIResponse<IEnumerable<TweetDTO>>(NotFound, null, INVALID_USER_ID);

        var repository = new Mock<ITweetRepository>();
        repository.Setup(r => r.GetAllNonFlaggedFollowedByUserIdAsync("000000000000000000000000", null, _ct)).ReturnsAsync(new DBResult<IEnumerable<Tweet>> { Model = null, DBError = INVALID_USER_ID });
        var service = new TweetService(repository.Object);

        // Act
        var actual = await service.GetUsersAndFollowedNonFlaggedTweetsAsync("000000000000000000000000", null, _ct);

        // Assert
        Assert.Equal(expected, actual);   
    }

    [Fact]
    public async Task GetUsersAndFollowedNonFlaggedTweetsAsync_given_valid_userId_returns_Ok_and_Tweets()
    {
        // Arrange
        var expected = new APIResponse<IEnumerable<TweetDTO>>(Ok, Enumerable.Empty<TweetDTO>());

        var repository = new Mock<ITweetRepository>();
        repository.Setup(r => r.GetAllNonFlaggedFollowedByUserIdAsync("000000000000000000000001", null, _ct)).ReturnsAsync(new DBResult<IEnumerable<Tweet>> { Model = Enumerable.Empty<Tweet>(), DBError = null });
        var service = new TweetService(repository.Object);

        // Act
        var actual = await service.GetUsersAndFollowedNonFlaggedTweetsAsync("000000000000000000000001", null, _ct);

        // Assert
        Assert.Equal(expected, actual);   
    }

    [Fact]
    public async Task GetUsersTweetsAsync_given_invalid_username_returns_NotFound_with_InvalidUsername()
    {
        // Arrange
        var expected = new APIResponse<IEnumerable<TweetDTO>>(NotFound, null, INVALID_USERNAME);

        var repository = new Mock<ITweetRepository>();
        repository.Setup(r => r.GetAllByUsernameAsync("Test", null, _ct)).ReturnsAsync(new DBResult<IEnumerable<Tweet>> { Model = null, DBError = INVALID_USERNAME });
        var service = new TweetService(repository.Object);

        // Act
        var actual = await service.GetUsersTweetsAsync("Test", null, _ct);

        // Assert
        Assert.Equal(expected, actual); 
    }

    [Fact]
    public async Task GetUsersTweetsAsync_given_valid_username_returns_Ok_and_Tweets()
    {
        // Arrange
        var expected = new APIResponse<IEnumerable<TweetDTO>>(Ok, Enumerable.Empty<TweetDTO>());

        var repository = new Mock<ITweetRepository>();
        repository.Setup(r => r.GetAllByUsernameAsync("Gustav", null, _ct)).ReturnsAsync(new DBResult<IEnumerable<Tweet>> { Model = Enumerable.Empty<Tweet>(), DBError = null });
        var service = new TweetService(repository.Object);

        // Act
        var actual = await service.GetUsersTweetsAsync("Gustav", null, _ct);

        // Assert
        Assert.Equal(expected, actual); 
    }
}