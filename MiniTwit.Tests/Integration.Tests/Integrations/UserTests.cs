using System.Net;
using System.Net.Http.Json;
using MiniTwit.Core.DTOs;
using MiniTwit.Core.Error;

namespace MiniTwit.Tests.Integration.Tests.Integrations;

public class UserTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public UserTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_given_valid_credentials_returns_User_and_Ok()
    {
        // Arrange
        var expected = new UserDTO { Id = "000000000000000000000001", Username = "Gustav", Email = "test@test.com" };

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/User/login", new LoginDTO { Username = "Gustav", Password = "password" });
        var content = await actual.Content.ReadFromJsonAsync<UserDTO>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task Login_given_invalid_username_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Invalid username" };

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/User/login", new LoginDTO { Username = "Test", Password = "password" });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Login_given_null_or_empty_username_returns_Unauthorized(string username)
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Username is missing" };

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/User/login", new LoginDTO { Username = username, Password = "password" });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task Login_given_invalid_password_returns_Unauthorized()
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Invalid password" };

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/User/login", new LoginDTO { Username = "Gustav", Password = "wrong" });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Login_given_null_or_empty_password_returns_Unauthorized(string password)
    {
        // Arrange
        var expected = new APIError { Status = 401, ErrorMsg = "Password is missing" };

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/User/login", new LoginDTO { Username = "Gustav", Password = password });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task Register_given_taken_username_returns_Conflict()
    {
        // Arrange
        var expected = new APIError { Status = 409, ErrorMsg = "Username is already taken" };

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/User/register", new UserCreateDTO { Username = "Gustav", Password = "password", Email = "test@test.com" });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Register_given_null_or_empty_username_returns_BadRequest(string username)
    {
        // Arrange
        var expected = new APIError { Status = 400, ErrorMsg = "Username is missing" };

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/User/register", new UserCreateDTO { Username = username, Password = "password", Email = "test@test.com" });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("testtest.com")]
    public async Task Register_given_null_empty_or_malformed_email_returns_BadRequest(string email)
    {
        // Arrange
        var expected = new APIError { Status = 400, ErrorMsg = "Email is missing or invalid" };

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/User/register", new UserCreateDTO { Username = "Test", Password = "password", Email = email });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Register_given_null_or_empty_password_returns_BadRequest(string password)
    {
        // Arrange
        var expected = new APIError { Status = 400, ErrorMsg = "Password is missing" };

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/User/register", new UserCreateDTO { Username = "Test", Password = password, Email = "test@test.com" });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task Register_given_valid_credentials_returns_Created()
    {
        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/User/register", new UserCreateDTO { Username = "Tester", Password = "password", Email = "test@test.com" });

        // Assert
        Assert.Equal(HttpStatusCode.Created, actual.StatusCode);
    }

    [Fact]
    public async Task Logout_returns_Ok()
    {
        // Act
        var actual = await _factory.CreateClient().PostAsync("/User/logout", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
    }
}