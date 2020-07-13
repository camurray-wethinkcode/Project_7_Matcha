using System.Threading.Tasks;
using Matcha.API.Models;

namespace Matcha.API.Data
{
    public interface IUserDataContext
    {
        public Task<User> GetById(int id);
        public Task<User> GetByUsername(string username);
        public Task<User> GetByEmail(string email);
        public Task<User> GetByVerifyToken(string token);
        public Task<User> GetByResetToken(string token);


    }

    public class UserDataContext : IUserDataContext
    {
        private readonly IDbAccess _dbAccess;

        public UserDataContext(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        private const string SelectUserValues = 
            "`Id`,         `Username`,     `PasswordHash`, `PasswordSalt`, `Gender`,"    +
            "`Sexuality`,  `DateOfBirth`,  `Name`,         `Surname`,      `Created`,"   +
            "`LastActive`, `Introduction`, `LookingFor`,   `Email`,        `Interests`," +
            "`City`,       `Country`,      `FameRating`,   `Deactivated`,  `Activated`," +
            "`Token`,      `Reset`";

        private User MapObjArrToUser(object[] objArr) => new User
        {
            Id = (int)objArr[0],
            Username = (string)objArr[1],
            PasswordHash = (byte[])objArr[2],
            PasswordSalt = (byte[])objArr[3],
            Gender = (string)objArr[4],
            Sexuality = (string)objArr[5],
            DateOfBirth = (System.DateTime)objArr[6],
            Name = (string)objArr[7],
            Surname = (string)objArr[8],
            Created = (System.DateTime)objArr[9],
            LastActive = (System.DateTime)objArr[10],
            Introduction = (string)objArr[11],
            LookingFor = (string)objArr[12],
            Email = (string)objArr[13],
            Interests = (string)objArr[14],
            City = (string)objArr[15],
            Country = (string)objArr[16],
            FameRating = (string)objArr[17],
            Deactivated = (int)objArr[18],
            Activated = (int)objArr[19],
            Token = (string)objArr[20],
            Reset = (string)objArr[21]
        };

        public async Task<User> GetById(int id)
        {
            var values = await _dbAccess.SelectOne("SELECT" + SelectUserValues +
                "FROM `Users`" +
                "WHERE `Id` = @id", new DBParam("id", id));

            return MapObjArrToUser(values);
        }

        public async Task<User> GetByUsername(string username)
        {
            var values = await _dbAccess.SelectOne("SELECT" + SelectUserValues +
                "FROM `Users`" +
                "WHERE `Username` = @username", new DBParam("username", username));

            return MapObjArrToUser(values);
        }


        public async Task<User> GetByEmail(string email)
        {
            var values = await _dbAccess.SelectOne("SELECT" + SelectUserValues +
                "FROM `Users`" +
                "WHERE `Email` = @email", new DBParam("email", email));

            return MapObjArrToUser(values);
        }

        public async Task<User> GetByVerifyToken(string token)
        {
            var values = await _dbAccess.SelectOne("SELECT" + SelectUserValues +
                "FROM `Users`" +
                "WHERE `Token` = @verifyToken", new DBParam("verifyToken", token));

            return MapObjArrToUser(values);
        }

        public async Task<User> GetByResetToken(string token)
        {
            var values = await _dbAccess.SelectOne("SELECT" + SelectUserValues +
                "FROM `Users`" +
                "WHERE `Reset` = @resetToken", new DBParam("resetToken", token));

            return MapObjArrToUser(values);
        }
    }
}