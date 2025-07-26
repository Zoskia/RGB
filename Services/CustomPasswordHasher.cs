using RedGreenBlue.Services.Interfaces;
using Isopoh.Cryptography.Argon2;
using System.Text;
using System.Security.Cryptography;

namespace RedGreenBlue.Services;

public class CustomPasswordHasher : IPasswordHasher
{
    public string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    public string HashPassword(string password, string salt)
    {


        var config = new Argon2Config
        {
            Type = Argon2Type.DataIndependentAddressing,
            Version = Argon2Version.Nineteen,
            TimeCost = 3,
            MemoryCost = 65536,
            Lanes = 4,
            Salt = Convert.FromBase64String(salt),
            Password = Encoding.UTF8.GetBytes(password),
            HashLength = 32
        };

        return Argon2.Hash(config);
    }

    public bool VerifyPassword(string hash, string password)
    {
        return Argon2.Verify(hash, password);
    }
}


