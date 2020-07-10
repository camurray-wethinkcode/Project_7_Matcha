using System.Threading.Tasks;
using Matcha.API.Models;

namespace Matcha.API.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task ResetPassword(User user, string newPassword);
        Task<bool> UserExists(string username);
    }
}