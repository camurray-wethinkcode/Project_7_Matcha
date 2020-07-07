using System;
using System.ComponentModel.DataAnnotations;

namespace Matcha.API.Dtos
{
    public class UserForRegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string Sexuality { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Email { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public int FameRating { get; set; }
        public int Deactivated { get; set; }
        public UserForRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}