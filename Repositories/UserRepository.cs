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
        if (!await UsernameAvailableAsync(user.Username))
        {
            return null;
        }

        await _context.Users.AddAsync(user);

        try
        {
            await _context.SaveChangesAsync();
            return user;
        }
        catch (DbUpdateException)
        {
            // Handle registration race condition:
            // username looked free before insert, but was created concurrently.
            _context.Entry(user).State = EntityState.Detached;

            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username == user.Username);

            if (usernameExists)
            {
                return null;
            }

            throw;
        }
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
