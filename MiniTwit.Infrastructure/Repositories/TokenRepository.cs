using MongoDB.Driver;
using MiniTwit.Core;
using MiniTwit.Core.Entities;
using MiniTwit.Core.IRepositories;
using MiniTwit.Core.Responses;
using static MiniTwit.Core.Error.Errors;

namespace MiniTwit.Infrastructure.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly IMiniTwitContext _context;

    public TokenRepository(IMiniTwitContext context)
    {
        _context = context;
    }

    public async Task<DBResult> CreateAsync(string jwtId, string userId, string refreshToken, DateTime expiryTime)
    {
        var user = await _context.Users.Find(user => user.Id == userId).FirstOrDefaultAsync();

        if (user == null)
        {
            return new DBResult
            {
                DBError = INVALID_USER_ID
            };
        }

        var token = new RefreshToken
        {
            JwtId = jwtId,
            UserId = userId,
            Token = refreshToken,
            ExpiryTime = expiryTime
        };

        await _context.RefreshTokens.InsertOneAsync(token);

        return new DBResult
        {
            DBError = null
        };
    }

    public async Task<DBResult> DeleteAllExpiredAsync()
    {
        await _context.RefreshTokens.DeleteManyAsync(token => token.ExpiryTime <= DateTime.UtcNow);

        return new DBResult
        {
            DBError = null
        };
    }

    public async Task<DBResult<RefreshToken>> GetAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens.Find(token => token.Token == refreshToken).FirstOrDefaultAsync();

        if (token == null)
        {
            return new DBResult<RefreshToken>
            {
                 DBError = INVALID_TOKEN,
                 Model = null
            };
        }

        return new DBResult<RefreshToken>
        {
            DBError = null,
            Model = token
        };
    }

    public async Task<DBResult> UpdateUsedAsync(string refreshToken, bool used)
    {
        var token = await _context.RefreshTokens.Find(token => token.Token == refreshToken).FirstOrDefaultAsync();

        if (token == null)
        {
            return new DBResult
            {
                DBError = INVALID_TOKEN
            };
        }

        var update = Builders<RefreshToken>.Update.Set(token => token.Used, used);
        await _context.RefreshTokens.UpdateOneAsync(token => token.Token == refreshToken, update);

        return new DBResult
        {
            DBError = null
        };
    }
}