using MiniTwit.Core.Responses;
using static MiniTwit.Core.Error.DBError;

namespace MiniTwit.Core.Error;

public enum DBError
{
    INVALID_USER_ID,
    INVALID_USERNAME,
    INVALID_PASSWORD,
    FOLLOW_SELF,
    UNFOLLOW_SELF,
    USERNAME_TAKEN,
    USERNAME_MISSING,
    EMAIL_MISSING_OR_INVALID,
    PASSWORD_MISSING,
    NO_TOKEN
}

public static class DBErrorExtensions
{
    private static string Msg(this DBError? dbError) => dbError switch
    {
        INVALID_USER_ID => "Invalid user id",
        INVALID_USERNAME => "Invalid username",
        INVALID_PASSWORD => "Invalid password",
        FOLLOW_SELF => "Can't follow yourself",
        UNFOLLOW_SELF => "Can't unfollow yourself",
        USERNAME_TAKEN => "Username is already taken",
        USERNAME_MISSING => "Username is missing",
        EMAIL_MISSING_OR_INVALID => "Email is missing or invalid",
        PASSWORD_MISSING => "Password is missing",
        NO_TOKEN => "No token found",
        _ => "Unknown error!"
    };

    public static APIError? ToAPIError(this DBError? dbError, HTTPResponse HTTPResponse)
    {
        if (dbError == null)
        {
            return null;
        }

        return new APIError
        {
            Status = (int)HTTPResponse,
            ErrorMsg = dbError.Msg()
        };
    }
}