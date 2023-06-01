using Moq;
using MiniTwit.Core.IRepositories;
using MiniTwit.Core.Responses;
using MiniTwit.Security.Hashing;
using MiniTwit.Service.Services;
using MiniTwit.Core.Entities;
using MiniTwit.Core.DTOs;
using static MiniTwit.Core.Error.DBError;
using static MiniTwit.Core.Responses.HTTPResponse;

namespace MiniTwit.Tests.Service.Tests.Services;

public class UserServiceTests
{
    private Mock<IHasher> _hasher;
    private CancellationToken _ct;
    
    public UserServiceTests()
    {
        _hasher = new Mock<IHasher>();
        _ct = new CancellationToken();
    }

    [Fact]
    public async Task AuthenticateAsync_given_invalid_username_returns_Unauthorized_with_InvalidUsername()
    {
        // Arrange
        var expected = new APIResponse<UserDTO>(Unauthorized, null, INVALID_USERNAME);

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByUsernameAsync("Test", _ct)).ReturnsAsync(new DBResult<User>{ Model = null, DBError = INVALID_USERNAME });
        var service = new UserService(repository.Object, _hasher.Object);

        // Act
        var actual = await service.AuthenticateAsync(new LoginDTO { Username = "Test", Password = "password" });
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task AuthenticateAsync_given_empty_username_returns_Unauthorized_with_UsernameMissing()
    {
        // Arrange
        var expected = new APIResponse<UserDTO>(Unauthorized, null, USERNAME_MISSING);

        var repository = new Mock<IUserRepository>();
        var service = new UserService(repository.Object, _hasher.Object);

        // Act
        var actual = await service.AuthenticateAsync(new LoginDTO { Username = "", Password = "password" });
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task AuthenticateAsync_given_empty_password_returns_Unauthorized_with_Password_Missing()
    {
        // Arrange
        var expected = new APIResponse<UserDTO>(Unauthorized, null, PASSWORD_MISSING);

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByUsernameAsync("Gustav", _ct)).ReturnsAsync(new DBResult<User>{ Model = new User { Id = "000000000000000000000001", Username = "Gustav", Email = "test@test.com", Password = "password", Salt = "salt" }, DBError = null });
        var service = new UserService(repository.Object, _hasher.Object);

        // Act
        var actual = await service.AuthenticateAsync(new LoginDTO { Username = "Gustav", Password = "" });
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task AuthenticateAsync_given_wrong_password_returns_Unauthorized_with_InvalidPassword()
    {
        // Arrange
        var expected = new APIResponse<UserDTO>(Unauthorized, null, INVALID_PASSWORD);

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByUsernameAsync("Gustav", _ct)).ReturnsAsync(new DBResult<User>{ Model = new User { Id = "000000000000000000000001", Username = "Gustav", Email = "test@test.com", Password = "password", Salt = "salt" }, DBError = null });
        _hasher.Setup(h => h.VerifyHashAsync("wrong", "password", "salt")).ReturnsAsync(false);
        var service = new UserService(repository.Object, _hasher.Object);

        // Act
        var actual = await service.AuthenticateAsync(new LoginDTO { Username = "Gustav", Password = "wrong" });
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task AuthenticateAsync_given_correct_credentials_returns_Ok_and_User()
    {
        // Arrange
        var expected = new APIResponse<UserDTO>(Ok, new UserDTO{ Id = "000000000000000000000001", Username = "Gustav", Email = "test@test.com" }, null);

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByUsernameAsync("Gustav", _ct)).ReturnsAsync(new DBResult<User>{ Model = new User { Id = "000000000000000000000001", Username = "Gustav", Email = "test@test.com", Password = "password", Salt = "salt" }, DBError = null });
        _hasher.Setup(h => h.VerifyHashAsync("password", "password", "salt")).ReturnsAsync(true);
        var service = new UserService(repository.Object, _hasher.Object);

        // Act
        var actual = await service.AuthenticateAsync(new LoginDTO { Username = "Gustav", Password = "password" });
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_given_invalid_username_returns_NotFound_with_InvalidUsername()
    {
        // Arrange
        var expected = new APIResponse<UserDTO>(NotFound, null, INVALID_USERNAME);

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByUsernameAsync("Test", _ct)).ReturnsAsync(new DBResult<User>{ DBError = INVALID_USERNAME });
        var service = new UserService(repository.Object, _hasher.Object);

        // Act
        var actual = await service.GetUserByUsernameAsync("Test", _ct);
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_given_valid_username_returns_Ok_and_User()
    {
        // Arrange
        var expected = new APIResponse<UserDTO>(Ok, new UserDTO{ Id = "000000000000000000000001", Username = "Gustav", Email = "test@test.com" }, null);

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByUsernameAsync("Gustav", _ct)).ReturnsAsync(new DBResult<User>{ Model = new User { Id = "000000000000000000000001", Username = "Gustav", Email = "test@test.com", Password = "password", Salt = "salt" }, DBError = null });
        var service = new UserService(repository.Object, _hasher.Object);

        // Act
        var actual = await service.GetUserByUsernameAsync("Gustav");
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RegisterUserAsync_given_already_taken_username_returns_Conflict_with_UsernameTaken()
    {
        // Arrange
        var expected = new APIResponse(Conflict, USERNAME_TAKEN);

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByUsernameAsync("Gustav", _ct)).ReturnsAsync(new DBResult<User>{ Model = new User { Id = "000000000000000000000001", Username = "Gustav", Email = "test@test.com", Password = "password", Salt = "salt" }, DBError = USERNAME_TAKEN });
        var service = new UserService(repository.Object, _hasher.Object);

        // Act
        var actual = await service.RegisterUserAsync(new UserCreateDTO { Username = "Gustav", Email = "test@test.com", Password = "password" });
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task RegisterUserAsync_given_null_or_empty_username_returns_BadRequest_with_UsernameMissing(string username)
    {
        // Arrange
        var expected = new APIResponse(BadRequest, USERNAME_MISSING);

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByUsernameAsync(username, _ct)).ReturnsAsync(new DBResult<User>{ Model = null, DBError = INVALID_USERNAME });
        var service = new UserService(repository.Object, _hasher.Object);

        // Act
        var actual = await service.RegisterUserAsync(new UserCreateDTO { Username = username, Email = "test@test.com", Password = "password" });
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("testtest.com")]
    [InlineData("")]
    [InlineData(null)]
    public async Task RegisterUserAsync_given_null_empty_or_invalid_email_returns_BadRequest_with_EmailMissingOrInvalid(string email)
    {
        // Arrange
        var expected = new APIResponse(BadRequest, EMAIL_MISSING_OR_INVALID);

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByUsernameAsync("Gustav", _ct)).ReturnsAsync(new DBResult<User>{ Model = null, DBError = INVALID_USERNAME });
        var service = new UserService(repository.Object, _hasher.Object);

        // Act
        var actual = await service.RegisterUserAsync(new UserCreateDTO { Username = "Gustav", Email = email, Password = "password" });
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task RegisterUserAsync_given_null_or_empty_password_returns_BadRequest_with_PasswordMissing(string password)
    {
        // Arrange
        var expected = new APIResponse(BadRequest, PASSWORD_MISSING);

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByUsernameAsync("Gustav", _ct)).ReturnsAsync(new DBResult<User>{ Model = null, DBError = INVALID_USERNAME });
        var service = new UserService(repository.Object, _hasher.Object);

        // Act
        var actual = await service.RegisterUserAsync(new UserCreateDTO { Username = "Gustav", Email = "test@test.com", Password = password });
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task RegisterUserAsync_given_valid_user_information_returns_Created()
    {
        // Arrange
        var expected = new APIResponse(Created, null);

        var repository = new Mock<IUserRepository>();
        repository.Setup(r => r.GetByUsernameAsync("Gustav", _ct)).ReturnsAsync(new DBResult<User>{ Model = null, DBError = INVALID_USERNAME });
        _hasher.Setup(h => h.HashAsync("password")).ReturnsAsync(("password", "salt"));
        var service = new UserService(repository.Object, _hasher.Object);

        // Act
        var actual = await service.RegisterUserAsync(new UserCreateDTO { Username = "Gustav", Email = "test@test.com", Password = "password" });
        
        // Assert
        Assert.Equal(expected, actual);
    }
}