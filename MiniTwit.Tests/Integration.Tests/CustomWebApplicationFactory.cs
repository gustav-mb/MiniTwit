using EphemeralMongo;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using MiniTwit.Infrastructure;
using MiniTwit.Core.MongoDB.DependencyInjection;
using MiniTwit.Core.Entities;
using MiniTwit.Core;
using MiniTwit.Security.Hashing;
using MiniTwit.Security.Authentication;

namespace MiniTwit.Tests.Integration.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly IMongoRunner _runner;
    public JwtSettings JwtSettings { private set; get; } = null!;

    public CustomWebApplicationFactory()
    {
        var options = new MongoRunnerOptions
        {
            AdditionalArguments = "--quiet",
            KillMongoProcessesWhenCurrentProcessExits = true
        };

        _runner = MongoRunner.Run(options);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Integration");

        builder.ConfigureServices(services =>
        {
            var mongoContext = services.SingleOrDefault(mc => mc.ServiceType == typeof(MiniTwitContext));

            if (mongoContext != null)
            {
                services.Remove(mongoContext);
            }

            JwtSettings = new JwtSettings
            {
                Issuer = "Issuer",
                Audience = "Audience",
                Key = "3d4ab1fc-3cf8-489e-b342-cb2f64ca20ee",
                TokenExpiryMin = 5,
                RefreshTokenExpiryMin = 60
            };

            services.Configure<JwtSettings>(options =>
            {
                options.Issuer = JwtSettings.Issuer;
                options.Audience = JwtSettings.Audience;
                options.Key = JwtSettings.Key;
                options.TokenExpiryMin = JwtSettings.TokenExpiryMin;
                options.RefreshTokenExpiryMin = JwtSettings.RefreshTokenExpiryMin;
            });

            services.AddMongoContext<IMiniTwitContext, MiniTwitContext>(options =>
            {
                options.ConnectionString = _runner.ConnectionString;
                options.DatabaseName = "MiniTwitTest";
            });

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var appContext = scope.ServiceProvider.GetRequiredService<IMiniTwitContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<IHasher>();

            Seed(appContext, hasher);
        });

        return base.CreateHost(builder);
    }

    private void Seed(IMiniTwitContext context, IHasher hasher)
    {
        if (context.Users.CountDocuments(new BsonDocument()) > 0)
        {
            return;
        }

        var hashResult = hasher.Hash("password");

        // Users
        var u1 = new User
        {
            Id = "000000000000000000000001",
            Username = "Gustav",
            Email = "test@test.com",
            Password = hashResult.Hash,
            Salt = hashResult.Salt
        };

        var u2 = new User
        {
            Id = "000000000000000000000002",
            Username = "Simon",
            Email = "test@test.com",
            Password = hashResult.Hash,
            Salt = hashResult.Salt
        };

        var u3 = new User
        {
            Id = "000000000000000000000003",
            Username = "Nikolaj",
            Email = "test@test.com",
            Password = hashResult.Hash,
            Salt = hashResult.Salt
        };

        var u4 = new User
        {
            Id = "000000000000000000000004",
            Username = "Victor",
            Email = "test@test.com",
            Password = hashResult.Hash,
            Salt = hashResult.Salt
        };

        context.Users.InsertMany(new[] { u1, u2, u3, u4 });

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

        context.Tweets.InsertMany(new[] { m1, m2, m3, m4, m5, m6, m7, m8, m9, m10 });

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

        context.Followers.InsertMany(new[] { f1, f2, f3, f4 });

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
            ExpiryTime = DateTime.UtcNow.AddMinutes(JwtSettings.RefreshTokenExpiryMin),
            Used = false,
            Invalidated = false
        };

        var rt3 = new RefreshToken
        {
            JwtId = "940890ef-5aff-4946-94b6-d6336979dabf",
            Token = "00000000000000000000000000000003",
            UserId = "000000000000000000000001",
            ExpiryTime = DateTime.UtcNow.AddMinutes(JwtSettings.RefreshTokenExpiryMin),
            Used = false,
            Invalidated = true
        };

        var rt4 = new RefreshToken
        {
            JwtId = "b8a21733-a66b-4c01-829a-aafe09638fa7",
            Token = "00000000000000000000000000000004",
            UserId = "000000000000000000000001",
            ExpiryTime = DateTime.UtcNow.AddMinutes(JwtSettings.RefreshTokenExpiryMin),
            Used = true,
            Invalidated = false
        };

        context.RefreshTokens.InsertMany(new[] { rt1, rt2, rt3, rt4 });
    }
}