using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using MiniTwit.Server.Controllers;
using MiniTwit.Service;
using MiniTwit.Core.Responses;
using MiniTwit.Core.DTOs;
using MiniTwit.Core.Error;
using static MiniTwit.Core.Error.DBError;
using static MiniTwit.Core.Responses.HTTPResponse;

namespace MiniTwit.Tests.Server.Tests.Controllers;

public class TweetControllerTests
{
    private readonly Mock<ILogger<TweetController>> _logger;
    private readonly CancellationToken _ct;

    public TweetControllerTests()
    {
        _logger = new Mock<ILogger<TweetController>>();
        _ct = new CancellationToken();
    }

    [Fact]
    public async Task Timeline_given_valid_userId_returns_Ok()
    {
        // Arrange
        var expected = Enumerable.Empty<TweetDTO>();

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.TweetService.GetUsersAndFollowedNonFlaggedTweetsAsync("000000000000000000000001", null, _ct)).ReturnsAsync(new APIResponse<IEnumerable<TweetDTO>>(Ok, Enumerable.Empty<TweetDTO>(), null));
        var controller = new TweetController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.Timeline("000000000000000000000001", null, _ct)).Result as OkObjectResult;

        // Assert
        Assert.Equal(200, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task Timeline_given_invalid_userId_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = "Invalid user id" };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.TweetService.GetUsersAndFollowedNonFlaggedTweetsAsync("000000000000000000000000", null, _ct)).ReturnsAsync(new APIResponse<IEnumerable<TweetDTO>>(NotFound, null, INVALID_USER_ID));
        var controller = new TweetController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.Timeline("000000000000000000000000", null, _ct)).Result as NotFoundObjectResult;

        // Assert
        Assert.Equal(404, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task PublicTimeline_returns_Ok()
    {
        // Arrange
        var expected = Enumerable.Empty<TweetDTO>();

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.TweetService.GetAllNonFlaggedTweetsAsync(null, _ct)).ReturnsAsync(new APIResponse<IEnumerable<TweetDTO>>(Ok, Enumerable.Empty<TweetDTO>(), null));
        var controller = new TweetController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.PublicTimeline(null, _ct)).Result as OkObjectResult;

        // Assert
        Assert.Equal(200, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task UserTimeline_given_invalid_username_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = "Invalid username" };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.TweetService.GetUsersTweetsAsync("Test", null, _ct)).ReturnsAsync(new APIResponse<IEnumerable<TweetDTO>>(NotFound, null, INVALID_USERNAME));
        var controller = new TweetController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.UserTimeline("Test", null, _ct)).Result as NotFoundObjectResult;

        // Assert
        Assert.Equal(404, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task UserTimeline_given_valid_username_returns_Ok()
    {
        // Arrange
        var expected = Enumerable.Empty<TweetDTO>();

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.TweetService.GetUsersTweetsAsync("Gustav", null, _ct)).ReturnsAsync(new APIResponse<IEnumerable<TweetDTO>>(Ok, Enumerable.Empty<TweetDTO>(), null));
        var controller = new TweetController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.UserTimeline("Gustav", null, _ct)).Result as OkObjectResult;

        // Assert
        Assert.Equal(200, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task AddMessage_given_invalid_AuthorId_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = "Invalid user id" };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.TweetService.CreateTweetAsync(new TweetCreateDTO { AuthorId = "000000000000000000000000", Text = "text" })).ReturnsAsync(new APIResponse(NotFound, INVALID_USER_ID));
        var controller = new TweetController(serviceManager.Object, _logger.Object);

        // Act
        var actual = await controller.AddMessage(new TweetCreateDTO { AuthorId = "000000000000000000000000", Text = "text" }) as NotFoundObjectResult;

        // Assert
        Assert.Equal(404, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task AddMessage_given_valid_AuthorId_returns_Created()
    {
        // Arrange
        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.TweetService.CreateTweetAsync(new TweetCreateDTO { AuthorId = "000000000000000000000001", Text = "text" })).ReturnsAsync(new APIResponse(Created, null));
        var controller = new TweetController(serviceManager.Object, _logger.Object);

        // Act
        var actual = await controller.AddMessage(new TweetCreateDTO { AuthorId = "000000000000000000000001", Text = "text" }) as CreatedResult;

        // Assert
        Assert.Equal(201, actual!.StatusCode);
        Assert.Equal("", actual.Location);
        Assert.Null(actual.Value);
    }
}