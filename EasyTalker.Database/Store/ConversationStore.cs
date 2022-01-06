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
        
        var usersConversations = await dbContext.UsersConversations
            .Where(x => x.UserId == userId)
            //.Select(x => x.ConversationId)
            .ToArrayAsync();

        var conversationsIds = usersConversations.Select(x => x.ConversationId).ToArray();
        var conversationsDb = await dbContext.Conversations
            .Where(x => conversationsIds.Contains(x.Id))
            .ToArrayAsync();

        var conversationsUsers = await dbContext.UsersConversations
            .Where(x => conversationsIds.Contains(x.ConversationId))
            .ToArrayAsync();

        
        var conversations = _mapper.Map<ConversationDto[]>(conversationsDb);
        var joined = conversations
            .Join(conversationsUsers, x => x.Id, x => x.ConversationId, (conversation, conversationUser) => new
            {
                conversation,
                conversationUser
            })
            .GroupBy(x => x.conversation.Id)
            .ToArray();

        foreach (var group in joined)
        {
            var participantsIds = group.Select(x => x.conversationUser.UserId).ToArray();
            group.First().conversation.ParticipantsId = participantsIds;
        }

        return conversations;
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
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();
       
        var conversationDb = new ConversationDb
        {
            //CreatorId = conversationCreateDto.CreatorId,
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

        conversation.CreatorId = conversationCreateDto.CreatorId;
        var participantsId = new List<string>
        {
            conversationCreateDto.CreatorId
        };
        
        participantsId.AddRange(conversationCreateDto.ParticipantsId);

        conversation.ParticipantsId = participantsId.ToArray();
        
        
        return conversation;
    }
}