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

        var conversations = await dbContext.Conversations
            .Where(x => conversationsIds.Contains(x.Id))
            .ToArrayAsync();

        return _mapper.Map<ConversationDto[]>(conversations);
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
            .Join(senders, x => x.SenderId, x => x.Id, (message, sender) => new {message = message, sender})
            .Select(x => new MessageDto
            {
                Id = x.message.Id,
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
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();
       
        var conversationDb = new ConversationDb
        {
            Title = conversationCreateDto.Title
        };

        await dbContext.Conversations.AddAsync(conversationDb);
        //await dbContext.SaveChangesAsync();

        var messageDb = _mapper.Map<MessageDb>(conversationCreateDto.Message);

        await dbContext.Messages.AddAsync(messageDb);
        
        var userConversations = new[]
        {
            new UserConversationDb(conversationCreateDto.CreatorId, conversationDb.Id),
            new UserConversationDb(conversationCreateDto.ParticipantId, conversationDb.Id),
        };

        await dbContext.UsersConversations.AddRangeAsync(userConversations);
        
        await dbContext.SaveChangesAsync();

        return _mapper.Map<ConversationDto>(conversationDb);
    }
}