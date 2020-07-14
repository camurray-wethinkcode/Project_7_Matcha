using System.Collections.Generic;
using System.Threading.Tasks;
using Matcha.API.Models;

namespace Matcha.API.Data
{
    public interface IMessagesDataContext
    {
        public Task<Message> GetById(int id);
        public Task<List<Message>> GetInbox(int recipientId);
        public Task<List<Message>> GetOutbox(int senderId);
        public Task<List<Message>> GetUnread(int recipientId);
        public Task<List<Message>> GetThread(int senderId, int recipientId);

        public Task<bool> Add(Message message);
        public Task<bool> Update(Message message);
        public Task<bool> Delete(int id);
    }

    public class MessagesDataContext : IMessagesDataContext
    {
        private readonly IDbAccess _dbAccess;

        public MessagesDataContext(IDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        private const string _messagesDBValues = "`Id`, `SenderId`, `RecipientId`, `Content`, `IsRead`, `DateRead`, `MessageSent`, `SenderDeleted`, `RecipientDeleted`";

        private Message MapObjArrToMessage(object[] objArr) => new Message
        {
            Id = (int)objArr[0],
            SenderId = (int)objArr[1],
            RecipientId = (int)objArr[2],
            Content = (string)objArr[3],
            IsRead = (bool)objArr[4],
            DateRead = (System.DateTime?)objArr[5],
            MessageSent = (System.DateTime)objArr[6],
            SenderDeleted = (bool)objArr[7],
            RecipientDeleted = (bool)objArr[8]
        };

        public async Task<Message> GetById(int id)
        {
            var values = await _dbAccess.SelectOne("SELECT" + _messagesDBValues +
                "FROM `Messages`" +
                "WHERE `Id` = @Id",
                new DBParam("Id", id));

            return MapObjArrToMessage(values);
        }

        public async Task<List<Message>> GetInbox(int recipientId)
        {
            var results = await _dbAccess.Select("SELECT" + _messagesDBValues +
                "FROM `Messages`" +
                "WHERE `RecipientId` = @RecipientId" +
                "AND `RecipientDeleted` = 0",
                new DBParam("RecipientId", recipientId));

            var list = results[0];
            var rtn = new List<Message>();

            foreach (var message in list)
            {
                rtn.Add(MapObjArrToMessage(message));
            }

            return rtn;
        }

        public async Task<List<Message>> GetOutbox(int senderId)
        {
            var results = await _dbAccess.Select("SELECT" + _messagesDBValues +
                "FROM `Messages`" +
                "WHERE `SenderId` = @SenderId" +
                "AND `SenderDeleted` = 0",
                new DBParam("SenderId", senderId));

            var list = results[0];
            var rtn = new List<Message>();

            foreach (var message in list)
            {
                rtn.Add(MapObjArrToMessage(message));
            }

            return rtn;
        }

        public async Task<List<Message>> GetUnread(int recipientId)
        {
            var results = await _dbAccess.Select("SELECT" + _messagesDBValues +
                "FROM `Messages`" +
                "WHERE `RecipientId` = @RecipientId" +
                "AND `RecipientDeleted` = 0" +
                "AND `IsRead` = 0",
                new DBParam("RecipientId", recipientId));

            var list = results[0];
            var rtn = new List<Message>();

            foreach (var message in list)
                rtn.Add(MapObjArrToMessage(message));

            return rtn;
        }

        public async Task<List<Message>> GetThread(int senderId, int recipientId)
        {
            var results = await _dbAccess.Select("SELECT" + _messagesDBValues +
                "FROM `Messages`" +
                "WHERE" +
                "   (`SenderId` = @SenderId AND `RecipientId` = @RecipientId AND `SenderDeleted` = 0)" +
                "OR" +
                "   (`RecipientId` = @SenderId AND `SenderId` = @RecipientId AND `RecipientDeleted` = 0)" +
                "ORDER BY" +
                "   `MessageSent` DESC;",
                new DBParam("SenderId", senderId),
                new DBParam("RecipientId", recipientId));

            var list = results[0];
            var rtn = new List<Message>();

            foreach (var message in list)
                rtn.Add(MapObjArrToMessage(message));

            return rtn;
        }

        public async Task<bool> Add(Message message)
        {
            var updateAmount = await _dbAccess.Insert("INSERT INTO `Messages` (" + _messagesDBValues + ") VALUES (" +
                "@Id, @SenderId, @RecipientId, @Content, @IsRead, @DateRead, @MessageSent, @SenderDeleted, @RecipientDeleted)",
                new DBParam("Id", message.Id), new DBParam("SenderId", message.SenderId), new DBParam("RecipientId", message.RecipientId), new DBParam("Content", message.Content), new DBParam("IsRead", message.IsRead),
                new DBParam("DateRead", message.DateRead), new DBParam("MessageSent", message.MessageSent), new DBParam("SenderDeleted", message.SenderDeleted), new DBParam("RecipientDeleted", message.RecipientDeleted));

            return updateAmount == 1;
        }

        public async Task<bool> Update(Message message)
        {
            var updateAmount = await _dbAccess.Update("UPDATE `Users` SET " +
                "   `Id` = @Id, `SenderId` = @SenderId, `RecipientId` = @RecipientId, `Content` = @Content, `IsRead` = @IsRead," +
                "   `DateRead` = @DateRead, `MessageSent` = @MessageSent, `SenderDeleted` = @SenderDeleted, `RecipientDeleted` = @RecipientDeleted" +
                "WHERE `Id` = @Id",
                new DBParam("Id", message.Id), new DBParam("SenderId", message.SenderId), new DBParam("RecipientId", message.RecipientId), new DBParam("Content", message.Content), new DBParam("IsRead", message.IsRead),
                new DBParam("DateRead", message.DateRead), new DBParam("MessageSent", message.MessageSent), new DBParam("SenderDeleted", message.SenderDeleted), new DBParam("RecipientDeleted", message.RecipientDeleted));

            return updateAmount == 1;
        }

        public async Task<bool> Delete(int id)
        {
            return await _dbAccess.Delete("DELETE FROM `Messages` WHERE `Id` = @Id", new DBParam("Id", id));
        }
    }
}
