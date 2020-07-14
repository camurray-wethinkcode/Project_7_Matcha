using System;
using System.Collections.Generic;
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
        public Task<List<User>> GetAllUsersByLastActive();

        public Task<bool> Add(User user);
        public Task<bool> Update(User user);
        public Task<bool> Delete(long id);
    }

    public class UserDataContext : IUserDataContext
    {
        private readonly IDbAccess _dbAccess;
        private readonly IPhotosDataContext _photosDataContext;

        public UserDataContext(IDbAccess dbAccess, IPhotosDataContext photosDataContext)
        {
            _dbAccess = dbAccess;
            _photosDataContext = photosDataContext;
        }

        private const string _userDBValues = 
            "`Id`,         `Username`,     `PasswordHash`, `PasswordSalt`, `Gender`,   " +
            "`Sexuality`,  `DateOfBirth`,  `Name`,         `Surname`,      `Created`,  " +
            "`LastActive`, `Introduction`, `LookingFor`,   `Email`,        `Interests`," +
            "`City`,       `Country`,      `FameRating`,   `Deactivated`,  `Activated`," +
            "`Token`,      `Reset` ";

        private User MapObjArrToUser(object[] objArr)
        {
            if (objArr[0].GetType() != typeof(long)) throw new Exception("Id is of wrong type: " + objArr[0].GetType().FullName);
            if (objArr[1].GetType() != typeof(string)) throw new Exception("Username is of wrong type: " + objArr[1].GetType().FullName);
            if (objArr[2].GetType() != typeof(byte[])) throw new Exception("PasswordHash is of wrong type: " + objArr[2].GetType().FullName);
            if (objArr[3].GetType() != typeof(byte[])) throw new Exception("PasswordSalt is of wrong type: " + objArr[3].GetType().FullName);
            if (objArr[4].GetType() != typeof(string)) throw new Exception("Gender is of wrong type: " + objArr[4].GetType().FullName);
            if (objArr[5].GetType() != typeof(string)) throw new Exception("Sexuality is of wrong type: " + objArr[5].GetType().FullName);
            if (objArr[6].GetType() != typeof(string)) throw new Exception("DateOfBirth is of wrong type: " + objArr[6].GetType().FullName);
            if (objArr[7].GetType() != typeof(string)) throw new Exception("Name is of wrong type: " + objArr[7].GetType().FullName);
            if (objArr[8].GetType() != typeof(string)) throw new Exception("Surname is of wrong type: " + objArr[8].GetType().FullName);
            if (objArr[9].GetType() != typeof(string)) throw new Exception("Created is of wrong type: " + objArr[9].GetType().FullName);
            if (objArr[10].GetType() != typeof(string)) throw new Exception("LastActive is of wrong type: " + objArr[10].GetType().FullName);
            if (objArr[11].GetType() != typeof(string)) throw new Exception("Introduction is of wrong type: " + objArr[11].GetType().FullName);
            if (objArr[12].GetType() != typeof(string)) throw new Exception("LookingFor is of wrong type: " + objArr[12].GetType().FullName);
            if (objArr[13].GetType() != typeof(string)) throw new Exception("Email is of wrong type: " + objArr[13].GetType().FullName);
            if (objArr[14].GetType() != typeof(string)) throw new Exception("Interests is of wrong type: " + objArr[14].GetType().FullName);
            if (objArr[15].GetType() != typeof(string)) throw new Exception("City is of wrong type: " + objArr[15].GetType().FullName);
            if (objArr[16].GetType() != typeof(string)) throw new Exception("Country is of wrong type: " + objArr[16].GetType().FullName);
            if (objArr[17].GetType() != typeof(long)) throw new Exception("FameRating is of wrong type: " + objArr[17].GetType().FullName);
            if (objArr[18].GetType() != typeof(long)) throw new Exception("Deactivated is of wrong type: " + objArr[18].GetType().FullName);
            if (objArr[19].GetType() != typeof(long)) throw new Exception("Activated is of wrong type: " + objArr[19].GetType().FullName);
            if (objArr[20].GetType() != typeof(string) &&
                objArr[20].GetType() != typeof(DBNull)) throw new Exception("Token is of wrong type: " + objArr[20].GetType().FullName);
            if (objArr[21].GetType() != typeof(string) &&
                objArr[21].GetType() != typeof(DBNull)) throw new Exception("Reset is of wrong type: " + objArr[21].GetType().FullName);
            return new User(_photosDataContext)
            {
                Id = (long)objArr[0],
                Username = (string)objArr[1],
                PasswordHash = (byte[])objArr[2],
                PasswordSalt = (byte[])objArr[3],
                Gender = (string)objArr[4],
                Sexuality = (string)objArr[5],
                DateOfBirth = DateTime.Parse((string)objArr[6]),
                Name = (string)objArr[7],
                Surname = (string)objArr[8],
                Created = DateTime.Parse((string)objArr[9]),
                LastActive = DateTime.Parse((string)objArr[10]),
                Introduction = (string)objArr[11],
                LookingFor = (string)objArr[12],
                Email = (string)objArr[13],
                Interests = (string)objArr[14],
                City = (string)objArr[15],
                Country = (string)objArr[16],
                FameRating = (long)objArr[17],
                Deactivated = (long)objArr[18],
                Activated = (long)objArr[19],
                Token = (objArr[20].GetType() == typeof(string)) ? (string)objArr[20] : null,
                Reset = (objArr[21].GetType() == typeof(string)) ? (string)objArr[21] : null
            };
        }

        public async Task<User> GetById(long id)
        {
            var values = await _dbAccess.SelectOne("SELECT " + _userDBValues +
                "FROM `Users` " +
                "WHERE `Id` = @id", new DBParam("id", id));

            if (values == null) return null;

            return MapObjArrToUser(values);
        }

        public async Task<User> GetByUsername(string username)
        {
            var values = await _dbAccess.SelectOne("SELECT " + _userDBValues +
                "FROM `Users` " +
                "WHERE `Username` = @username", new DBParam("username", username));

            if (values == null) return null;

            return MapObjArrToUser(values);
        }

        public async Task<User> GetByEmail(string email)
        {
            var values = await _dbAccess.SelectOne("SELECT " + _userDBValues +
                "FROM `Users` " +
                "WHERE `Email` = @email", new DBParam("email", email));

            if (values == null) return null;

            return MapObjArrToUser(values);
        }

        public async Task<User> GetByVerifyToken(string token)
        {
            var values = await _dbAccess.SelectOne("SELECT " + _userDBValues +
                "FROM `Users` " +
                "WHERE `Token` = @verifyToken", new DBParam("verifyToken", token));

            if (values == null) return null;

            return MapObjArrToUser(values);
        }

        public async Task<User> GetByResetToken(string token)
        {
            var values = await _dbAccess.SelectOne("SELECT " + _userDBValues +
                "FROM `Users` " +
                "WHERE `Reset` = @resetToken", new DBParam("resetToken", token));

            if (values == null) return null;

            return MapObjArrToUser(values);
        }
        public async Task<List<User>> GetAllUsersByLastActive()
        {
            var results = await _dbAccess.Select("SELECT " + _userDBValues +
                "FROM `Users` " +
                "ORDER BY " +
                "   `LastActive` DESC;");

            if (results.Count == 0) return new List<User>();

            var list = results[0];
            var rtn = new List<User>();

            foreach (var user in list)
                rtn.Add(MapObjArrToUser(user));

            return rtn;
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
                "`Token` = @Token, `Reset` = @Reset " +
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
