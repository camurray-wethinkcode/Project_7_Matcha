using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Matcha.API.Models;

namespace Matcha.API.Data
{
    public interface IPhotosDataContext
    {
        public Task<Photo> GetById(long id);
        public Task<Photo> GetMainForUser(long id);
        public Task<List<Photo>> GetAllForUser(long userid);
        public Task<bool> Add(Photo photo);
        public Task<bool> Update(Photo photo);
        public Task<bool> Delete(long id);
    }

    public class PhotosDataContext : IPhotosDataContext
    {
        private readonly IDbAccess _dbAccess;

        public PhotosDataContext(IDbAccess dbAccess) => _dbAccess = dbAccess;

        private const string _photosDBValues = "`Id`, `Url`, `Description`, `DateAdded`, `IsMain`, `UserId`, `PublicId` ";

        private Photo MapObjArrToPhoto(object[] objArr) => new Photo
        {
            Id = (long)objArr[0],
            Url = (objArr[1].GetType() == typeof(string)) ? (string)objArr[1] : null,
            Description = (objArr[2].GetType() == typeof(string)) ? (string)objArr[2] : null,
            DateAdded = DateTime.Parse((string)objArr[3]),
            IsMain = Convert.ToBoolean((long)objArr[4]),
            UserId = (long)objArr[5],
            PublicId = (objArr[6].GetType() == typeof(string)) ? (string)objArr[6] : null
        };

        public async Task<Photo> GetById(long id)
        {
            var values = await _dbAccess.SelectOne("SELECT " + _photosDBValues +
                "FROM `Photos` " +
                "WHERE `Id` = @Id",
                new DBParam("Id", id));

            if (values == null) return null;

            return MapObjArrToPhoto(values);
        }

        public async Task<Photo> GetMainForUser(long id)
        {
            var values = await _dbAccess.SelectOne("SELECT " + _photosDBValues +
                "FROM `Photos` " +
                "WHERE " +
                "   `UserId` = @UserId " +
                "AND " +
                "   `IsMain` = 1",
                new DBParam("UserId", id));

            if (values == null) return null;

            return MapObjArrToPhoto(values);
        }

        public async Task<List<Photo>> GetAllForUser(long userid)
        {
            var results = await _dbAccess.Select("SELECT " + _photosDBValues +
                "FROM `Photos` " +
                "WHERE " +
                "   `UserId` = @UserId",
                new DBParam("UserId", userid));

            if (results.Count == 0) return new List<Photo>();

            var list = results[0];
            var rtn = new List<Photo>();

            foreach (var photo in list)
                rtn.Add(MapObjArrToPhoto(photo));

            return rtn;
        }

        public async Task<bool> Add(Photo photo)
        {
            var updateAmount = await _dbAccess.Insert("INSERT INTO `Photos` (" + _photosDBValues + ") VALUES (" +
                "@Id, @Url, @Description, @DateAdded, @IsMain, @UserId, @PublicId)",
                new DBParam("Id", photo.Id), new DBParam("Url", photo.Url), new DBParam("Description", photo.Description), new DBParam("DateAdded", photo.DateAdded),
                new DBParam("IsMain", photo.IsMain), new DBParam("UserId", photo.UserId), new DBParam("PublicId", photo.PublicId));

            return updateAmount == 1;
        }

        public async Task<bool> Update(Photo photo)
        {
            var updateAmount = await _dbAccess.Update("UPDATE `Users` SET " +
                "   `Id` = @Id, `Url` = @Url, `Description` = @Description, `DateAdded` = @DateAdded, " +
                "   `IsMain` = @IsMain, `UserId` = @UserId, `PublicId` = @PublicId " +
                "WHERE `Id` = @Id",
                new DBParam("Id", photo.Id), new DBParam("Url", photo.Url), new DBParam("Description", photo.Description), new DBParam("DateAdded", photo.DateAdded),
                new DBParam("IsMain", photo.IsMain), new DBParam("UserId", photo.UserId), new DBParam("PublicId", photo.PublicId));

            return updateAmount == 1;
        }

        public async Task<bool> Delete(long id)
        {
            return await _dbAccess.Delete("DELETE FROM `Photos` WHERE `Id` = @Id", new DBParam("Id", id));
        }
    }
}
