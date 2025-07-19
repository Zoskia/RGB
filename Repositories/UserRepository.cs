using System;
using RedGreenBlue.Models;

namespace RedGreenBlue.Repositories;

public class UserRepository : IUserRepository
{
    public Task AddNewUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UsernameAvailableAsync(string username)
    {
        throw new NotImplementedException();
    }
}
