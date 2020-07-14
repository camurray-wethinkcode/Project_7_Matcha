using System.Threading.Tasks;
using Matcha.API.Models;

namespace Matcha.API.Data
{
    public interface ILikesDataContext
    {
        public Task<Like> Get(long likerId, long likeeId);
        public Task<bool> Add(Like like);
        public Task<bool> Update(Like like);
        public Task<bool> Delete(long likerId, long likeeId);
    }

    public class LikesDataContext : ILikesDataContext
    {
        private readonly IDbAccess _dbAccess;

        public LikesDataContext(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        private const string _likesDBValues = "`LikerId`, `LikeeId` ";

        public async Task<Like> Get(long likerId, long likeeId)
        {
            var values = await _dbAccess.SelectOne("SELECT " + _likesDBValues +
                "FROM `Likes` " +
                "WHERE " +
                "   `LikerId` = @LikerId " +
                "AND " +
                "   `LikeeId` = @LikeeId ",
                new DBParam("LikerId", likerId),
                new DBParam("LikeeId", likeeId));

            if (values == null) return null;

            return new Like
            {
                LikerId = (long)values[0],
                LikeeId = (long)values[1]
            };
        }

        public async Task<bool> Add(Like like)
        {
            var updateAmount = await _dbAccess.Insert("INSERT INTO `Likes` (" + _likesDBValues + ") VALUES (@LikerId, @LikeeId)",
                new DBParam("LikerId", like.LikerId), new DBParam("LikeeId", like.LikeeId));

            return updateAmount == 1;
        }

        public async Task<bool> Update(Like like)
        {
            var updateAmount = await _dbAccess.Update("UPDATE `Users` SET " +
                "   `LikerId` = @LikerId, `LikeeId` = @LikeeId " +
                "WHERE `Id` = @Id",
                new DBParam("LikerId", like.LikerId), new DBParam("LikeeId", like.LikeeId));

            return updateAmount == 1;
        }

        public async Task<bool> Delete(long likerId, long likeeId)
        {
            return await _dbAccess.Delete("DELETE FROM `Likes` WHERE " +
                "   `LikerId` = @LikerId " +
                "AND" +
                "   `LikeeId` = @LikeeId ",
                new DBParam("LikerId", likerId),
                new DBParam("LikeeId", likeeId));
        }
    }
}
