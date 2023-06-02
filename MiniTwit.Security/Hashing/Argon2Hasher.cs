using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace MiniTwit.Security.Hashing;

public class Argon2Hasher : IHasher
{
    private readonly HashSettings _settings;

    public Argon2Hasher(IOptions<HashSettings> settings)
    {
        _settings = settings.Value;
    }

    public (string Hash, string Salt) Hash(string data)
    {
        var salt = CreateSalt();
        var hash = Hash(data, salt);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public async Task<(string Hash, string Salt)> HashAsync(string data)
    {
        var salt = CreateSalt();
        var hash = await HashAsync(data, salt);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public bool VerifyHash(string data, string hash, string salt)
    {
        var hashByte = Convert.FromBase64String(hash);
        var saltByte = Convert.FromBase64String(salt);
        var dataHash = Hash(data, saltByte);

        return hashByte.SequenceEqual(dataHash);
    }

    public async Task<bool> VerifyHashAsync(string data, string hash, string salt)
    {
        var hashByte = Convert.FromBase64String(hash);
        var saltByte = Convert.FromBase64String(salt);
        var dataHash = await HashAsync(data, saltByte);

        return hashByte.SequenceEqual(dataHash);
    }

    private byte[] CreateSalt()
    {
        var buffer = new byte[_settings.SaltLength];
        var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buffer);

        return buffer;
    }

    private async Task<byte[]> HashAsync(string data, byte[] salt)
    {
        var argon2 = SetupArgon(data, salt);
        return await argon2.GetBytesAsync(_settings.TagLength);
    }

    private byte[] Hash(string data, byte[] salt)
    {
        var argon2 = SetupArgon(data, salt);
        return argon2.GetBytes(_settings.TagLength);
    }

    private Argon2id SetupArgon(string data, byte[] salt)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        var argon2 = new Argon2id(bytes);
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = _settings.DegreeOfParallelism;
        argon2.Iterations = _settings.Iterations;
        argon2.MemorySize = _settings.MemorySizeMB;

        return argon2;
    }
}