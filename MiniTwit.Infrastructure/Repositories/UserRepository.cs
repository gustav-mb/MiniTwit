using MongoDB.Driver;
using MiniTwit.Core;
using MiniTwit.Core.Entities;
using MiniTwit.Core.IRepositories;
using MiniTwit.Core.Responses;
using static MiniTwit.Core.Error.DBError;

namespace MiniTwit.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMiniTwitContext _context;

    public UserRepository(IMiniTwitContext context)
    {
        _context = context;
    }

    public async Task<DBResult> CreateAsync(string username, string email, string password, string salt)
    {
        var existingUser = await _context.Users.Find(user => user.Username == username).FirstOrDefaultAsync();

        // Username taken
        if (existingUser != null)
        {
            return new DBResult
            {
                DBError = USERNAME_TAKEN
            };
        }

        var user = new User
        {
            Username = username,
            Email = email,
            Password = password,
            Salt = salt
        };

        await _context.Users.InsertOneAsync(user);

        return new DBResult();
    }

    public async Task<DBResult<User>> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        var user = await _context.Users.Find(user => user.Username == username).FirstOrDefaultAsync(ct);

        // User does not exist
        if (user == null)
        {
            return new DBResult<User>
            {
                DBError = INVALID_USERNAME,
                Model = null
            };
        }

        return new DBResult<User>
        {
            DBError = null,
            Model = user
        };
    }

    public async Task<DBResult<User>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var user = await _context.Users.Find(user => user.Id == userId).FirstOrDefaultAsync();

        // User does not exist
        if (user == null)
        {
            return new DBResult<User>
            {
                DBError = INVALID_USER_ID,
                Model = null
            };
        }

        return new DBResult<User>
        {
            DBError = null,
            Model = user
        };
    }
}