using static MiniTwit.Core.Error.DBError;

namespace MiniTwit.Core.Error;

public enum DBError
{
    INVALID_USER_ID,
    INVALID_USERNAME,
    INVALID_PASSWORD,
    FOLLOW_SELF,
    UNFOLLOW_SELF,
    USERNAME_TAKEN
}

public static class DBErrorExtensions
{
    public static string ToMsg(this DBError DBError) => DBError switch
    {
        INVALID_USER_ID => "Invalid user id",
        INVALID_USERNAME => "Invalid username",
        INVALID_PASSWORD => "Invalid password",
        FOLLOW_SELF => "Can't follow yourself",
        UNFOLLOW_SELF => "Can't unfollow yourself",
        USERNAME_TAKEN => "Username is already taken",
        _ => throw new NotSupportedException($"DBError of type '{typeof(DBError)}' not supported!")
    };
}