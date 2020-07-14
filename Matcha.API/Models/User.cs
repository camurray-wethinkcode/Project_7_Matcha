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
            IPhotosDataContext photosDataContext,
            ILikesDataContext likesDataContext,
            IMessagesDataContext messagesDataContext)
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
        public async Task<ICollection<Photo>> Photos() => await _photosDataContext.GetAllForUser(Id);
        public async Task<ICollection<Like>> Likers() => await _likesDataContext.GetLikers(Id);
        public async Task<ICollection<Like>> Likees() => await _likesDataContext.GetLikees(Id);
        public async Task<ICollection<Message>> MessagesSent() => await _messagesDataContext.GetOutbox(Id);
        public async Task<ICollection<Message>> MessagesReceived() => await _messagesDataContext.GetInbox(Id);
    }
}