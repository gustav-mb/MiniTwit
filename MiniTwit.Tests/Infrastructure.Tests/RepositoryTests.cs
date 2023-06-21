using System.Globalization;
using EphemeralMongo;
using MongoDB.Driver;
using MiniTwit.Core.Entities;
using MiniTwit.Infrastructure;
using MiniTwit.Core.MongoDB.Builders;

namespace MiniTwit.Tests.Infrastructure.Tests;

public class RepositoryTests : IDisposable
{
    protected readonly MiniTwitContext Context;
    private readonly IMongoRunner _runner;

    public RepositoryTests()
    {
        var options = new MongoRunnerOptions
        {
            AdditionalArguments = "--quiet",
            KillMongoProcessesWhenCurrentProcessExits = true
        };

        _runner = MongoRunner.Run(options);

        var database = new MongoClient(_runner.ConnectionString).GetDatabase("MiniTwit");
        var builder = new MongoContextOptionsBuilder(database);
        Context = new MiniTwitContext(builder);

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Users
        var u1 = new User
        {
            Id = "000000000000000000000001",
            Username = "Gustav",
            Email = "test@test.com",
            Password = "password",
            Salt = "salt"
        };

        var u2 = new User
        {
            Id = "000000000000000000000002",
            Username = "Simon",
            Email = "test@test.com",
            Password = "password",
            Salt = "salt"
        };

        var u3 = new User
        {
            Id = "000000000000000000000003",
            Username = "Nikolaj",
            Email = "test@test.com",
            Password = "password",
            Salt = "salt"
        };

         var u4 = new User
        {
            Id = "000000000000000000000004",
            Username = "Victor",
            Email = "test@test.com",
            Password = "password",
            Salt = "salt"
        };

        Context.Users.InsertMany(new[] { u1, u2, u3, u4 });

        // Messages
        var m1 = new Tweet
        {
            Id = "000000000000000000000001",
            AuthorId = u1.Id,
            Text = "Gustav's first tweet!",
            PubDate = DateTime.Parse("01/01/2023 12:00:00").ToUniversalTime(),
            Flagged = false
        };

        var m2 = new Tweet
        {
            Id = "000000000000000000000002",
            AuthorId = u1.Id,
            Text = "Gustav's second tweet!",
            PubDate = DateTime.Parse("01/01/2023 12:00:00").ToUniversalTime(),
            Flagged = false
        };

        var m3 = new Tweet
        {
            Id = "000000000000000000000003",
            AuthorId = u1.Id,
            Text = "Gustav's Flagged",
            PubDate = DateTime.Parse("01/01/2023 12:00:01").ToUniversalTime(),
            Flagged = true
        };

        var m4 = new Tweet
        {
            Id = "000000000000000000000004",
            AuthorId = u2.Id,
            Text = "Simon's first tweet",
            PubDate = DateTime.Parse("01/01/2023 12:00:02").ToUniversalTime(),
            Flagged = false
        };

        var m5 = new Tweet
        {
            Id = "000000000000000000000005",
            AuthorId = u2.Id,
            Text = "Simon's second tweet",
            PubDate = DateTime.Parse("01/01/2023 12:00:03").ToUniversalTime(),
            Flagged = false
        };

        var m6 = new Tweet
        {
            Id = "000000000000000000000006",
            AuthorId = u2.Id,
            Text = "Simon's third tweet",
            PubDate = DateTime.Parse("01/01/2023 12:00:04").ToUniversalTime(),
            Flagged = false
        };

        var m7 = new Tweet
        {
            Id = "000000000000000000000007",
            AuthorId = u3.Id,
            Text = "Nikolaj1",
            PubDate = DateTime.Parse("01/01/2023 12:00:05").ToUniversalTime(),
            Flagged = false
        };

        var m8 = new Tweet
        {
            Id = "000000000000000000000008",
            AuthorId = u3.Id,
            Text = "Nikolaj2",
            PubDate = DateTime.Parse("01/01/2023 12:00:06").ToUniversalTime(),
            Flagged = false
        };

        var m9 = new Tweet
        {
            Id = "000000000000000000000009",
            AuthorId = u4.Id,
            Text = "Victor1",
            PubDate = DateTime.Parse("01/01/2023 12:00:01").ToUniversalTime(),
            Flagged = false
        };

        var m10 = new Tweet
        {
            Id = "000000000000000000000010",
            AuthorId = u4.Id,
            Text = "Victor2",
            PubDate = DateTime.Parse("01/01/2023 12:00:02").ToUniversalTime(),
            Flagged = false
        };

        Context.Tweets.InsertMany(new[] { m1, m2, m3, m4, m5, m6, m7, m8, m9, m10 });

        // Followers
        var f1 = new Follower
        {
            WhoId = u1.Id,
            WhomId = u2.Id
        };

        var f2 = new Follower
        {
            WhoId = u2.Id,
            WhomId = u3.Id
        };

        var f3 = new Follower
        {
            WhoId = u2.Id,
            WhomId = u4.Id
        };

        var f4 = new Follower
        {
            WhoId = u4.Id,
            WhomId = u1.Id
        };

        Context.Followers.InsertMany(new[] { f1, f2, f3, f4 });

        // Refresh Tokens
        var rt1 = new RefreshToken
        {
            JwtId = "3a096f60-2ab0-4ecb-9627-1f02a22cccbd",
            Token = "00000000000000000000000000000001",
            UserId = "000000000000000000000001",
            ExpiryTime = DateTime.Parse("01/01/2023 12:00:00").ToUniversalTime(),
            Used = false,
            Invalidated = false
        };

        var rt2 = new RefreshToken
        {
            JwtId = "3fc85c49-6dce-4e66-abad-e3b7c1aea29e",
            Token = "00000000000000000000000000000002",
            UserId = "000000000000000000000001",
            ExpiryTime = DateTime.UtcNow.AddYears(1),
            Used = false,
            Invalidated = false
        };

        Context.RefreshTokens.InsertMany(new[] { rt1, rt2 });
    }

    [Fact]
    public void RepositoryTests_constructor_works()
    {
        Assert.True(true);
    }

    public void Dispose()
    {
        _runner.Dispose();
    }
}