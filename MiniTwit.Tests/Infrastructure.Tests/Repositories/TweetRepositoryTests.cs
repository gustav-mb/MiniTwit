using FluentAssertions;
using FluentAssertions.Extensions;
using MiniTwit.Core.Entities;
using MiniTwit.Core.Responses;
using MiniTwit.Infrastructure.Repositories;
using MongoDB.Driver;
using static MiniTwit.Core.Error.Errors;

namespace MiniTwit.Tests.Infrastructure.Tests.Repositories;

public class TweetRepositoryTests : RepositoryTests
{
    private readonly TweetRepository _repository;

    public TweetRepositoryTests()
    {
        _repository = new TweetRepository(Context);
    }

    [Fact]
    public async Task CreateAsync_given_invalid_authorId_returns_InvalidUserId()
    {
        // Arrange
        var expected = new DBResult
        {
            DBError = INVALID_USER_ID
        };

        // Act
        var actual = await _repository.CreateAsync("000000000000000000000000", "test");
        var actualTweet = await Context.Tweets.Find(tweet => tweet.AuthorId == "000000000000000000000000").FirstOrDefaultAsync();

        // Assert
        Assert.Equal(expected, actual);
        Assert.Null(actualTweet);
    }

    [Fact]
    public async Task CreateAsync_given_valid_authorId_returns_no_error_and_creates_Tweet()
    {
        // Arrange
        var expected = new DBResult
        {
            DBError = null
        };

        // Act
        var actual = await _repository.CreateAsync("000000000000000000000001", "test");
        var actualTweet = await Context.Tweets.Find(tweet => tweet.AuthorId == "000000000000000000000001" && tweet.Text == "test").FirstOrDefaultAsync();

        // Assert
        Assert.Equal(expected, actual);
        Assert.NotNull(actualTweet);
        Assert.Equal("000000000000000000000001", actualTweet.AuthorId);
        Assert.Equal("Gustav", actualTweet.AuthorName);
        Assert.Equal("test", actualTweet.Text);
        actualTweet.PubDate.Should().BeCloseTo(DateTime.UtcNow, 2000.Milliseconds());
        Assert.False(actualTweet.Flagged);
    }

    [Fact]
    public async Task GetAllByUsernameAsync_given_invalid_username_returns_InvalidUsername()
    {
        // Arrange
        var expected = new DBResult<IEnumerable<Tweet>>
        {
            DBError = INVALID_USERNAME,
            Model = null
        };

        // Act
        var actual = await _repository.GetAllByUsernameAsync("Test", null);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetAllByUsernameAsync_with_null_limit_given_valid_username_returns_all_Tweets()
    {
        // Act
        var actual = await _repository.GetAllByUsernameAsync("Gustav", null);

        // Assert
        Assert.Null(actual.DBError);
        Assert.NotNull(actual.Model);
        Assert.Collection(actual.Model,
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000003", AuthorId = "000000000000000000000001", AuthorName = "Gustav", Text = "Gustav's Flagged", PubDate = DateTime.Parse("01/01/2023 12:00:01").ToUniversalTime(), Flagged = true}),
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000001", AuthorId = "000000000000000000000001", AuthorName = "Gustav", Text = "Gustav's first tweet!", PubDate = DateTime.Parse("01/01/2023 12:00:00").ToUniversalTime(), Flagged = false }),
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000002", AuthorId = "000000000000000000000001", AuthorName = "Gustav", Text = "Gustav's second tweet!", PubDate = DateTime.Parse("01/01/2023 12:00:00").ToUniversalTime(), Flagged = false })
        );
    }

    [Theory]
    [InlineData(3, -4)]
    [InlineData(3, -3)]
    [InlineData(1, -1)]
    [InlineData(3, null)]
    [InlineData(3, 0)]
    [InlineData(1, 1)]
    [InlineData(3, 3)]
    [InlineData(3, 4)]
    public async Task GetAllByUsernameAsync_given_valid_username_and_limit_returns_limit_number_of_Tweets(int expected, int? limit)
    {
        // Act
        var actual = await _repository.GetAllByUsernameAsync("Gustav", limit);

        // Assert
        Assert.Null(actual.DBError);
        Assert.NotNull(actual.Model);
        Assert.Equal(expected, actual.Model.Count());
    }

    [Fact]
    public async Task GetAllNonFlaggedAsync_with_null_limit_returns_all_non_flagged_Tweets()
    {
        // Act
        var actual = await _repository.GetAllNonFlaggedAsync(null);

        // Assert
        Assert.Null(actual.DBError);
        Assert.NotNull(actual.Model);
        Assert.Collection(actual.Model,
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000008", AuthorId = "000000000000000000000003", AuthorName = "Nikolaj", Text = "Nikolaj2", PubDate = DateTime.Parse("01/01/2023 12:00:06").ToUniversalTime(), Flagged = false }),
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000007", AuthorId = "000000000000000000000003", AuthorName = "Nikolaj", Text = "Nikolaj1", PubDate = DateTime.Parse("01/01/2023 12:00:05").ToUniversalTime(), Flagged = false }),
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000006", AuthorId = "000000000000000000000002", AuthorName = "Simon", Text = "Simon's third tweet", PubDate = DateTime.Parse("01/01/2023 12:00:04").ToUniversalTime(), Flagged = false }),
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000005", AuthorId = "000000000000000000000002", AuthorName = "Simon", Text = "Simon's second tweet", PubDate = DateTime.Parse("01/01/2023 12:00:03").ToUniversalTime(), Flagged = false }),
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000004", AuthorId = "000000000000000000000002", AuthorName = "Simon", Text = "Simon's first tweet", PubDate = DateTime.Parse("01/01/2023 12:00:02").ToUniversalTime(), Flagged = false }),
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000010", AuthorId = "000000000000000000000004", AuthorName = "Victor", Text = "Victor2", PubDate = DateTime.Parse("01/01/2023 12:00:02").ToUniversalTime(), Flagged = false }),
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000009", AuthorId = "000000000000000000000004", AuthorName = "Victor", Text = "Victor1", PubDate = DateTime.Parse("01/01/2023 12:00:01").ToUniversalTime(), Flagged = false }),
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000001", AuthorId = "000000000000000000000001", AuthorName = "Gustav", Text = "Gustav's first tweet!", PubDate = DateTime.Parse("01/01/2023 12:00:00").ToUniversalTime(), Flagged = false }),
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000002", AuthorId = "000000000000000000000001", AuthorName = "Gustav", Text = "Gustav's second tweet!", PubDate = DateTime.Parse("01/01/2023 12:00:00").ToUniversalTime(), Flagged = false })
        );
    }

    [Theory]
    [InlineData(9, -10)]
    [InlineData(9, -9)]
    [InlineData(1, -1)]
    [InlineData(9, null)]
    [InlineData(9, 0)]
    [InlineData(1, 1)]
    [InlineData(9, 9)]
    [InlineData(9, 10)]
    public async Task GetAllNonFlaggedAsync_returns_limit_number_of_non_flagged_Tweets(int expected, int? limit)
    {
        // Act
        var actual = await _repository.GetAllNonFlaggedAsync(limit);

        // Assert
        Assert.Null(actual.DBError);
        Assert.NotNull(actual.Model);
        Assert.Equal(expected, actual.Model.Count());
    }

    [Fact]
    public async Task GetAllNonFlaggedFollowedByUserIdAsync_given_invalid_userId_returns_InvalidUserId()
    {
        // Arrange
        var expected = new DBResult<IEnumerable<Tweet>>
        {
            DBError = INVALID_USER_ID,
            Model = null
        };

        // Act
        var actual = await _repository.GetAllNonFlaggedFollowedByUserIdAsync("000000000000000000000000", null);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetAllNonFlaggedFollowedByUserIdAsync_given_valid_userId_returns_all_non_flagged_tweets_of_userId_and_its_followers()
    {
        // Act
        var actual = await _repository.GetAllNonFlaggedFollowedByUserIdAsync("000000000000000000000001", null);

        // Assert
        Assert.Null(actual.DBError);
        Assert.NotNull(actual.Model);
        Assert.Collection(actual.Model,
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000006", AuthorId = "000000000000000000000002", AuthorName = "Simon", Text = "Simon's third tweet", PubDate = DateTime.Parse("01/01/2023 12:00:04").ToUniversalTime(), Flagged = false }),
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000005", AuthorId = "000000000000000000000002", AuthorName = "Simon", Text = "Simon's second tweet", PubDate = DateTime.Parse("01/01/2023 12:00:03").ToUniversalTime(), Flagged = false }),
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000004", AuthorId = "000000000000000000000002", AuthorName = "Simon", Text = "Simon's first tweet", PubDate = DateTime.Parse("01/01/2023 12:00:02").ToUniversalTime(), Flagged = false }),
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000001", AuthorId = "000000000000000000000001", AuthorName = "Gustav", Text = "Gustav's first tweet!", PubDate = DateTime.Parse("01/01/2023 12:00:00").ToUniversalTime(), Flagged = false }),
            tweet => tweet.Should().BeEquivalentTo(new Tweet { Id = "000000000000000000000002", AuthorId = "000000000000000000000001", AuthorName = "Gustav", Text = "Gustav's second tweet!", PubDate = DateTime.Parse("01/01/2023 12:00:00").ToUniversalTime(), Flagged = false })
        );
    }

    [Theory]
    [InlineData(5, -6)]
    [InlineData(5, -5)]
    [InlineData(1, -1)]
    [InlineData(5, null)]
    [InlineData(5, 0)]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    [InlineData(5, 6)]
    public async Task GetAllNonFlaggedFollowedByUserIdAsync_given_valid_userId_and_limit_returns_limit_number_of_non_flagged_tweets_of_userId_and_its_followers(int expected, int? limit)
    {
        // Act
        var actual = await _repository.GetAllNonFlaggedFollowedByUserIdAsync("000000000000000000000001", limit);

        // Assert
        Assert.Null(actual.DBError);
        Assert.NotNull(actual.Model);
        Assert.Equal(expected, actual.Model.Count());
    }
}