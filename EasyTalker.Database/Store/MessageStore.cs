using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Core.Dto.User;
using EasyTalker.Core.Enums;

namespace EasyTalker.Database.Store;

public class MessageStore : IMessageStore
{
    private readonly IDbConnection _dbConnection;

    public MessageStore(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<MessageDto> Add(MessageCreateDto messageCreateDto)
    {
        messageCreateDto.Status = MessageStatus.Send.ToString();
        
        var createdMessageId = await _dbConnection.QuerySingleAsync<long>(@"
                    DECLARE @InsertedRows AS TABLE (Id bigint);
                    INSERT INTO Messages (SenderId, ConversationId, Text, Status) OUTPUT Inserted.Id INTO @InsertedRows
                    VALUES (@senderId, @conversationId, @text, @status);
                    SELECT Id FROM @InsertedRows", new
        {
            messageCreateDto.SenderId,
            messageCreateDto.ConversationId,
            messageCreateDto.Text,
            messageCreateDto.Status
        });

        var query = @"
                SELECT m.Id, m.CreatedAt, m.SenderId, m.ConversationId, m.Text, m.Status, u.Id, u.UserName, u.Email, u.IsActive, u.IsOnline 
                FROM Messages m
                LEFT JOIN AspNetUsers u ON u.Id = m.SenderId
                WHERE m.Id = @id";
        
        var messageDb = (await _dbConnection.QueryAsync<MessageDto, UserDto, MessageDto>(
            query, 
            (message, user) =>
            {
                message.Sender = user;
                return message;
            },
        new { id = createdMessageId })).Single();

        return messageDb;
    }
}