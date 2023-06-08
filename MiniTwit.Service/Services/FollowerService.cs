using MiniTwit.Core.IRepositories;
using MiniTwit.Core.Responses;
using MiniTwit.Service.IServices;
using static MiniTwit.Core.Error.Errors;
using static MiniTwit.Core.Responses.HTTPResponse;

namespace MiniTwit.Service.Services;

public class FollowerService : IFollowerService
{
    private readonly IFollowerRepository _repository;

    public FollowerService(IFollowerRepository repository)
    {
        _repository = repository;
    }

    public async Task<APIResponse> FollowUserAsync(string userId, string targetUsername)
    {
        var dbResult = await _repository.CreateAsync(userId, targetUsername);

        if (dbResult.DBError == INVALID_USER_ID || dbResult.DBError == INVALID_USERNAME)
        {
            return new APIResponse(NotFound, dbResult.DBError);
        }

        if (dbResult.DBError == FOLLOW_SELF)
        {
            return new APIResponse(BadRequest, dbResult.DBError);
        }

        return new APIResponse(Created);
    }

    public async Task<APIResponse> UnfollowUserAsync(string userId, string targetUsername)
    {
        var dbResult = await _repository.DeleteAsync(userId, targetUsername);

        if (dbResult.DBError == INVALID_USER_ID || dbResult.DBError == INVALID_USERNAME)
        {
            return new APIResponse(NotFound, dbResult.DBError);
        }

        if (dbResult.DBError == UNFOLLOW_SELF)
        {
            return new APIResponse(BadRequest, dbResult.DBError);
        }

        return new APIResponse(NoContent);
    }
}