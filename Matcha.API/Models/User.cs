using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Matcha.API.Data;

namespace Matcha.API.Models
{
    public class User
    {
        private readonly IPhotosDataContext _photosDataContext;
        private readonly ILikesDataContext _likesDataContext;
        private readonly IMessagesDataContext _messagesDataContext;

        public User(
            IPhotosDataContext photosDataContext = null,
            ILikesDataContext likesDataContext = null,
            IMessagesDataContext messagesDataContext = null)
        {
            _photosDataContext = photosDataContext;
            _likesDataContext = likesDataContext;
            _messagesDataContext = messagesDataContext;
        }

        public long Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Gender { get; set; }
        public string Sexuality { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Email { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public long FameRating { get; set; }
        public long Deactivated { get; set; }
        public long Activated { get; set; }
        public string Token { get; set; }
        public string Reset { get; set; }
        public virtual ICollection<Photo> PhotosFromSeed { get; set; }
        public async Task<ICollection<Photo>> Photos()
        {
            if (_photosDataContext != null)
                return await _photosDataContext.GetAllForUser(Id);
            else
                return new List<Photo>();
        }

        public async Task<ICollection<Like>> Likers()
        {
            if (_likesDataContext != null)
                return await _likesDataContext.GetLikers(Id);
            else
                return new List<Like>();
        }

        public async Task<ICollection<Like>> Likees()
        {
            if (_likesDataContext != null)
                return await _likesDataContext.GetLikees(Id);
            else
                return new List<Like>();
        }

        public async Task<ICollection<Message>> MessagesSent()
        {
            if (_messagesDataContext != null)
                return await _messagesDataContext.GetOutbox(Id);
            else
                return new List<Message>();
        }

        public async Task<ICollection<Message>> MessagesReceived()
        {
            if (_messagesDataContext != null)
                return await _messagesDataContext.GetInbox(Id);
            else
                return new List<Message>();
        }
    }
}