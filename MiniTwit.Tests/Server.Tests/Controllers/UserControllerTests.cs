using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniTwit.Core.DTOs;
using MiniTwit.Core.Error;
using MiniTwit.Core.Responses;
using MiniTwit.Service;
using MiniTwit.Server.Controllers;
using static MiniTwit.Core.Error.DBError;
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
    public async Task Login_given_valid_credentials_returns_Ok()
    {
        // Arrange
        var expected = new UserDTO { Id = "000000000000000000000001", Username = "Gustav", Email = "test@test.com" };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.UserService.AuthenticateAsync(new LoginDTO { Username = "Gustav", Password = "password" })).ReturnsAsync(new APIResponse<UserDTO>(Ok, new UserDTO { Id = "000000000000000000000001", Username = "Gustav", Email = "test@test.com" }, null));
        var controller = new UserController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.Login(new LoginDTO { Username = "Gustav", Password = "password" })).Result as OkObjectResult;

        // Assert
        Assert.Equal(200, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task Login_given_invalid_username_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Invalid username" };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.UserService.AuthenticateAsync(new LoginDTO { Username = "Test", Password = "password" })).ReturnsAsync(new APIResponse<UserDTO>(Unauthorized, null, INVALID_USERNAME));
        var controller = new UserController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.Login(new LoginDTO { Username = "Test", Password = "password" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task Login_given_empty_username_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Username is missing" };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.UserService.AuthenticateAsync(new LoginDTO { Username = "", Password = "password" })).ReturnsAsync(new APIResponse<UserDTO>(Unauthorized, null, USERNAME_MISSING));
        var controller = new UserController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.Login(new LoginDTO { Username = "", Password = "password" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task Login_given_empty_password_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Password is missing" };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.UserService.AuthenticateAsync(new LoginDTO { Username = "Gustav", Password = "" })).ReturnsAsync(new APIResponse<UserDTO>(Unauthorized, null, PASSWORD_MISSING));
        var controller = new UserController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.Login(new LoginDTO { Username = "Gustav", Password = "" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task Login_given_invalid_password_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Invalid password" };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.UserService.AuthenticateAsync(new LoginDTO { Username = "Gustav", Password = "wrong" })).ReturnsAsync(new APIResponse<UserDTO>(Unauthorized, null, INVALID_PASSWORD));
        var controller = new UserController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.Login(new LoginDTO { Username = "Gustav", Password = "wrong" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task Register_given_taken_username_returns_Conflict()
    {
        // Arrange
        var expected = new APIError { Status = 409, ErrorMsg = "Username is already taken" };

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
        var expected = new APIError { Status = 400, ErrorMsg = "Username is missing" };

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
        var expected = new APIError { Status = 400, ErrorMsg = "Email is missing or invalid" };

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
        var expected = new APIError { Status = 400, ErrorMsg = "Password is missing" };

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
        serviceManager.Setup(sm => sm.UserService.RegisterUserAsync(new UserCreateDTO { Username = "Gustav", Password = "password", Email = "test@test.com" })).ReturnsAsync(new APIResponse(Created, null));
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