using System;
using System.Threading.Tasks;
using Matcha.API.Models;

namespace Matcha.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IUserDataContext _userDataContext;

        public AuthRepository(IUserDataContext userDataContext)
        {
            _userDataContext = userDataContext;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _userDataContext.GetByUsername(username);
            //var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _userDataContext.Add(user);

            return user;
        }

        public async Task ResetPassword(User user, string newPassword)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _userDataContext.Update(user);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _userDataContext.GetByUsername(username) != null)
                return true;

            return false;
        }
    }
}