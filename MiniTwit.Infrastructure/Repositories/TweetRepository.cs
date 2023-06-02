using MongoDB.Driver;
using MiniTwit.Core;
using MiniTwit.Core.Entities;
using MiniTwit.Core.IRepositories;
using MiniTwit.Core.Responses;
using static MiniTwit.Core.Error.DBError;

namespace MiniTwit.Infrastructure.Repositories;

public class TweetRepository : ITweetRepository
{
    private readonly IMiniTwitContext _context;

    public TweetRepository(IMiniTwitContext context)
    {
        _context = context;
    }

    public async Task<DBResult> CreateAsync(string authorId, string text)
    {
        var user = await GetUserByUserIdAsync(authorId);

        // User does not exist
        if (user == null)
        {
            return new DBResult
            {
                DBError = INVALID_USER_ID
            };
        }

        var tweet = new Tweet
        {
            AuthorId = user.Id,
            Text = text,
            PubDate = DateTime.Now,
            Flagged = false
        };

        await _context.Tweets.InsertOneAsync(tweet);

        return new DBResult();
    }

    public async Task<DBResult<IEnumerable<Tweet>>> GetAllByUsernameAsync(string username, int? limit = null, CancellationToken ct = default)
    {
        var user = await _context.Users.Find(user => user.Username == username).FirstOrDefaultAsync(ct);

        // User does not exist
        if (user == null)
        {
            return new DBResult<IEnumerable<Tweet>>
            {
                DBError = INVALID_USERNAME,
                Model = null
            };
        }

        var tweets = await _context.Tweets.Find(tweet => tweet.AuthorId == user.Id).SortByDescending(tweet => tweet.PubDate).Limit(limit).ToListAsync(ct);

        return new DBResult<IEnumerable<Tweet>>
        {
            DBError = null,
            Model = tweets
        };
    }

    public async Task<DBResult<IEnumerable<Tweet>>> GetAllNonFlaggedAsync(int? limit = null, CancellationToken ct = default)
    {
        var tweets = await _context.Tweets.Find(tweet => !tweet.Flagged).SortByDescending(tweet => tweet.PubDate).Limit(limit).ToListAsync(ct);

        return new DBResult<IEnumerable<Tweet>>
        {
            DBError = null,
            Model = tweets
        };
    }

    public async Task<DBResult<IEnumerable<Tweet>>> GetAllNonFlaggedFollowedByUserIdAsync(string userId, int? limit = null, CancellationToken ct = default)
    {
        var user = await GetUserByUserIdAsync(userId, ct);

        // User does not exist
        if (user == null)
        {
            return new DBResult<IEnumerable<Tweet>>
            {
                DBError = INVALID_USER_ID,
                Model = null
            };
        }

        // If limit is 0 return all
        if (limit == 0 || limit == null)
        {
            limit = int.MaxValue;
        }

        var tweets = _context.Tweets.AsQueryable();
        var users = _context.Users.AsQueryable();
        var followers = _context.Followers.AsQueryable();

        var result = tweets.Join(users, tweet => tweet.AuthorId, user => user.Id, (tweet, user) => new { tweet, user })
            .Join(followers, tu => tu.tweet.AuthorId, follower => follower.WhomId, (tu, follower) => new { tu.tweet, tu.user, follower })
            .Where(tuf => !tuf.tweet.Flagged && tuf.user.Id == userId || tuf.follower.WhoId == userId)
            .Select(tuf => tuf.tweet)
            .OrderByDescending(tweet => tweet.PubDate)
            .Take(Math.Abs((int)limit))
            .ToList();

        return new DBResult<IEnumerable<Tweet>>
        {
            DBError = null,
            Model = result
        };
    }

    private async Task<User> GetUserByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _context.Users.Find(user => user.Id == userId).FirstOrDefaultAsync(ct);
    }
}