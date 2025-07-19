using RedGreenBlue.Services.Interfaces;
using Isopoh.Cryptography.Argon2;
using System.Text;

namespace RedGreenBlue.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        var config = new Argon2Config
        {
            Type = Argon2Type.DataIndependentAddressing,
            Version = Argon2Version.Nineteen,
            TimeCost = 3,
            MemoryCost = 65536,
            Lanes = 4,
            Password = Encoding.UTF8.GetBytes(password)
        };

        return Argon2.Hash(config);
    }

    public bool VerifyPassword(string hash, string password)
    {
        return Argon2.Verify(hash, password);
    }
}


