using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using MiniTwit.Core.Responses;
using MiniTwit.Server.Controllers;
using MiniTwit.Service;
using MiniTwit.Core.Error;
using MiniTwit.Server.Extensions;
using static MiniTwit.Core.Responses.HTTPResponse;
using static MiniTwit.Core.Error.Errors;

namespace MiniTwit.Tests.Server.Tests.Controllers;

public class FollowerControllerTests
{
    private readonly Mock<ILogger<FollowerController>> _logger;

    public FollowerControllerTests()
    {
        _logger = new Mock<ILogger<FollowerController>>();
    }

    [Fact]
    public async Task FollowUser_given_valid_username_and_userId_returns_Created()
    {
        // Arrange
        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.FollowerService.FollowUserAsync("000000000000000000000001", "Simon")).ReturnsAsync(new APIResponse(Created));

        var controller = new FollowerController(serviceManager.Object, _logger.Object);
        controller.ControllerContext.HttpContext = CreateHttpContextWithClaims("000000000000000000000001");

        // Act
        var actual = await controller.FollowUser("Simon", "000000000000000000000001") as CreatedResult;

        // Assert
        Assert.Equal(201, actual!.StatusCode);
        Assert.Null(actual.Value);
        Assert.Equal("", actual.Location);
    }

    [Fact]
    public async Task FollowUser_given_invalid_username_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = INVALID_USERNAME };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.FollowerService.FollowUserAsync("000000000000000000000001", "Test")).ReturnsAsync(new APIResponse(NotFound, INVALID_USERNAME));

        var controller = new FollowerController(serviceManager.Object, _logger.Object);
        controller.ControllerContext.HttpContext = CreateHttpContextWithClaims("000000000000000000000001");

        // Act
        var actual = await controller.FollowUser("Test", "000000000000000000000001") as NotFoundObjectResult;

        // Assert
        Assert.Equal(404, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task FollowUser_given_invalid_userId_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = INVALID_USER_ID };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.FollowerService.FollowUserAsync("000000000000000000000000", "Simon")).ReturnsAsync(new APIResponse(NotFound, INVALID_USER_ID));

        var controller = new FollowerController(serviceManager.Object, _logger.Object);
        controller.ControllerContext.HttpContext = CreateHttpContextWithClaims("000000000000000000000000");

        // Act
        var actual = await controller.FollowUser("Simon", "000000000000000000000000") as NotFoundObjectResult;

        // Assert
        Assert.Equal(404, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task FollowUser_given_same_username_and_userId_returns_BadRequest()
    {
        // Arrange
        var expected = new APIError { Status = 400, ErrorMsg = FOLLOW_SELF };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.FollowerService.FollowUserAsync("000000000000000000000001", "Gustav")).ReturnsAsync(new APIResponse(BadRequest, FOLLOW_SELF));
        
        var controller = new FollowerController(serviceManager.Object, _logger.Object);
        controller.ControllerContext.HttpContext = CreateHttpContextWithClaims("000000000000000000000001");

        // Act
        var actual = await controller.FollowUser("Gustav", "000000000000000000000001") as BadRequestObjectResult;

        // Assert
        Assert.Equal(400, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task FollowUser_given_different_UserId_in_claims_returns_Forbidden()
    {
        // Arrange
        var expected = new APIError { Status = 403, ErrorMsg = FORBIDDEN_OPERATION };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.FollowerService.FollowUserAsync("000000000000000000000001", "Gustav")).ReturnsAsync(new APIResponse(BadRequest, FOLLOW_SELF));

        var controller = new FollowerController(serviceManager.Object, _logger.Object);
        controller.ControllerContext.HttpContext = CreateHttpContextWithClaims("000000000000000000000000");

        // Act
        var actual = await controller.FollowUser("Gustav", "000000000000000000000001") as ForbiddenObjectResult;

        // Assert
        Assert.Equal(403, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task UnfollowUser_given_valid_username_and_userId_returns_Created()
    {
        // Arrange
        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.FollowerService.UnfollowUserAsync("000000000000000000000001", "Simon")).ReturnsAsync(new APIResponse(Created));

        var controller = new FollowerController(serviceManager.Object, _logger.Object);
        controller.ControllerContext.HttpContext = CreateHttpContextWithClaims("000000000000000000000001");

        // Act
        var actual = await controller.UnfollowUser("Simon", "000000000000000000000001") as CreatedResult;

        // Assert
        Assert.Equal(201, actual!.StatusCode);
        Assert.Null(actual.Value);
        Assert.Equal("", actual.Location);
    }

    [Fact]
    public async Task UnfollowUser_given_invalid_username_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = INVALID_USERNAME };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.FollowerService.UnfollowUserAsync("000000000000000000000001", "Test")).ReturnsAsync(new APIResponse(NotFound, INVALID_USERNAME));
        
        var controller = new FollowerController(serviceManager.Object, _logger.Object);
        controller.ControllerContext.HttpContext = CreateHttpContextWithClaims("000000000000000000000001");

        // Act
        var actual = await controller.UnfollowUser("Test", "000000000000000000000001") as NotFoundObjectResult;

        // Assert
        Assert.Equal(404, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task UnfollowUser_given_invalid_userId_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = INVALID_USER_ID };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.FollowerService.UnfollowUserAsync("000000000000000000000000", "Simon")).ReturnsAsync(new APIResponse(NotFound, INVALID_USER_ID));
        
        var controller = new FollowerController(serviceManager.Object, _logger.Object);
        controller.ControllerContext.HttpContext = CreateHttpContextWithClaims("000000000000000000000000");

        // Act
        var actual = await controller.UnfollowUser("Simon", "000000000000000000000000") as NotFoundObjectResult;

        // Assert
        Assert.Equal(404, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task UnfollowUser_given_same_username_and_userId_returns_BadRequest()
    {
        // Arrange
        var expected = new APIError { Status = 400, ErrorMsg = UNFOLLOW_SELF };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.FollowerService.UnfollowUserAsync("000000000000000000000001", "Gustav")).ReturnsAsync(new APIResponse(BadRequest, UNFOLLOW_SELF));
        
        var controller = new FollowerController(serviceManager.Object, _logger.Object);
        controller.ControllerContext.HttpContext = CreateHttpContextWithClaims("000000000000000000000001");

        // Act
        var actual = await controller.UnfollowUser("Gustav", "000000000000000000000001") as BadRequestObjectResult;

        // Assert
        Assert.Equal(400, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task UnfollowUser_given_different_UserId_in_claims_returns_Forbidden()
    {
        // Arrange
        var expected = new APIError { Status = 403, ErrorMsg = FORBIDDEN_OPERATION };

        var serviceManager = new Mock<IServiceManager>();
        var controller = new FollowerController(serviceManager.Object, _logger.Object);
        controller.ControllerContext.HttpContext = CreateHttpContextWithClaims("000000000000000000000000");

        // Act
        var actual = await controller.UnfollowUser("Gustav", "000000000000000000000001") as ForbiddenObjectResult;

        // Assert
        Assert.Equal(403, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    public DefaultHttpContext CreateHttpContextWithClaims(string userId)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }));

        return new DefaultHttpContext
        {
            User = principal
        };
    }
}