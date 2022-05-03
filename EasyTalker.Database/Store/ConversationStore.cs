using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Core.Dto.User;
using EasyTalker.Core.Dto.UserConversation;
using EasyTalker.Core.Enums;
using EasyTalker.Database.Entities;

namespace EasyTalker.Database.Store;

public class ConversationStore : IConversationStore
{
    private readonly IMapper _mapper;
    private readonly IDbConnection _dbConnection;

    public ConversationStore(IMapper mapper, IDbConnection dbConnection)
    {
        _mapper = mapper;
        _dbConnection = dbConnection;
    }
    
    public async Task<ConversationDto[]> GetLoggedUserConversations(string loggedUserId)
    {
        var conversationsIds = (await _dbConnection.QueryAsync<long>(@"
            SELECT ConversationId
            FROM UsersConversations
            WHERE UserId = @userId", new { userId = loggedUserId })).ToArray();

        return await GetConversations(conversationsIds, loggedUserId);
    }

    public async Task<MessageDto[]> GetMessages(long conversationId)
    {
        const string query = @"
                SELECT m.Id, m.CreatedAt, m.SenderId, m.ConversationId, m.Text, m.Status, u.Id, u.UserName, u.Email, u.IsActive, u.IsOnline 
                FROM Messages m
                LEFT JOIN AspNetUsers u ON u.Id = m.SenderId
                WHERE m.ConversationId = @conversationId
                ORDER BY m.CreatedAt";
        
        var messages = (await _dbConnection.QueryAsync<MessageDto, UserDto, MessageDto>(
            query, 
            (message, user) =>
            {
                message.Sender = user;
                return message;
            },
            new { conversationId })).ToArray();

        return messages;
    }

    public async Task<ConversationDto> Add(ConversationCreateDto conversationCreateDto)
    {
        if (string.IsNullOrEmpty(conversationCreateDto.Title))
            throw new Exception("Title is required");

        if (string.IsNullOrEmpty(conversationCreateDto.CreatorId))
            throw new Exception("Creator is required");

        // if (await dbContext.Conversations.AnyAsync() && (!conversationCreateDto.ParticipantsId?.Any() ?? false))
        //     throw new Exception("At least one participant is required");
        
        var conversationDb = new ConversationDb
        {
            CreatorId = conversationCreateDto.CreatorId,
            Title = conversationCreateDto.Title
        };

        var createdConversationId = await _dbConnection.QuerySingleAsync<long>(@"
            DECLARE @InsertedRows AS TABLE (Id bigint);
            INSERT INTO Conversations (CreatorId, Title, Status) OUTPUT Inserted.Id INTO @InsertedRows
            VALUES (@creatorId, @title, @status);
            SELECT Id FROM @InsertedRows", new
        {
            creatorId = conversationDb.CreatorId,
            title = conversationDb.Title,
            status = ConversationStatus.Open.ToString()
        });

        conversationDb.Id = createdConversationId;
        
        var userConversations = conversationCreateDto.ParticipantsId
            .Select(x => new UserConversationDb(x, conversationDb.Id))
            .ToList();
        
        userConversations.Add(new UserConversationDb(conversationCreateDto.CreatorId, conversationDb.Id));

        foreach (var userConversation in userConversations)
        {
            await _dbConnection.QueryAsync<long>(@"
                INSERT INTO UsersConversations
                   (UserId,
                    ConversationId,
                    AccessStatus,
                    LastSeenAt)
                VALUES
                   (@userId, @conversationId, @accessStatus, @lastSeenAt)", new
                {
                    userId = userConversation.UserId,
                    conversationId = userConversation.ConversationId,
                    accessStatus = userConversation.AccessStatus.ToString(),
                    lastSeenAt = DateTime.Now.AddDays(-1)
                });
        }
        
        var conversation = _mapper.Map<ConversationDto>(conversationDb);
        conversation.CreatorId = conversationCreateDto.CreatorId;
        var conversationParticipants = new List<ConversationParticipantDto>
        {
            new(conversation.CreatorId, ConversationAccessStatus.ReadAndWrite)
        };
        
        conversationParticipants.AddRange(conversationCreateDto.ParticipantsId.Select(x => new ConversationParticipantDto(x, ConversationAccessStatus.ReadAndWrite)));
        conversation.Participants = conversationParticipants.ToArray();
        
        return conversation;
    }
    
    public async Task AddUsersToConversation(long conversationId, IEnumerable<string> userIds)
    {
        var newUserConversations = userIds
            .Select(x => new UserConversationDb(x, conversationId))
            .ToArray();
        
        foreach (var userConversation in newUserConversations)
        {
            await _dbConnection.QueryAsync<long>(@"
                INSERT INTO UsersConversations
                   (UserId,
                    ConversationId,
                    AccessStatus,
                    LastSeenAt)
                VALUES
                   (@userId, @conversationId, @accessStatus, @lastSeenAt)", new
            {
                userId = userConversation.UserId,
                conversationId = userConversation.ConversationId,
                accessStatus = userConversation.AccessStatus.ToString(),
                lastSeenAt = DateTime.Now.AddDays(-1)
            });
        }
    }

    public async Task UpdateUserConversationAccessStatus(long conversationId, string userId, ConversationAccessStatus accessStatus)
    {
        await _dbConnection.QueryAsync(@"
            UPDATE UsersConversations
            SET AccessStatus = @accessStatus
            WHERE ConversationId = @conversationId AND UserId = @userId", new
        {
            accessStatus = accessStatus.ToString(),
            conversationId,
            userId
        });
    }

    public async Task<ConversationLastSeenDto> UpdateLoggedUserConversationLastSeenAt(long conversationId, string loggedUserId)
    {
        await _dbConnection.QueryAsync(@"
            UPDATE UsersConversations
            SET LastSeenAt = @lastSeenAt
            WHERE ConversationId = @conversationId AND UserId = @userId", new
        {
            lastSeenAt = DateTime.Now.AddHours(-2),
            conversationId,
            userId = loggedUserId
        });

        var xx = await _dbConnection.QueryFirstOrDefaultAsync<ConversationLastSeenDto>(@"
            SELECT ConversationId, LastSeenAt
            FROM UsersConversations
            WHERE ConversationId = @conversationId AND UserId = @userId", new
        {
            conversationId,
            userId = loggedUserId
        });
        
        return xx;
    }

    public async Task UpdateConversationStatus(long conversationId, ConversationStatus status)
    {
        await _dbConnection.QueryAsync(@"
            UPDATE Conversations
            SET Status = @status
            WHERE Id = @conversationId", new
        {
            status = status.ToString(),
            conversationId
        });
    }
    
    public async Task<ConversationDto[]> GetConversations(long[] conversationsIds, string loggedUserId)
    {
        var conversations = await _dbConnection.QueryAsync<ConversationDto>(
            @"SELECT c.Id, c.Title, c.CreatorId, c.Status
                 FROM Conversations c
                 WHERE c.Id IN @conversationsIds",
            new {conversationsIds});
        
        var usersConversations = await _dbConnection.QueryAsync<UserConversationDto>(
            @"SELECT Id, UserId, ConversationId, AccessStatus, LastSeenAt, CreatedAt
                 FROM UsersConversations 
                 WHERE ConversationId IN @conversationsIds",
            new {conversationsIds});

        var lastMessagesData = (await _dbConnection.QueryAsync<LastMessageDto>(@"
            SELECT m.ConversationId, m.Id, m.Text, m.CreatedAt, m.Status
            FROM messages m
            WHERE id IN (
	            SELECT max(Id)
	            FROM Messages
	            GROUP BY ConversationId 
            )")).ToArray();

        var joinedData = (from conversation in conversations
            join userConversation in usersConversations on conversation.Id equals userConversation.ConversationId
            join lastMessageData in lastMessagesData on conversation.Id equals lastMessageData.ConversationId into lm
            from lmd in lm.DefaultIfEmpty()
            select new
            {
                Conversation = conversation, 
                UserConversation = userConversation, 
                LastMessageData = lmd
            }).ToArray();
        
        var result = joinedData
            .GroupBy(x => x.Conversation.Id)
            .Select(x => new ConversationDto
            {
                Id = x.Key,
                Status = x.First().Conversation.Status,
                Title = x.First().Conversation.Title,
                CreatorId = x.First().Conversation.CreatorId,
                LastMessage = x.FirstOrDefault()?.LastMessageData != null
                ?   new()
                    {
                        Id = x.First().LastMessageData.Id,
                        ConversationId = x.First().LastMessageData.ConversationId,
                        CreatedAt = x.First().LastMessageData.CreatedAt,
                        Status = x.First().LastMessageData.Status,
                        Text = x.First().LastMessageData.Text
                    }
                : null,
                LastSeenAt = x.First(y => y.UserConversation.UserId == loggedUserId).UserConversation.LastSeenAt,
                Participants = x
                    .Select(y => new ConversationParticipantDto(
                        y.UserConversation.UserId, 
                        Enum.Parse<ConversationAccessStatus>(y.UserConversation.AccessStatus)))
                    .ToArray()
            })
            .ToArray();
        
        return result;
    }
}