namespace Matcha.API.Dtos
{
    public class UserForUpdateDto
    {
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public int FameRating { get; set; }
        public int Deactivated { get; set; }
        public int Activated { get; set; }
    }
}