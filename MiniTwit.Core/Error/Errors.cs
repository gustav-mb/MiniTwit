using MiniTwit.Core.Responses;

namespace MiniTwit.Core.Error;

public static class Errors
{
    public const string INVALID_USER_ID = "Invalid user id";
    public const string INVALID_USERNAME = "Invalid username";
    public const string INVALID_PASSWORD = "Invalid password";
    public const string INVALID_TOKEN = "Invalid token";

    public const string FOLLOW_SELF = "You cannot follow yourself";
    public const string UNFOLLOW_SELF = "You cannot unfollow yourself";
    public const string USERNAME_TAKEN = "Username is already taken";

    public const string USERNAME_MISSING = "Username is missing";
    public const string EMAIL_MISSING_OR_INVALID = "Email is missing or invalid";
    public const string PASSWORD_MISSING = "Password is missing";

    public const string TOKEN_EXPIRED = "The token has expired";
    public const string TOKEN_NOT_EXPIRED = "The token has not expired yet";
    public const string TOKEN_INVALIDATED = "The token has been invalidated";
    public const string TOKEN_USED = "The token has aleady been used";

    public const string FORBIDDEN_OPERATION = "You are not authorized to do that";

    public static APIError ToAPIError(HTTPResponse HTTPResponse, string error)
    {
        return new APIError
        {
            Status = (int)HTTPResponse,
            ErrorMsg = error
        };
    }
}