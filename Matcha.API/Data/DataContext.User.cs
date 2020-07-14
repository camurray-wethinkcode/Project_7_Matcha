using System.Threading.Tasks;
using Matcha.API.Models;

namespace Matcha.API.Data
{
    public interface IUserDataContext
    {
        public Task<User> GetById(long id);
        public Task<User> GetByUsername(string username);
        public Task<User> GetByEmail(string email);
        public Task<User> GetByVerifyToken(string token);
        public Task<User> GetByResetToken(string token);

        public Task<bool> Add(User user);
        public Task<bool> Update(User user);
        public Task<bool> Delete(long id);
    }

    public class UserDataContext : IUserDataContext
    {
        private readonly IDbAccess _dbAccess;

        public UserDataContext(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        private const string _userDBValues = 
            "`Id`,         `Username`,     `PasswordHash`, `PasswordSalt`, `Gender`,   " +
            "`Sexuality`,  `DateOfBirth`,  `Name`,         `Surname`,      `Created`,  " +
            "`LastActive`, `Introduction`, `LookingFor`,   `Email`,        `Interests`," +
            "`City`,       `Country`,      `FameRating`,   `Deactivated`,  `Activated`," +
            "`Token`,      `Reset`";

        private User MapObjArrToUser(object[] objArr) => new User
        {
            Id = (long)objArr[0],
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
            Deactivated = (long)objArr[18],
            Activated = (long)objArr[19],
            Token = (string)objArr[20],
            Reset = (string)objArr[21]
        };

        public async Task<User> GetById(long id)
        {
            var values = await _dbAccess.SelectOne("SELECT" + _userDBValues +
                "FROM `Users`" +
                "WHERE `Id` = @id", new DBParam("id", id));

            return MapObjArrToUser(values);
        }

        public async Task<User> GetByUsername(string username)
        {
            var values = await _dbAccess.SelectOne("SELECT" + _userDBValues +
                "FROM `Users`" +
                "WHERE `Username` = @username", new DBParam("username", username));

            return MapObjArrToUser(values);
        }

        public async Task<User> GetByEmail(string email)
        {
            var values = await _dbAccess.SelectOne("SELECT" + _userDBValues +
                "FROM `Users`" +
                "WHERE `Email` = @email", new DBParam("email", email));

            return MapObjArrToUser(values);
        }

        public async Task<User> GetByVerifyToken(string token)
        {
            var values = await _dbAccess.SelectOne("SELECT" + _userDBValues +
                "FROM `Users`" +
                "WHERE `Token` = @verifyToken", new DBParam("verifyToken", token));

            return MapObjArrToUser(values);
        }

        public async Task<User> GetByResetToken(string token)
        {
            var values = await _dbAccess.SelectOne("SELECT" + _userDBValues +
                "FROM `Users`" +
                "WHERE `Reset` = @resetToken", new DBParam("resetToken", token));

            return MapObjArrToUser(values);
        }

        public async Task<bool> Add(User user)
        {
            var updateAmount = await _dbAccess.Insert("INSERT INTO `Users` (" + _userDBValues + ") VALUES (" +
                "@Id,         @Username,     @PasswordHash, @PasswordSalt, @Gender,   " +
                "@Sexuality,  @DateOfBirth,  @Name,         @Surname,      @Created,  " +
                "@LastActive, @Introduction, @LookingFor,   @Email,        @Interests," +
                "@City,       @Country,      @FameRating,   @Deactivated,  @Activated," +
                "@Token,      @Reset )",
                new DBParam("Id", user.Id), new DBParam("Username", user.Username), new DBParam("PasswordHash", user.PasswordHash), new DBParam("PasswordSalt", user.PasswordSalt), new DBParam("Gender", user.Gender),
                new DBParam("Sexuality", user.Sexuality), new DBParam("DateOfBirth", user.DateOfBirth), new DBParam("Name", user.Name), new DBParam("Surname", user.Surname), new DBParam("Created", user.Created),
                new DBParam("LastActive", user.LastActive), new DBParam("Introduction", user.Introduction), new DBParam("LookingFor", user.LookingFor), new DBParam("Email", user.Email), new DBParam("Interests", user.Interests),
                new DBParam("City", user.City), new DBParam("Country", user.Country), new DBParam("FameRating", user.FameRating), new DBParam("Deactivated", user.Deactivated), new DBParam("Activated", user.Activated),
                new DBParam("Token", user.Token), new DBParam("Reset", user.Reset));

            return updateAmount == 1;
        }

        public async Task<bool> Update(User user)
        {
            var updateAmount = await _dbAccess.Update("UPDATE `Users` SET " +
                "`Id` = @Id, `Username` = @Username, `PasswordHash` = @PasswordHash, `PasswordSalt` = @PasswordSalt, `Gender` = @Gender, " +
                "`Sexuality` = @Sexuality, `DateOfBirth` = @DateOfBirth, `Name` = @Name, `Surname` = @Surname, `Created` = @Created, " +
                "`LastActive` = @LastActive, `Introduction` = @Introduction, `LookingFor` = @LookingFor, `Email` = @Email, `Interests` = @Interests, " +
                "`City` = @City, `Country` = @Country, `FameRating` = @FameRating, `Deactivated` = @Deactivated, `Activated` = @Activated, " +
                "`Token` = @Token, `Reset` = @Reset" +
                "WHERE `Id` = @Id",
                new DBParam("Id", user.Id), new DBParam("Username", user.Username), new DBParam("PasswordHash", user.PasswordHash), new DBParam("PasswordSalt", user.PasswordSalt), new DBParam("Gender", user.Gender),
                new DBParam("Sexuality", user.Sexuality), new DBParam("DateOfBirth", user.DateOfBirth), new DBParam("Name", user.Name), new DBParam("Surname", user.Surname), new DBParam("Created", user.Created),
                new DBParam("LastActive", user.LastActive), new DBParam("Introduction", user.Introduction), new DBParam("LookingFor", user.LookingFor), new DBParam("Email", user.Email), new DBParam("Interests", user.Interests),
                new DBParam("City", user.City), new DBParam("Country", user.Country), new DBParam("FameRating", user.FameRating), new DBParam("Deactivated", user.Deactivated), new DBParam("Activated", user.Activated),
                new DBParam("Token", user.Token), new DBParam("Reset", user.Reset));

            return updateAmount == 1;
        }

        public async Task<bool> Delete(long id)
        {
            return await _dbAccess.Delete("DELETE FROM `Users` WHERE `Id` = @Id", new DBParam("Id", id));
        }
    }
}
