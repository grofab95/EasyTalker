using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Core.Dto.User;
using EasyTalker.Database.Entities;
using EasyTalker.Database.Extensions;
using EasyTalker.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTalker.Database.Store;

public class ConversationStore : IConversationStore
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly UserManager<UserDb> _userManager;
    private readonly IMapper _mapper;

    public ConversationStore(IServiceScopeFactory serviceScopeFactory, UserManager<UserDb> userManager, IMapper mapper)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _userManager = userManager;
        _mapper = mapper;
    }
    
    public async Task<ConversationDto[]> GetUserConversations(string loggedUserId)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();
        
        var conversationsIds = await dbContext.UsersConversations
            .Where(x => x.UserId == loggedUserId)
            .Select(x => x.ConversationId)
            .ToArrayAsync();

        return await GetConversations(dbContext, conversationsIds, loggedUserId);
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

        if (!conversationCreateDto.ParticipantsId?.Any() ?? false)
            throw new Exception("At least one participant is required");
        
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();
       
        var conversationDb = new ConversationDb
        {
            CreatorId = conversationCreateDto.CreatorId,
            Title = conversationCreateDto.Title
        };

        await dbContext.Conversations.AddAsync(conversationDb);
        await dbContext.SaveChangesAsync();

        // var messageDb = _mapper.Map<MessageDb>(conversationCreateDto.Message);
        //
        // await dbContext.Messages.AddAsync(messageDb);

        var userConversations = conversationCreateDto.ParticipantsId
            .Select(x => new UserConversationDb(x, conversationDb.Id))
            .ToList();
        
        userConversations.Add(new UserConversationDb(conversationCreateDto.CreatorId, conversationDb.Id));
        
        await dbContext.UsersConversations.AddRangeAsync(userConversations);
        
        await dbContext.SaveChangesAsync();

        var conversation = _mapper.Map<ConversationDto>(conversationDb);
        //conversation.LastMessageAt = conversationDb.CreatedAt;

        conversation.CreatorId = conversationCreateDto.CreatorId;
        var conversationParticipants = new List<ConversationParticipantDto>
        {
            new(conversation.CreatorId, true)
        };
        
        conversationParticipants.AddRange(conversationCreateDto.ParticipantsId.Select(x => new ConversationParticipantDto(x, true)));

        conversation.Participants = conversationParticipants.ToArray();
        
        return conversation;
    }
    
    public async Task<ConversationDto> AddParticipant(long conversationId, string[] userIds, string loggedUserId)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();

        var userConversationsDb = await dbContext.UsersConversations
            .Where(x => x.ConversationId == conversationId)
            .ToListAsync();

        var existing = userConversationsDb.GetSame(userIds, x => x.UserId, x => x).ToList();
        existing.ForEach(x => x.HasAccess = true);

        //dbContext.UpdateRange(existing);
        
        var newUserConversations = userIds
            .GetUniques(userConversationsDb, x => x, x => x.UserId)
            .Select(x => new UserConversationDb(x, conversationId))
            .ToArray();
        
        await dbContext.UsersConversations.AddRangeAsync(newUserConversations);
        await dbContext.SaveChangesAsync();
        
        return await GetConversation(dbContext, conversationId, loggedUserId);
    }

    public async Task<ConversationDto> RemoveParticipant(long conversationId, string[] participantsIds, string loggedUserId)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();
        
        var userConversationsDb = await dbContext.UsersConversations
            .Where(x => x.ConversationId == conversationId && participantsIds.Contains(x.UserId))
            .ToListAsync();
        
        userConversationsDb.ForEach(x => x.HasAccess = false);
        await dbContext.SaveChangesAsync();

        return await GetConversation(dbContext, conversationId, loggedUserId);
    }

    private async Task<ConversationDto> GetConversation(EasyTalkerContext dbContext, long conversationId, string loggedUserId)
    {
        return (await GetConversations(dbContext, new[] {conversationId}, loggedUserId))?.SingleOrDefault();
    }
    
    private async Task<ConversationDto[]> GetConversations(EasyTalkerContext dbContext, long[] conversationsIds, string loggedUserId)
    {
        var conversationsInfos = await dbContext.ConversationInfosView
            .Where(x => conversationsIds.Contains(x.ConversationId))
            .ToArrayAsync();

        var messagesIds = conversationsInfos.Select(x => x.LastMessageId).Distinct().ToArray();
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
        
        return conversationsInfos
            .Join(lastMessages, x => x.ConversationId, x => x.ConversationId, (conversationInfo, lastMessage) => new { conversationInfo, lastMessage })
            .Join(lastSeenAtList, x => x.conversationInfo.ConversationId, x => x.ConversationId, (a, b) => new { a,b })
            .Select(x => x.a.conversationInfo.ToConversationDto(x.a.lastMessage, x.b.LastSeenAt))
            .ToArray();
    }

    public async Task<ConversationLastSeenDto> UpdateConversationLastSeenAt(long conversationId, string loggedUserId)
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
}