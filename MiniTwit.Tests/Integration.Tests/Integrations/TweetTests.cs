using System.Net;
using System.Net.Http.Json;
using Xunit.Priority;
using FluentAssertions;
using MiniTwit.Core.DTOs;
using MiniTwit.Core.Error;
using static MiniTwit.Core.Error.Errors;

namespace MiniTwit.Tests.Integration.Tests.Integrations;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
[DefaultPriority(0)]
public class TweetTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TweetTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Timeline_given_invalid_userId_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = INVALID_USER_ID };

        // Act
        var actual = await _factory.CreateClient().GetAsync("/Tweet/?userId=000000000000000000000000");
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task Timeline_given_valid_userId_with_null_limit_returns_all_users_and_its_followers_Tweets()
    {
        // Act
        var actual = await _factory.CreateClient().GetAsync("/Tweet/?userId=000000000000000000000001");
        var content = await actual.Content.ReadFromJsonAsync<IEnumerable<TweetDTO>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.Collection(content!,
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000002", Text = "Simon's third tweet", PubDate = DateTime.Parse("01/01/2023 12:00:04") }),
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000002", Text = "Simon's second tweet", PubDate = DateTime.Parse("01/01/2023 12:00:03") }),
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000002", Text = "Simon's first tweet", PubDate = DateTime.Parse("01/01/2023 12:00:02") }),
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000001", Text = "Gustav's first tweet!", PubDate = DateTime.Parse("01/01/2023 12:00:00") }),
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000001", Text = "Gustav's second tweet!", PubDate = DateTime.Parse("01/01/2023 12:00:00") })
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
    public async Task Timeline_given_valid_userId_returns_users_and_its_followers_limit_number_of_Tweets(int expected, int? limit)
    {
        // Act
        var actual = await _factory.CreateClient().GetAsync($"/Tweet/?userId=000000000000000000000001&limit={limit}");
        var content = await actual.Content.ReadFromJsonAsync<IEnumerable<TweetDTO>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.Equal(expected, content!.Count());
    }

    [Fact]
    public async Task PublicTimeline_with_null_limit_returns_all_non_flagged_Tweets_and_Ok()
    {
        // Act
        var actual = await _factory.CreateClient().GetAsync("/Tweet/public");
        var content = await actual.Content.ReadFromJsonAsync<IEnumerable<TweetDTO>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.Collection(content!,
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000003", Text = "Nikolaj2", PubDate = DateTime.Parse("01/01/2023 12:00:06") }),
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000003", Text = "Nikolaj1", PubDate = DateTime.Parse("01/01/2023 12:00:05") }),
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000002", Text = "Simon's third tweet", PubDate = DateTime.Parse("01/01/2023 12:00:04") }),
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000002", Text = "Simon's second tweet", PubDate = DateTime.Parse("01/01/2023 12:00:03") }),
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000002", Text = "Simon's first tweet", PubDate = DateTime.Parse("01/01/2023 12:00:02") }),
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000004", Text = "Victor2", PubDate = DateTime.Parse("01/01/2023 12:00:02") }),
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000004", Text = "Victor1", PubDate = DateTime.Parse("01/01/2023 12:00:01") }),
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000001", Text = "Gustav's first tweet!", PubDate = DateTime.Parse("01/01/2023 12:00:00") }),
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000001", Text = "Gustav's second tweet!", PubDate = DateTime.Parse("01/01/2023 12:00:00") })
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
    public async Task PublicTimeline_returns_limit_number_of_non_flagged_Tweets_and_Ok(int expected, int? limit)
    {
        // Act
        var actual = await _factory.CreateClient().GetAsync($"/Tweet/public?limit={limit}");
        var content = await actual.Content.ReadFromJsonAsync<IEnumerable<TweetDTO>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.Equal(expected, content!.Count());
    }

    [Fact]
    public async Task UserTimeline_with_null_limit_given_valid_username_returns_all_users_Tweets_and_Ok()
    {
        // Act
        var actual = await _factory.CreateClient().GetAsync("/Tweet/Gustav");
        var content = await actual.Content.ReadFromJsonAsync<IEnumerable<TweetDTO>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.Collection(content!,
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000001", Text = "Gustav's Flagged", PubDate = DateTime.Parse("01/01/2023 12:00:01") }),
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000001", Text = "Gustav's first tweet!", PubDate = DateTime.Parse("01/01/2023 12:00:00") }),
            tweet => tweet.Should().BeEquivalentTo(new TweetDTO { AuthorId = "000000000000000000000001", Text = "Gustav's second tweet!", PubDate = DateTime.Parse("01/01/2023 12:00:00") })
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
    public async Task UserTimeline_given_valid_username_and_limit_returns_limit_number_of_Tweets_and_Ok(int expected, int? limit)
    {
        // Act
        var actual = await _factory.CreateClient().GetAsync($"/Tweet/Gustav?limit={limit}");
        var content = await actual.Content.ReadFromJsonAsync<IEnumerable<TweetDTO>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.Equal(expected, content!.Count());
    }

    [Fact]
    public async Task UserTimeline_with_null_limit_given_invalid_username_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = INVALID_USERNAME };

        // Act
        var actual = await _factory.CreateClient().GetAsync("/Tweet/Test");
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    public async Task AddMessage_given_invalid_AuthorId_returns_NotFound()
    {
        // Arrange
        var expected = new APIError { Status = 404, ErrorMsg = INVALID_USER_ID };

        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Tweet/add_message", new TweetCreateDTO { AuthorId = "000000000000000000000000", Text = "New Tweet" });
        var content = await actual.Content.ReadFromJsonAsync<APIError>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
        Assert.Equal(expected, content);
    }

    [Fact]
    [Priority(1)]
    public async Task AddMessage_given_valid_AuthorId_returns_Created()
    {
        // Act
        var actual = await _factory.CreateClient().PostAsJsonAsync("/Tweet/add_message", new TweetCreateDTO { AuthorId = "000000000000000000000001", Text = "New Tweet" });

        // Assert
        Assert.Equal(HttpStatusCode.Created, actual.StatusCode);
    }
}