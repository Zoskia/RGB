using System.Threading.Tasks;
using RedGreenBlue.Models;

namespace RedGreenBlue.Repositories
{
    public interface IUserRepository
    {
        Task<bool> UsernameAvailableAsync(string username);
        Task<User?> AddNewUserAsync(User user);
        Task<User?> GetByUsernameAsync(string username);
    }

}
