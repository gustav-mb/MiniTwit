using MiniTwit.Security.Hashing;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace MiniTwit.Tests.Security.Tests.Hashing;

public class Argon2HasherTests
{
    private readonly HashSettings _settings;
    private readonly Argon2Hasher _hasher;
    
    public Argon2HasherTests()
    {
        _settings = new HashSettings
        {
            DegreeOfParallelism = 8,
            Iterations = 4,
            MemorySizeMB = 50,
            TagLength = 128,
            SaltLength = 16
        };

        _hasher = new Argon2Hasher(Options.Create<HashSettings>(_settings));
    }

    private byte[] CreateSalt()
    {
        var buffer = new byte[_settings.SaltLength];
        var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buffer);

        return buffer;
    }

    [Fact]
    public async Task HashAsync_creates_different_hashes_given_same_input()
    {
        // Act
        var actual1 = await _hasher.HashAsync("password");
        var actual2 = await _hasher.HashAsync("password");

        // Assert
        Assert.NotEqual(actual1, actual2);
    }

    [Fact]
    public async Task VerifyHashAsync_returns_true_when_data_and_hash_are_equal()
    {
        // Arrange
        var hashResult = await _hasher.HashAsync("password");

        // Act
        var actual = await _hasher.VerifyHashAsync("password", hashResult.Hash, hashResult.Salt);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public async Task VerifyHashAsync_returns_false_when_data_and_hash_are_not_equal()
    {
        // Arrange
        var hashResult = await _hasher.HashAsync("password");

        // Act
        var actual = await _hasher.VerifyHashAsync("pass", hashResult.Hash, hashResult.Salt);

        // Assert
        Assert.False(actual);
    }
}