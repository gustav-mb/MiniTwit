using MongoDB.Driver;
using MiniTwit.Core;
using MiniTwit.Core.IRepositories;
using MiniTwit.Core.Responses;
using MiniTwit.Core.Entities;
using static MiniTwit.Core.Error.Errors;

namespace MiniTwit.Infrastructure.Repositories;

public class FollowerRepository : IFollowerRepository
{
    private readonly IMiniTwitContext _context;

    public FollowerRepository(IMiniTwitContext context)
    {
        _context = context;
    }

    public async Task<DBResult> CreateAsync(string sourceId, string targetUsername)
    {
        var user = await GetUserById(sourceId);

        // See if source exists
        if (user == null)
        {
            return new DBResult
            {
                DBError = INVALID_USER_ID
            };
        }

        var userToFollow = await GetUserByUsername(targetUsername);

        // See if target exists
        if (userToFollow == null)
        {
            return new DBResult
            {
                DBError = INVALID_USERNAME
            };
        }

        // See if source and target are the same
        if (sourceId == userToFollow.Id)
        {
            return new DBResult
            {
                DBError = FOLLOW_SELF
            };
        }
        
        var follower = new Follower
        {
            WhoId = sourceId,
            WhomId = userToFollow.Id
        };

        await _context.Followers.InsertOneAsync(follower);

        return new DBResult();
    }

    public async Task<DBResult> DeleteAsync(string sourceId, string targetUsername)
    {
        var user = await GetUserById(sourceId);

        // See if source exists
        if (user == null)
        {
            return new DBResult
            {
                DBError = INVALID_USER_ID
            };
        }

        var userToUnfollow = await GetUserByUsername(targetUsername);

        // See if target exists
        if (userToUnfollow == null)
        {
            return new DBResult
            {
                DBError = INVALID_USERNAME
            };
        }

        // See if source and target are the same
        if (sourceId == userToUnfollow.Id)
        {
            return new DBResult
            {
                DBError = UNFOLLOW_SELF
            };
        }

        await _context.Followers.DeleteOneAsync(follower => follower.WhoId == sourceId && follower.WhomId == userToUnfollow.Id);

        return new DBResult();
    }

    private async Task<User> GetUserById(string userId)
    {
        return await _context.Users.Find(user => user.Id == userId).FirstOrDefaultAsync();
    }

    private async Task<User> GetUserByUsername(string username)
    {
        return await _context.Users.Find(user => user.Username == username).FirstOrDefaultAsync();
    }
}