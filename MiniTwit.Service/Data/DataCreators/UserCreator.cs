using MiniTwit.Core.Entities;

namespace MiniTwit.Service.Data.DataCreators;

public static class UserCreator
{
    public static User Create(string username, string email, string password)
    {
        return new User
        {
            Username = username,
            Email = email,
            Password = password
        };
    }
}