using System;
using Microsoft.AspNetCore.Identity;
using RedGreenBlue.Services.Interfaces;

namespace RedGreenBlue.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        throw new NotImplementedException();
    }

    public bool VerifyPassword(string password, string hash)
    {
        throw new NotImplementedException();
    }
}
