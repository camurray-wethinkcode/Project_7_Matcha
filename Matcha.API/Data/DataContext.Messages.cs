using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Matcha.API.Models;

namespace Matcha.API.Data
{
    public interface IMessagesDataContext
    {
        public Task<Message> GetById(long id);
        public Task<List<Message>> GetInbox(long recipientId);
        public Task<List<Message>> GetOutbox(long senderId);
        public Task<List<Message>> GetUnread(long recipientId);
        public Task<List<Message>> GetThread(long senderId, long recipientId);

        public Task<bool> Add(Message message);
        public Task<bool> Update(Message message);
        public Task<bool> Delete(long id);
    }

    public class MessagesDataContext : IMessagesDataContext
    {
        private readonly IDbAccess _dbAccess;

        public MessagesDataContext(IDbAccess dbAccess) => _dbAccess = dbAccess;

        private const string _messagesDBInsertValues = "`SenderId`, `RecipientId`, `Content`, `IsRead`, `DateRead`, `MessageSent`, `SenderDeleted`, `RecipientDeleted` ";
        private const string _messagesDBValues = "`Id`, " + _messagesDBInsertValues;

        private Message MapObjArrToMessage(object[] objArr) => new Message
        {
            Id = (long)objArr[0],
            SenderId = (long)objArr[1],
            RecipientId = (long)objArr[2],
            Content = (objArr[3].GetType() == typeof(string)) ? (string)objArr[3] : null,
            IsRead = Convert.ToBoolean((long)objArr[4]),
            DateRead = (objArr[5].GetType() == typeof(DateTime)) ? DateTime.Parse((string)objArr[5]) : (DateTime?)null,
            MessageSent = DateTime.Parse((string)objArr[6]),
            SenderDeleted = Convert.ToBoolean((long)objArr[7]),
            RecipientDeleted = Convert.ToBoolean((long)objArr[8])
        };

        public async Task<Message> GetById(long id)
        {
            var values = await _dbAccess.SelectOne("SELECT " + _messagesDBValues +
                "FROM `Messages` " +
                "WHERE `Id` = @Id",
                new DBParam("Id", id));

            if (values == null) return null;

            return MapObjArrToMessage(values);
        }

        public async Task<List<Message>> GetInbox(long recipientId)
        {
            var results = await _dbAccess.Select("SELECT " + _messagesDBValues +
                "FROM `Messages` " +
                "WHERE `RecipientId` = @RecipientId " +
                "AND `RecipientDeleted` = 0 ",
                new DBParam("RecipientId", recipientId));

            if (results.Count == 0) return new List<Message>();

            var list = results[0];
            var rtn = new List<Message>();

            foreach (var message in list)
            {
                rtn.Add(MapObjArrToMessage(message));
            }

            return rtn;
        }

        public async Task<List<Message>> GetOutbox(long senderId)
        {
            var results = await _dbAccess.Select("SELECT " + _messagesDBValues +
                "FROM `Messages` " +
                "WHERE `SenderId` = @SenderId " +
                "AND `SenderDeleted` = 0",
                new DBParam("SenderId", senderId));

            if (results.Count == 0) return new List<Message>();

            var list = results[0];
            var rtn = new List<Message>();

            foreach (var message in list)
            {
                rtn.Add(MapObjArrToMessage(message));
            }

            return rtn;
        }

        public async Task<List<Message>> GetUnread(long recipientId)
        {
            var results = await _dbAccess.Select("SELECT " + _messagesDBValues +
                "FROM `Messages` " +
                "WHERE `RecipientId` = @RecipientId " +
                "AND `RecipientDeleted` = 0 " +
                "AND `IsRead` = 0 ",
                new DBParam("RecipientId", recipientId));

            if (results.Count == 0) return new List<Message>();

            var list = results[0];
            var rtn = new List<Message>();

            foreach (var message in list)
                rtn.Add(MapObjArrToMessage(message));

            return rtn;
        }

        public async Task<List<Message>> GetThread(long senderId, long recipientId)
        {
            var results = await _dbAccess.Select("SELECT " + _messagesDBValues +
                "FROM `Messages` " +
                "WHERE " +
                "   (`SenderId` = @SenderId AND `RecipientId` = @RecipientId AND `SenderDeleted` = 0) " +
                "OR " +
                "   (`RecipientId` = @SenderId AND `SenderId` = @RecipientId AND `RecipientDeleted` = 0) " +
                "ORDER BY " +
                "   `MessageSent` DESC;",
                new DBParam("SenderId", senderId),
                new DBParam("RecipientId", recipientId));

            if (results.Count == 0) return new List<Message>();

            var list = results[0];
            var rtn = new List<Message>();

            foreach (var message in list)
                rtn.Add(MapObjArrToMessage(message));

            return rtn;
        }

        public async Task<bool> Add(Message message)
        {
            var updateAmount = await _dbAccess.NonQuery("INSERT INTO `Messages` (" + _messagesDBInsertValues + ") VALUES (" +
                "@SenderId, @RecipientId, @Content, @IsRead, @DateRead, @MessageSent, @SenderDeleted, @RecipientDeleted)",
                new DBParam("SenderId", message.SenderId), new DBParam("RecipientId", message.RecipientId), new DBParam("Content", message.Content), new DBParam("IsRead", message.IsRead),
                new DBParam("DateRead", message.DateRead), new DBParam("MessageSent", message.MessageSent), new DBParam("SenderDeleted", message.SenderDeleted), new DBParam("RecipientDeleted", message.RecipientDeleted));

            return updateAmount == 1;
        }

        public async Task<bool> Update(Message message)
        {
            var updateAmount = await _dbAccess.NonQuery("UPDATE `Messages` SET " +
                "   `Id` = @Id, `SenderId` = @SenderId, `RecipientId` = @RecipientId, `Content` = @Content, `IsRead` = @IsRead," +
                "   `DateRead` = @DateRead, `MessageSent` = @MessageSent, `SenderDeleted` = @SenderDeleted, `RecipientDeleted` = @RecipientDeleted " +
                "WHERE `Id` = @Id",
                new DBParam("Id", message.Id), new DBParam("SenderId", message.SenderId), new DBParam("RecipientId", message.RecipientId), new DBParam("Content", message.Content), new DBParam("IsRead", message.IsRead),
                new DBParam("DateRead", message.DateRead), new DBParam("MessageSent", message.MessageSent), new DBParam("SenderDeleted", message.SenderDeleted), new DBParam("RecipientDeleted", message.RecipientDeleted));

            return updateAmount == 1;
        }

        public async Task<bool> Delete(long id)
        {
            var updateAmount = await _dbAccess.NonQuery("DELETE FROM `Messages` WHERE `Id` = @Id", new DBParam("Id", id));

            return updateAmount == 1;
        }
    }
}
