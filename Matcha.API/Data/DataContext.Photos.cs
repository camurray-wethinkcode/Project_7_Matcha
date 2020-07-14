using System.ComponentModel;
using System.Threading.Tasks;
using Matcha.API.Models;

namespace Matcha.API.Data
{
    public interface IPhotosDataContext
    {
        public Task<Photo> GetById(int id);
        public Task<Photo> GetMainForUser(int id);
        public Task<bool> Add(Photo photo);
        public Task<bool> Delete(int id);
    }

    public class PhotosDataContext : IPhotosDataContext
    {
        private readonly IDbAccess _dbAccess;

        public PhotosDataContext(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        private const string _photosDBValues = "`Id`, `Url`, `Description`, `DateAdded`, `IsMain`, `UserId`, `PublicId`";

        private Photo MapObjArrToUser(object[] objArr) => new Photo
        {
            Id = (int)objArr[0],
            Url = (string)objArr[1],
            Description = (string)objArr[2],
            DateAdded = (System.DateTime)objArr[3],
            IsMain = (bool)objArr[4],
            UserId = (int)objArr[5],
            PublicId = (string)objArr[6]
        };

        public async Task<Photo> GetById(int id)
        {
            var values = await _dbAccess.SelectOne("SELECT" + _photosDBValues +
                "FROM `Photos`" +
                "WHERE `Id` = @Id",
                new DBParam("Id", id));

            return MapObjArrToUser(values);
        }

        public async Task<Photo> GetMainForUser(int id)
        {
            var values = await _dbAccess.SelectOne("SELECT" + _photosDBValues +
                "FROM `Photos`" +
                "WHERE" +
                "   `UserId` = @UserId" +
                "AND" +
                "   `IsMain` = 1",
                new DBParam("UserId", id));

            return MapObjArrToUser(values);
        }

        public async Task<bool> Add(Photo photo)
        {
            var updateAmount = await _dbAccess.Insert("INSERT INTO `Photos` (" + _photosDBValues + ") VALUES (" +
                "@Id, @Url, @Description, @DateAdded, @IsMain, @UserId, @PublicId)",
                new DBParam("Id", photo.Id), new DBParam("Url", photo.Url), new DBParam("Description", photo.Description), new DBParam("DateAdded", photo.DateAdded),
                new DBParam("IsMain", photo.IsMain), new DBParam("UserId", photo.UserId), new DBParam("PublicId", photo.PublicId));

            return updateAmount == 1;
        }

        public async Task<bool> Delete(int id)
        {
            return await _dbAccess.Delete("DELETE FROM `Photos` WHERE `Id` = @Id", new DBParam("Id", id));
        }
    }
}
