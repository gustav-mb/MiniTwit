namespace MiniTwit.Security.Hashing;

public interface IHasher
{
    (string Hash, string Salt) Hash(string data);
    Task<(string Hash, string Salt)> HashAsync(string data);
    bool VerifyHash(string data, string hash, string salt);
    Task<bool> VerifyHashAsync(string data, string hash, string salt);
}