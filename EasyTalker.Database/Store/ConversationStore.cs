using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Core.Dto.User;
using EasyTalker.Core.Enums;
using EasyTalker.Database.Entities;
using EasyTalker.Database.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTalker.Database.Store;

public class ConversationStore : IConversationStore
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMapper _mapper;

    public ConversationStore(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _mapper = mapper;
    }
    
    public async Task<ConversationDto[]> GetLoggedUserConversations(string loggedUserId)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();
        
        var conversationsIds = await dbContext.UsersConversations
            .Where(x => x.UserId == loggedUserId)
            .Select(x => x.ConversationId)
            .ToArrayAsync();

        return await GetConversations(conversationsIds, loggedUserId);
    }

    public async Task<MessageDto[]> GetMessages(long conversationId)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();

        var messages = await dbContext.Messages
            .Where(x => x.ConversationId == conversationId)
            .ToArrayAsync();

        var sendersIds = messages.Select(x => x.SenderId).Distinct().ToArray();
        var senders = await dbContext.Users
            .Where(x => sendersIds.Contains(x.Id))
            .ToArrayAsync();

        var messagesDto = messages
            .Join(senders, x => x.SenderId, x => x.Id, (message, sender) => new {message, sender})
            .Select(x => new MessageDto
            {
                Id = x.message.Id,
                ConversationId = x.message.ConversationId,
                CreatedAt = x.message.CreatedAt,
                Sender = _mapper.Map<UserDto>(x.sender),
                Status = x.message.Status,
                Text = x.message.Text
            })
            .OrderBy(x => x.CreatedAt)
            .ToArray();

        return messagesDto;
    }

    public async Task<ConversationDto> Add(ConversationCreateDto conversationCreateDto)
    {
        if (string.IsNullOrEmpty(conversationCreateDto.Title))
            throw new Exception("Title is required");

        if (string.IsNullOrEmpty(conversationCreateDto.CreatorId))
            throw new Exception("Creator is required");

        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();
        
        if (await dbContext.Conversations.AnyAsync() && (!conversationCreateDto.ParticipantsId?.Any() ?? false))
            throw new Exception("At least one participant is required");
        
        var conversationDb = new ConversationDb
        {
            CreatorId = conversationCreateDto.CreatorId,
            Title = conversationCreateDto.Title
        };

        await dbContext.Conversations.AddAsync(conversationDb);
        await dbContext.SaveChangesAsync();

        var userConversations = conversationCreateDto.ParticipantsId
            .Select(x => new UserConversationDb(x, conversationDb.Id))
            .ToList();
        
        userConversations.Add(new UserConversationDb(conversationCreateDto.CreatorId, conversationDb.Id));
        await dbContext.UsersConversations.AddRangeAsync(userConversations);
        await dbContext.SaveChangesAsync();
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
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();

        var newUserConversations = userIds
            .Select(x => new UserConversationDb(x, conversationId))
            .ToArray();
        
        await dbContext.UsersConversations.AddRangeAsync(newUserConversations);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateUserConversationAccessStatus(long conversationId, string userId, ConversationAccessStatus accessStatus)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();
         
        var userConversationsDb = await dbContext.UsersConversations
            .FirstOrDefaultAsync(x => x.ConversationId == conversationId && x.UserId == userId)
                ?? throw new Exception($"User with id {userId} is not member od conversation id {conversationId}");
        
        userConversationsDb.AccessStatus = accessStatus;
        await dbContext.SaveChangesAsync();
    }

    public async Task<ConversationLastSeenDto> UpdateLoggedUserConversationLastSeenAt(long conversationId, string loggedUserId)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();

        var userConversation = await dbContext.UsersConversations
            .FirstOrDefaultAsync(x => x.ConversationId == conversationId && x.UserId == loggedUserId)
                               ?? throw new Exception($"Conversation with id {conversationId} not exist");

        userConversation.LastSeenAt = DateTime.Now;
        await dbContext.SaveChangesAsync();
        return new ConversationLastSeenDto
        {
            ConversationId = conversationId,
            LastSeenAt = userConversation.LastSeenAt
        };
    }

    public async Task UpdateConversationStatus(long conversationId, ConversationStatus status)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();

        var conversation = await dbContext.Conversations.FindAsync(conversationId)
                           ?? throw new Exception($"Conversation with id {conversationId} not exist");

        conversation.Status = status;
        await dbContext.SaveChangesAsync();
    }
    
    public async Task<ConversationDto[]> GetConversations(long[] conversationsIds, string loggedUserId)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();
        
        var conversationsInfos = await dbContext.ConversationInfosView
            .Where(x => conversationsIds.Contains(x.ConversationId))
            .ToArrayAsync();

        var messagesIds = conversationsInfos
            .Where(x => x.LastMessageAt != null)
            .Select(x => x.LastMessageId).Distinct().ToArray();
        
        var lastMessages = await dbContext.Messages
            .Where(x => messagesIds.Contains(x.Id))
            .Select(x => new MessageDto
            {   
                Id = x.Id,
                ConversationId = x.ConversationId,
                Text = x.Text,
                Status = x.Status,
                CreatedAt = x.CreatedAt,
                Sender = new UserDto
                {
                    Id = x.SenderId
                }
            })
            .ToArrayAsync();

        var lastSeenAtList = await dbContext.UsersConversations
            .Where(x => x.UserId == loggedUserId && conversationsIds.Contains(x.ConversationId))
            .Select(x => new {x.ConversationId, x.LastSeenAt})
            .ToArrayAsync();
        
        var conversations =  conversationsInfos
            .Join(lastSeenAtList, x => x.ConversationId, x => x.ConversationId, (a, b) => new { a, b })
            .Select(x => x.a.ToConversationDto(x.b.LastSeenAt))
            .ToArray();

        conversations
            .Join(lastMessages, x => x.Id, x => x.ConversationId,
                (conversation, lastMessage) => new {conversation, lastMessage})
            .ToList()
            .ForEach(x => x.conversation.LastMessage = x.lastMessage);
        
        return conversations;
    }
}