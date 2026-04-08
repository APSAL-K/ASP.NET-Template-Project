using System;
using System.Security.Cryptography;
using ModuleDrivenFramwork.Modules.Auth.Application.Interfaces;

namespace ModuleDrivenFramwork.Modules.Auth.Services;

public class PasswordHasher : IPasswordHasher
{
    public (string hash, string salt) HashPassword(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        var salt = Convert.ToBase64String(saltBytes);
        var hash = HashUsingPbkdf2(password, saltBytes);
        return (Convert.ToBase64String(hash), salt);
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        try
        {
            var saltBytes = Convert.FromBase64String(salt);
            var expectedHash = HashUsingPbkdf2(password, saltBytes);
            var storedHash = Convert.FromBase64String(hash);

            return storedHash.Length == expectedHash.Length &&
                   CryptographicOperations.FixedTimeEquals(storedHash, expectedHash);
        }
        catch
        {
            return false;
        }
    }

    private static byte[] HashUsingPbkdf2(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, 10000, HashAlgorithmName.SHA256, 32);
    }
}
