using System;

namespace Matcha.API.Dtos
{
    public class UserForListDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Gender { get; set; }
        public string Sexuality { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int FameRating { get; set; }
        public int Deactivated { get; set; }
        public int Activated { get; set; }
        public string Token { get; set; }
        public string Reset { get; set; }
        public string PhotoUrl { get; set; }
    }
}