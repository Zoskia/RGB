using System;
using Microsoft.AspNetCore.Identity;
using RedGreenBlue.Dtos;
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

    public async Task<User?> LoginAsync(string username, string password)
    {
        var dbUser = await _userRepository.GetByUsernameAsync(username);
        if (dbUser == null) return null;

        bool isPasswordCorrect = _passwordHasher.VerifyPassword(dbUser.Password, password);
        if (!isPasswordCorrect) return null;

        return dbUser;

        //TODO: JWT, session etc
    }


    public async Task<User?> RegisterAsync(User user)
    {
        string salt = _passwordHasher.GenerateSalt();
        string hash = _passwordHasher.HashPassword(user.Password, salt);

        user.Password = hash;
        user.Salt = salt;

        return await _userRepository.AddNewUserAsync(user);
    }

}
