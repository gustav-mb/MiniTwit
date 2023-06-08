using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniTwit.Server.Controllers;
using MiniTwit.Service;
using MiniTwit.Core.DTOs;
using MiniTwit.Core.Error;
using MiniTwit.Core.Responses;
using static MiniTwit.Core.Error.Errors;
using static MiniTwit.Core.Responses.HTTPResponse;

namespace MiniTwit.Tests.Server.Tests.Controllers;

public class AuthenticationControllerTests
{
    private readonly Mock<ILogger<AuthenticationController>> _logger;

    public AuthenticationControllerTests()
    {
        _logger = new Mock<ILogger<AuthenticationController>>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Login_given_null_or_empty_username_returns_Unauthorized(string username)
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = USERNAME_MISSING };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.AuthenticationService.AuthenticateAsync(new LoginDTO { Username = username, Password = "password" })).ReturnsAsync(new APIResponse<TokenDTO>(Unauthorized, null, USERNAME_MISSING));

        var controller = new AuthenticationController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.Login(new LoginDTO { Username = username, Password = "password" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Login_given_null_or_empty_password_returns_Unauthorized(string password)
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = PASSWORD_MISSING };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.AuthenticationService.AuthenticateAsync(new LoginDTO { Username = "Gustav", Password = password })).ReturnsAsync(new APIResponse<TokenDTO>(Unauthorized, null, PASSWORD_MISSING));

        var controller = new AuthenticationController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.Login(new LoginDTO { Username = "Gustav", Password = password })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task Login_given_invalid_username_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = INVALID_USERNAME };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.AuthenticationService.AuthenticateAsync(new LoginDTO { Username = "Test", Password = "password" })).ReturnsAsync(new APIResponse<TokenDTO>(Unauthorized, null, INVALID_USERNAME));

        var controller = new AuthenticationController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.Login(new LoginDTO { Username = "Test", Password = "password" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task Login_given_valid_username_and_invalid_password_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = INVALID_PASSWORD };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.AuthenticationService.AuthenticateAsync(new LoginDTO { Username = "Gustav", Password = "wrong" })).ReturnsAsync(new APIResponse<TokenDTO>(Unauthorized, null, INVALID_PASSWORD));

        var controller = new AuthenticationController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.Login(new LoginDTO { Username = "Gustav", Password = "wrong" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task Login_given_valid_username_and_password_returns_Ok()
    {
        // Arrange
        var expected = new TokenDTO { AccessToken = "access", RefreshToken = "refresh" };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.AuthenticationService.AuthenticateAsync(new LoginDTO { Username = "Gustav", Password = "wrong" })).ReturnsAsync(new APIResponse<TokenDTO>(Ok, new TokenDTO { AccessToken = "access", RefreshToken = "refresh" }));

        var controller = new AuthenticationController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.Login(new LoginDTO { Username = "Gustav", Password = "wrong" })).Result as OkObjectResult;

        // Assert
        Assert.Equal(200, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task RefreshToken_given_no_NameIdentifier_claim_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = INVALID_TOKEN };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.AuthenticationService.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).ReturnsAsync(new APIResponse<TokenDTO>(Unauthorized, null, INVALID_TOKEN));

        var controller = new AuthenticationController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.RefreshToken(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task RefreshToken_given_access_token_not_expired_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = TOKEN_NOT_EXPIRED };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.AuthenticationService.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).ReturnsAsync(new APIResponse<TokenDTO>(Unauthorized, null, TOKEN_NOT_EXPIRED));

        var controller = new AuthenticationController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.RefreshToken(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task RefreshToken_given_non_existing_refresh_token_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = INVALID_TOKEN };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.AuthenticationService.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).ReturnsAsync(new APIResponse<TokenDTO>(Unauthorized, null, INVALID_TOKEN));

        var controller = new AuthenticationController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.RefreshToken(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task RefreshToken_given_expired_refresh_token_returns_Unathorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = TOKEN_EXPIRED };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.AuthenticationService.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).ReturnsAsync(new APIResponse<TokenDTO>(Unauthorized, null, TOKEN_EXPIRED));

        var controller = new AuthenticationController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.RefreshToken(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task RefreshToken_given_refresh_token_invalidated_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = TOKEN_INVALIDATED };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.AuthenticationService.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).ReturnsAsync(new APIResponse<TokenDTO>(Unauthorized, null, TOKEN_INVALIDATED));

        var controller = new AuthenticationController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.RefreshToken(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task RefreshToken_given_refresh_token_already_used_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = TOKEN_USED };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.AuthenticationService.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).ReturnsAsync(new APIResponse<TokenDTO>(Unauthorized, null, TOKEN_USED));

        var controller = new AuthenticationController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.RefreshToken(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task RefreshToken_given_access_and_different_refresh_token_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = INVALID_TOKEN };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.AuthenticationService.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).ReturnsAsync(new APIResponse<TokenDTO>(Unauthorized, null, INVALID_TOKEN));

        var controller = new AuthenticationController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.RefreshToken(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task RefreshToken_given_invalid_userId_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = INVALID_USER_ID };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.AuthenticationService.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).ReturnsAsync(new APIResponse<TokenDTO>(Unauthorized, null, INVALID_USER_ID));

        var controller = new AuthenticationController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.RefreshToken(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).Result as UnauthorizedObjectResult;

        // Assert
        Assert.Equal(401, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task RefreshToken_given_valid_access_and_refresh_token_returns_Ok()
    {
        // Arrange
        var expected = new TokenDTO { AccessToken = "newAccess", RefreshToken = "newRefresh" };

        var serviceManager = new Mock<IServiceManager>();
        serviceManager.Setup(sm => sm.AuthenticationService.RefreshTokenAsync(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).ReturnsAsync(new APIResponse<TokenDTO>(Ok, new TokenDTO { AccessToken = "newAccess", RefreshToken = "newRefresh" }));

        var controller = new AuthenticationController(serviceManager.Object, _logger.Object);

        // Act
        var actual = (await controller.RefreshToken(new TokenDTO { AccessToken = "access", RefreshToken = "refresh" })).Result as OkObjectResult;

        // Assert
        Assert.Equal(200, actual!.StatusCode);
        Assert.Equal(expected, actual.Value);
    }
}