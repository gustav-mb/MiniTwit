using MiniTwit.Core.Entities;

namespace MiniTwit.Service.Data.DataCreators;

public static class FollowerCreator
{
    public static Follower Create(string sourceUserId, string targetUserId)
    {
        return new Follower
        {
            WhoId = sourceUserId,
            WhomId = targetUserId
        };
    }
}