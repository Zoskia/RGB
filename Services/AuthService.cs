using System;
using RedGreenBlue.Models;
using RedGreenBlue.Repositories;
using RedGreenBlue.Services.Interfaces;

namespace RedGreenBlue.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public Task<User?> LoginAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> RegisterAsync(User user)
    {
        user.Password = _passwordHasher.HashPassword(user.Password);
        return await _userRepository.AddNewUserAsync(user);
    }
}
