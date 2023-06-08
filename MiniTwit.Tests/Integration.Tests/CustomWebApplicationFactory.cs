using EphemeralMongo;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using MiniTwit.Infrastructure;
using MiniTwit.Security.Hashing;
using MiniTwit.Core.MongoDB.DependencyInjection;
using MiniTwit.Core.Entities;
using MiniTwit.Core;

namespace MiniTwit.Tests.Integration.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly IMongoRunner _runner;

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
        builder.ConfigureServices(services =>
        {
            var mongoContext = services.SingleOrDefault(mc => mc.ServiceType == typeof(MiniTwitContext));

            if (mongoContext != null)
            {
                services.Remove(mongoContext);
            }

            // Add Test Scheme defined in TestAuthHandler
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
                options.DefaultScheme = "Test";
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

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

        builder.UseEnvironment("Integration");

        return base.CreateHost(builder);
    }

    private void Seed(IMiniTwitContext context, IHasher hasher)
    {
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
            PubDate = DateTime.Parse("01/01/2023 12:00:00", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
            Flagged = false
        };

        var m2 = new Tweet
        {
            Id = "000000000000000000000002",
            AuthorId = u1.Id,
            Text = "Gustav's second tweet!",
            PubDate = DateTime.Parse("01/01/2023 12:00:00", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
            Flagged = false
        };

        var m3 = new Tweet
        {
            Id = "000000000000000000000003",
            AuthorId = u1.Id,
            Text = "Gustav's Flagged",
            PubDate = DateTime.Parse("01/01/2023 12:00:01", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
            Flagged = true
        };

        var m4 = new Tweet
        {
            Id = "000000000000000000000004",
            AuthorId = u2.Id,
            Text = "Simon's first tweet",
            PubDate = DateTime.Parse("01/01/2023 12:00:02", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
            Flagged = false
        };

        var m5 = new Tweet
        {
            Id = "000000000000000000000005",
            AuthorId = u2.Id,
            Text = "Simon's second tweet",
            PubDate = DateTime.Parse("01/01/2023 12:00:03", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
            Flagged = false
        };

        var m6 = new Tweet
        {
            Id = "000000000000000000000006",
            AuthorId = u2.Id,
            Text = "Simon's third tweet",
            PubDate = DateTime.Parse("01/01/2023 12:00:04", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
            Flagged = false
        };

        var m7 = new Tweet
        {
            Id = "000000000000000000000007",
            AuthorId = u3.Id,
            Text = "Nikolaj1",
            PubDate = DateTime.Parse("01/01/2023 12:00:05", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
            Flagged = false
        };

        var m8 = new Tweet
        {
            Id = "000000000000000000000008",
            AuthorId = u3.Id,
            Text = "Nikolaj2",
            PubDate = DateTime.Parse("01/01/2023 12:00:06", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
            Flagged = false
        };

        var m9 = new Tweet
        {
            Id = "000000000000000000000009",
            AuthorId = u4.Id,
            Text = "Victor1",
            PubDate = DateTime.Parse("01/01/2023 12:00:01", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
            Flagged = false
        };

        var m10 = new Tweet
        {
            Id = "000000000000000000000010",
            AuthorId = u4.Id,
            Text = "Victor2",
            PubDate = DateTime.Parse("01/01/2023 12:00:02", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
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
    }
}