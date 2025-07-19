using System;
using RedGreenBlue.Models;

namespace RedGreenBlue.Services;

public class AuthService : IAuthService
{
    public Task<User?> LoginAsync(string username, string password)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RegisterAsync(string username, string password)
    {
        throw new NotImplementedException();
    }
}
