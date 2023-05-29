using FluentAssertions;
using MiniTwit.Core.Entities;
using MiniTwit.Core.Responses;
using MiniTwit.Infrastructure.Repositories;
using MongoDB.Driver;
using static MiniTwit.Core.Error.DBError;

namespace MiniTwit.Tests.Infrastructure.Tests.Repositories;

public class UserRepositoryTests : RepositoryTests
{
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        _repository = new UserRepository(Context);
    }

    [Fact]
    public async Task CreateAsync_given_taken_username_returns_UsernameTaken()
    {
        // Arrange
        var expected = new DBResult
        {
            DBError = USERNAME_TAKEN
        };

        // Act
        var actual = await _repository.CreateAsync("Gustav", "test@test.com", "password", "salt");
        var actualUsers = await Context.Users.Find(user => user.Username == "Gustav").ToListAsync();

        // Assert
        Assert.Equal(expected, actual);
        Assert.Single(actualUsers);
    }

    [Fact]
    public async Task CreateAsync_given_non_taken_username_returns_no_error_and_creates_User()
    {
        // Arrange
        var expected = new DBResult
        {
            DBError = null
        };

        // Act
        var actual = await _repository.CreateAsync("Test", "test@test.com", "password", "salt");
        var actualUser = await Context.Users.Find(user => user.Username == "Test").FirstOrDefaultAsync();

        // Assert
        Assert.Equal(expected, actual);
        Assert.NotNull(actualUser);
        Assert.Equal("Test", actualUser.Username);
        Assert.Equal("test@test.com", actualUser.Email);
        Assert.Equal("password", actualUser.Password);
        Assert.Equal("salt", actualUser.Salt);
    }

    [Fact]
    public async Task GetByUsernameAsync_given_invalid_username_returns_InvalidUsername()
    {
        // Arrange
        var expected = new DBResult<User>
        {
            DBError = INVALID_USERNAME,
            Model = null
        };

        // Act
        var actual = await _repository.GetByUsernameAsync("Test");

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetByUsernameAsync_given_valid_username_returns_user()
    {
        // Arrange
        var user = new User { Id = "000000000000000000000001", Username = "Gustav", Email = "test@test.com", Password = "password", Salt = "salt" };
        var expected = new DBResult<User>
        {
            DBError = null,
            Model = user
        };

        // Act
        var actual = await _repository.GetByUsernameAsync("Gustav");

        // Assert
        Assert.Null(actual.DBError);
        actual.Model.Should().BeEquivalentTo(user);
    }
}