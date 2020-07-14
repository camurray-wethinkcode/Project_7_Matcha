using System.Collections.Generic;
using System.Threading.Tasks;
using Matcha.API.Helpers;
using Matcha.API.Models;

namespace Matcha.API.Data
{
    public interface IDatingRepository
    {
        Task<bool> Add<T>(T entity) where T : class;
        Task<bool> Update<T>(T entity) where T : class;
        Task<bool> Delete<T>(T entity) where T : class;
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(int id);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByVerifyToken(string verifyToken);
        Task<User> GetUserByResetToken(string resetToken);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);
        Task<Like> GetLike(int userId, int recipientId);
        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
    }
}