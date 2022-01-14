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
    
    public async Task<ConversationDto[]> GetUserConversations(string userId)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();
        
        var conversationsIds = await dbContext.UsersConversations
            .Where(x => x.UserId == userId)
            .Select(x => x.ConversationId)
            .ToArrayAsync();

        return await GetConversations(dbContext, conversationsIds);

        // var usersConversations = await dbContext.UsersConversations
        //     .Where(x => x.UserId == userId)
        //     //.Select(x => x.ConversationId)
        //     .ToArrayAsync();
        //
        // var conversationsIds = usersConversations.Select(x => x.ConversationId).ToArray();
        // var conversationsDb = await dbContext.Conversations
        //     .Where(x => conversationsIds.Contains(x.Id))
        //     .ToArrayAsync();
        //
        // var conversationsUsers = await dbContext.UsersConversations
        //     .Where(x => conversationsIds.Contains(x.ConversationId))
        //     .ToArrayAsync();
        //
        // var conversations = _mapper.Map<ConversationDto[]>(conversationsDb);
        // var joined = conversations
        //     .Join(conversationsUsers, x => x.Id, x => x.ConversationId, (conversation, conversationUser) => new
        //     {
        //         conversation,
        //         conversationUser
        //     })
        //     .GroupBy(x => x.conversation.Id)
        //     .ToArray();
        //
        // foreach (var group in joined)
        // {
        //     var participants = group
        //         .Select(x => new ConversationParticipantDto(x.conversationUser.UserId.ToString(), x.conversationUser.HasAccess))
        //         .ToArray();
        //
        //     group.First().conversation.Participants = participants;
        // }
        //
        // return conversations;
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
        conversation.LastMessageAt = conversationDb.CreatedAt;

        conversation.CreatorId = conversationCreateDto.CreatorId;
        var conversationParticipants = new List<ConversationParticipantDto>
        {
            new(conversation.CreatorId, true)
        };
        
        
        conversationParticipants.AddRange(conversationCreateDto.ParticipantsId.Select(x => new ConversationParticipantDto(x, true)));

        conversation.Participants = conversationParticipants.ToArray();
        
        return conversation;
    }
    
    public async Task<ConversationDto> AddParticipant(long conversationId, string[] userIds)
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
        
        return await GetConversation(dbContext, conversationId);
    }

    public async Task<ConversationDto> RemoveParticipant(long conversationId, string[] participantsIds)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();
        
        var userConversationsDb = await dbContext.UsersConversations
            .Where(x => x.ConversationId == conversationId && participantsIds.Contains(x.UserId))
            .ToListAsync();
        
        userConversationsDb.ForEach(x => x.HasAccess = false);
        await dbContext.SaveChangesAsync();

        return await GetConversation(dbContext, conversationId);
    }

    private async Task<ConversationDto> GetConversation(EasyTalkerContext dbContext, long conversationId)
    {
        return (await GetConversations(dbContext, new[] {conversationId}))?.SingleOrDefault();
    }
    
    private async Task<ConversationDto[]> GetConversations(EasyTalkerContext dbContext, long[] conversationsIds)
    {
        var conversationsInfos = await dbContext.ConversationInfosView
            .Where(x => conversationsIds.Contains(x.ConversationId))
            .ToArrayAsync();

        return conversationsInfos.Select(x => x.ToConversationDto()).ToArray();
    }

    public async Task UpdateConversationLastSeenAt(long conversationId, string userId, DateTime seenAt)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();

        var userConversation = await dbContext.UsersConversations
            .FirstOrDefaultAsync(x => x.ConversationId == conversationId && x.UserId == userId)
                               ?? throw new Exception($"Conversation with id {conversationId} not exist");

        userConversation.LastSeenAt = seenAt;
        await dbContext.SaveChangesAsync();
    }
}