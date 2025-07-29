using System;
using Microsoft.EntityFrameworkCore;
using RedGreenBlue.Data;
using RedGreenBlue.Models;

namespace RedGreenBlue.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> AddNewUserAsync(User user)
    {
        if (await UsernameAvailableAsync(user.Username))
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        return null;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Username == username);
    }

    public async Task<bool> UsernameAvailableAsync(string username)
    {
        return !await _context.Users.AnyAsync(user => user.Username == username);
    }
}
