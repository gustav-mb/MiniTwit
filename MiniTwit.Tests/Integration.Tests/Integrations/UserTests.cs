using System.Net;
using System.Net.Http.Json;
using MiniTwit.Core.DTOs;
using MiniTwit.Core.Error;
using static MiniTwit.Core.Error.Errors;

namespace MiniTwit.Tests.Integration.Tests.Integrations;

public class UserTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public UserTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_given_taken_username_returns_Conflict()
    {
        // Arrange
        var expected = new APIError { Status = 409, ErrorMsg = USERNAME_TAKEN };

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
        var expected = new APIError { Status = 400, ErrorMsg = USERNAME_MISSING };

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
        var expected = new APIError { Status = 400, ErrorMsg = EMAIL_MISSING_OR_INVALID };

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
        var expected = new APIError { Status = 400, ErrorMsg = PASSWORD_MISSING };

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