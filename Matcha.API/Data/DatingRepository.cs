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
        private readonly IMessagesDataContext _messagesDataContext;

        public DatingRepository(
            IUserDataContext userDataContext,
            ILikesDataContext likesDataContext,
            IPhotosDataContext photosDataContext,
            IMessagesDataContext messagesDataContext)
        {
            _userDataContext = userDataContext;
            _likesDataContext = likesDataContext;
            _photosDataContext = photosDataContext;
            _messagesDataContext = messagesDataContext;
        }

        public async Task<bool> Add<T>(T entity) where T : class
        {
            if (typeof(T) == typeof(User))
                return await _userDataContext.Add(entity as User);
            else if (typeof(T) == typeof(Like))
                return await _likesDataContext.Add(entity as Like);
            else if (typeof(T) == typeof(Photo))
                return await _photosDataContext.Add(entity as Photo);
            else if (typeof(T) == typeof(Message))
                return await _messagesDataContext.Add(entity as Message);
            else
                throw new NotImplementedException();
        }

        public async Task<bool> Update<T>(T entity) where T:class
        {
            if (typeof(T) == typeof(User))
                return await _userDataContext.Update(entity as User);
            else if (typeof(T) == typeof(Like))
                return await _likesDataContext.Update(entity as Like);
            else if (typeof(T) == typeof(Photo))
                return await _photosDataContext.Update(entity as Photo);
            else if (typeof(T) == typeof(Message))
                return await _messagesDataContext.Update(entity as Message);
            else
                throw new NotImplementedException();
        }

        public async Task<bool> Delete<T>(T entity) where T : class
        {
            if (typeof(T) == typeof(User))
                return await _userDataContext.Delete((entity as User).Id);
            else if (typeof(T) == typeof(Like))
                return await _likesDataContext.Delete((entity as Like).LikerId, (entity as Like).LikeeId);
            else if (typeof(T) == typeof(Photo))
                return await _photosDataContext.Delete((entity as Photo).Id);
            else if (typeof(T) == typeof(Message))
                return await _messagesDataContext.Delete((entity as Message).Id);
            else
                throw new NotImplementedException();
        }

        public Task<Like> GetLike(int userId, int recipientId) => _likesDataContext.Get(userId, recipientId);

        public Task<Photo> GetMainPhotoForUser(int userId) => _photosDataContext.GetMainForUser(userId);

        public Task<Photo> GetPhoto(int id) => _photosDataContext.GetById(id);

        public Task<User> GetUser(int id) => _userDataContext.GetById(id);

        public Task<User> GetUserByEmail(string email) => _userDataContext.GetByEmail(email);

        public Task<User> GetUserByVerifyToken(string verifyToken) => _userDataContext.GetByVerifyToken(verifyToken);

        public Task<User> GetUserByResetToken(string resetToken) => _userDataContext.GetByResetToken(resetToken);

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            IEnumerable<User> users;

            users = await _userDataContext.GetAllUsersByLastActive();

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

            if (userParams.SameCountry)
            {
                var currentUser = await _userDataContext.GetById(userParams.UserId);
                users = users.Where(u => u.Country == currentUser.Country);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.FameRating);
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.FameRating);
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            return PagedList<User>.Create(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<long>> GetUserLikes(int id, bool likers)
        {
            var user = await _userDataContext.GetById(id);

            if (likers)
            {
                return (await user.Likers()).Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return (await user.Likees()).Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }

        public async Task<Message> GetMessage(int id) => await _messagesDataContext.GetById(id);

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            IEnumerable<Message> messages;

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = await _messagesDataContext.GetInbox(messageParams.UserId);
                    break;
                case "Outbox":
                    messages = await _messagesDataContext.GetOutbox(messageParams.UserId);
                    break;
                default:
                    messages = await _messagesDataContext.GetUnread(messageParams.UserId);
                    break;
            }

            messages = messages.OrderByDescending(d => d.MessageSent);

            return PagedList<Message>.Create(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId) => await _messagesDataContext.GetThread(userId, recipientId);
    }
}