using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Matcha.API.Helpers;
using Matcha.API.Models;

namespace Matcha.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly IUserDataContext _userDataContext;
        private readonly ILikesDataContext _likesDataContext;
        private readonly IPhotosDataContext _photosDataContext;

        public DatingRepository(IUserDataContext userDataContext, ILikesDataContext likesDataContext, IPhotosDataContext photosDataContext)
        {
            _userDataContext = userDataContext;
            _likesDataContext = likesDataContext;
            _photosDataContext = photosDataContext;
        }

        public void Add<T>(T entity) where T : class
        {
            if (typeof(T) == typeof(User))
                _userDataContext.Add(entity as User);
            else if (typeof(T) == typeof(Like))
                _likesDataContext.Add(entity as Like);
            else if (typeof(T) == typeof(Photo))
                _photosDataContext.Add(entity as Photo);
            else
                throw new NotImplementedException();
        }

        public void Delete<T>(T entity) where T : class
        {
            if (typeof(T) == typeof(User))
                _userDataContext.Delete((entity as User).Id);
            else if (typeof(T) == typeof(Like))
                _likesDataContext.Delete((entity as Like).LikerId, (entity as Like).LikeeId);
            else if (typeof(T) == typeof(Photo))
                _photosDataContext.Delete((entity as Photo).Id);
            else
                throw new NotImplementedException();
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _likesDataContext.Get(userId, recipientId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _photosDataContext.GetMainForUser(userId);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await _photosDataContext.GetById(id);
        }

        public async Task<User> GetUser(int id)
        {
            return await _userDataContext.GetById(id);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _userDataContext.GetByEmail(email);
        }

        public async Task<User> GetUserByVerifyToken(string verifyToken)
        {
            return await _userDataContext.GetByVerifyToken(verifyToken);
        }

        public async Task<User> GetUserByResetToken(string resetToken)
        {
            return await _userDataContext.GetByResetToken(resetToken);
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(u => u.Id != userParams.UserId);

            users = users.Where(u => u.Gender == userParams.Gender);

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _userDataContext.GetById(id);

            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId
                        && u.RecipientDeleted == false);
                    break;
                case "Outbox":
                    messages = messages.Where(u => u.SenderId == messageParams.UserId
                        && u.SenderDeleted == false);
                    break;
                default:
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId
                        && u.RecipientDeleted == false && u.IsRead == false);
                    break;
            }

            messages = messages.OrderByDescending(d => d.MessageSent);

            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
                .Where(m => m.RecipientId == userId && m.RecipientDeleted == false
                    && m.SenderId == recipientId
                    || m.RecipientId == recipientId && m.SenderId == userId
                    && m.SenderDeleted == false)
                .OrderByDescending(m => m.MessageSent)
                .ToListAsync();

            return messages;
        }
    }
}