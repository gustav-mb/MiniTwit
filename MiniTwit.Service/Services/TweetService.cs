using MiniTwit.Core.DTOs;
using MiniTwit.Core.IRepositories;
using MiniTwit.Core.Responses;
using MiniTwit.Service.IServices;
using static MiniTwit.Core.Responses.HTTPResponse;

namespace MiniTwit.Service.Services;

public class TweetService : ITweetService
{
    private readonly ITweetRepository _repository;

    public TweetService(ITweetRepository repository)
    {
        _repository = repository;
    }

    public async Task<APIResponse> CreateTweetAsync(TweetCreateDTO tweetCreateDTO)
    {
        var dbResult = await _repository.CreateAsync(tweetCreateDTO.AuthorId, tweetCreateDTO.Text);

        if (dbResult.DBError != null)
        {
            return new APIResponse(NotFound, dbResult.DBError);
        }

        return new APIResponse(Created);
    }

    public async Task<APIResponse<IEnumerable<TweetDTO>>> GetAllNonFlaggedTweetsAsync(int? limit = null, CancellationToken ct = default)
    {
        var dbResult = await _repository.GetAllNonFlaggedAsync(limit, ct);

        return new APIResponse<IEnumerable<TweetDTO>>(Ok, dbResult.ConvertModelTo<IEnumerable<TweetDTO>>());
    }

    public async Task<APIResponse<IEnumerable<TweetDTO>>> GetUsersAndFollowedNonFlaggedTweetsAsync(string userId, int? limit = null, CancellationToken ct = default)
    {
        var dbResult = await _repository.GetAllNonFlaggedFollowedByUserIdAsync(userId, limit, ct);

        if (dbResult.DBError != null)
        {
            return new APIResponse<IEnumerable<TweetDTO>>(NotFound, null, dbResult.DBError);
        }

        return new APIResponse<IEnumerable<TweetDTO>>(Ok, dbResult.ConvertModelTo<IEnumerable<TweetDTO>>());
    }

    public async Task<APIResponse<IEnumerable<TweetDTO>>> GetUsersTweetsAsync(string username, int? limit = null, CancellationToken ct = default)
    {
        var dbResult = await _repository.GetAllByUsernameAsync(username, limit, ct);

        if (dbResult.DBError != null)
        {
            return new APIResponse<IEnumerable<TweetDTO>>(NotFound, null, dbResult.DBError);
        }

        return new APIResponse<IEnumerable<TweetDTO>>(Ok, dbResult.ConvertModelTo<IEnumerable<TweetDTO>>());
    }
}