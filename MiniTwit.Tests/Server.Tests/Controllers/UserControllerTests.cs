using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniTwit.Core.DTOs;
using MiniTwit.Core.Error;
using MiniTwit.Core.Responses;
using MiniTwit.Service;
using MiniTwit.Server.Controllers;
using static MiniTwit.Core.Error.Errors;
using static MiniTwit.Core.Responses.HTTPResponse;

namespace MiniTwit.Tests.Server.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<ILogger<UserController>> _logger;

    public UserControllerTests()
    {
        _logger = new Mock<ILogger<UserController>>();
    }

    [Fact]
    public async Task Register_given_taken_username_returns_Conflict()
    {
        // Arrange
        var expected = new APIError { Status = 409, ErrorMsg = USERNAME_TAKEN };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.UserService.RegisterUserAsync(new UserCreateDTO { Username = "Gustav", Password = "password", Email = "test@test.com" })).ReturnsAsync(new APIResponse(Conflict, USERNAME_TAKEN));
        var controller = new UserController(serviceManager.Object, _logger.Object);

        // Act
        var actual = await controller.Register(new UserCreateDTO { Username = "Gustav", Password = "password", Email = "test@test.com" }) as ConflictObjectResult;

        // Assert
        Assert.Equal(409, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Register_given_null_or_empty_username_returns_BadRequest(string username)
    {
        // Arrange
        var expected = new APIError { Status = 400, ErrorMsg = USERNAME_MISSING };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.UserService.RegisterUserAsync(new UserCreateDTO { Username = username, Password = "password", Email = "test@test.com" })).ReturnsAsync(new APIResponse(BadRequest, USERNAME_MISSING));
        var controller = new UserController(serviceManager.Object, _logger.Object);

        // Act
        var actual = await controller.Register(new UserCreateDTO { Username = username, Password = "password", Email = "test@test.com" }) as BadRequestObjectResult;

        // Assert
        Assert.Equal(400, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Theory]
    [InlineData("testtest.com")]
    [InlineData("")]
    [InlineData(null)]
    public async Task Register_given_null_malformed_or_empty_email_returns_BadRequest(string email)
    {
        // Arrange
        var expected = new APIError { Status = 400, ErrorMsg = EMAIL_MISSING_OR_INVALID };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.UserService.RegisterUserAsync(new UserCreateDTO { Username = "Gustav", Password = "password", Email = email })).ReturnsAsync(new APIResponse(BadRequest, EMAIL_MISSING_OR_INVALID));
        var controller = new UserController(serviceManager.Object, _logger.Object);

        // Act
        var actual = await controller.Register(new UserCreateDTO { Username = "Gustav", Password = "password", Email = email }) as BadRequestObjectResult;

        // Assert
        Assert.Equal(400, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Register_given_null_or_empty_password_returns_BadRequest(string password)
    {
        // Arrange
        var expected = new APIError { Status = 400, ErrorMsg = PASSWORD_MISSING };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.UserService.RegisterUserAsync(new UserCreateDTO { Username = "Gustav", Password = password, Email = "test@test.com" })).ReturnsAsync(new APIResponse(BadRequest, PASSWORD_MISSING));
        var controller = new UserController(serviceManager.Object, _logger.Object);

        // Act
        var actual = await controller.Register(new UserCreateDTO { Username = "Gustav", Password = password, Email = "test@test.com" }) as BadRequestObjectResult;

        // Assert
        Assert.Equal(400, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task Register_given_valid_user_information_returns_Created()
    {
        // Arrange
        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.UserService.RegisterUserAsync(new UserCreateDTO { Username = "Gustav", Password = "password", Email = "test@test.com" })).ReturnsAsync(new APIResponse(Created));
        var controller = new UserController(serviceManager.Object, _logger.Object);

        // Act
        var actual = await controller.Register(new UserCreateDTO { Username = "Gustav", Password = "password", Email = "test@test.com" }) as CreatedResult;

        // Assert
        Assert.Equal(201, actual!.StatusCode);
        Assert.Equal("", actual.Location);
        Assert.Null(actual.Value);
    }

    [Fact]
    public async Task Logout_returns_Ok()
    {
        // Arrange
        var serviceManager = new Mock<IServiceManager>();
        var controller = new UserController(serviceManager.Object, _logger.Object);

        // Act
        var actual = await controller.Logout() as OkResult;

        // Assert
        Assert.Equal(200, actual!.StatusCode);
    }
}