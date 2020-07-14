using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Matcha.API.Data;

namespace Matcha.API.Models
{
    public class User
    {
        private readonly IPhotosDataContext _photosDataContext;

        public User(IPhotosDataContext photosDataContext)
        {
            _photosDataContext = photosDataContext;
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
        public virtual ICollection<Like> Likers { get; set; }
        public virtual ICollection<Like> Likees { get; set; }
        public virtual ICollection<Message> MessagesSent { get; set; }
        public virtual ICollection<Message> MessagesReceived { get; set; }
    }
}