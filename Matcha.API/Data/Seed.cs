using System.Collections.Generic;
using System.Linq;
using Matcha.API.Models;
using Newtonsoft.Json;

namespace Matcha.API.Data
{
    public class Seed
    {
        public static void SeedUsers(IUserDataContext context, IPhotosDataContext photosDataContext)
        {
            if (!context.GetAllUsersByLastActive().Result.Any())
            {
                System.Console.WriteLine("Seeding users...");
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach (var user in users)
                {
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash("password", out passwordHash, out passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.Username = user.Username.ToLower();
                    context.Add(user);

                    foreach (var photo in user.PhotosFromSeed)
                        photosDataContext.Add(photo);
                }
                System.Console.WriteLine("User Seed complete.");
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}