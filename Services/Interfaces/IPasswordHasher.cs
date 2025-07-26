using System;

namespace RedGreenBlue.Services.Interfaces;

public interface IPasswordHasher
{
    string GenerateSalt();
    string HashPassword(string password, string salt);
    bool VerifyPassword(string hash, string password);
}
