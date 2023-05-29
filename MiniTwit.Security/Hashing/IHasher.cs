namespace MiniTwit.Security.Hashing;

public interface IHasher
{
    Task<(string Hash, string Salt)> HashAsync(string data);
    Task<bool> VerifyHashAsync(string data, string hash, string salt);
}