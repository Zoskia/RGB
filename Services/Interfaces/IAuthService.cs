using System;
using RedGreenBlue.Models;

namespace RedGreenBlue.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(User user);
    Task<User?> LoginAsync(User user);
}
