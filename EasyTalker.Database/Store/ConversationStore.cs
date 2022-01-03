using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EasyTalker.Core.Adapters;
using EasyTalker.Database.Entities;
using EasyTalker.Infrastructure.Dto.Conversation;
using EasyTalker.Infrastructure.Dto.Message;
using EasyTalker.Infrastructure.Dto.User;
using Microsoft.AspNetCore.Identity;
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
        var user = await _userManager.FindByIdAsync(userId);
        var userDto = _mapper.Map<UserDto>(user);
        var fakeUser = new UserDto
        {
            UserName = "Worms",
            Email = "test@easytalker.pl",
            IsActive = "true"
        };
        
        return new List<ConversationDto>
        {
            new ConversationDto
            {
                Id = 1,
                Participants = new []{ userDto, fakeUser },
                Title = "Hello World",
                CreatedAt = DateTime.Now.AddDays(-7),
                UpdatedAt = DateTime.Now,
                Messages = new []
                {
                    new MessageDto
                    {
                        Id =  1,
                        Sender = fakeUser,
                        Status = MessageStatus.Read.ToString(),
                        Text = "Witam"
                    },
                    
                    new MessageDto
                    {
                        Id = 2,
                        Sender = userDto,
                        Status = MessageStatus.Read.ToString(),
                        Text = "Dzień dobry"
                    },
                    
                    new MessageDto
                    {
                        Id = 3,
                        Sender = fakeUser,
                        Status = MessageStatus.Read.ToString(),
                        Text = "Lorem Ipsum is simply dummy text of the printing and typesetting industry."
                    },
                    
                    new MessageDto
                    {
                        Id = 4,
                        Sender = userDto,
                        Status = MessageStatus.Read.ToString(),
                        Text = "XD"
                    },
                    
                    new MessageDto
                    {
                        Id = 5,
                        Sender = userDto,
                        Status = MessageStatus.Read.ToString(),
                        Text = "The standard chunk of Lorem Ipsum used since the 1500s is reproduced below for those interested. "
                    },
                }
            },
            
            new ConversationDto
            {
                Id = 2,
                Participants = new []{ userDto, fakeUser },
                Title = "Sylwester",
                CreatedAt = DateTime.Now.AddDays(-7),
                UpdatedAt = DateTime.Now,
                Messages = new []
                {
                    new MessageDto
                    {
                        Id =  1,
                        Sender = fakeUser,
                        Status = MessageStatus.Read.ToString(),
                        Text = "Witam"
                    },
                    
                    new MessageDto
                    {
                        Id = 2,
                        Sender = userDto,
                        Status = MessageStatus.Read.ToString(),
                        Text = "Dzień dobry"
                    },
                    
                    new MessageDto
                    {
                        Id = 3,
                        Sender = fakeUser,
                        Status = MessageStatus.Read.ToString(),
                        Text = "Lorem Ipsum is simply dummy text of the printing and typesetting industry."
                    },
                    
                    new MessageDto
                    {
                        Id = 4,
                        Sender = userDto,
                        Status = MessageStatus.Read.ToString(),
                        Text = "XD"
                    },
                    
                    new MessageDto
                    {
                        Id = 5,
                        Sender = userDto,
                        Status = MessageStatus.Read.ToString(),
                        Text = "The standard chunk of Lorem Ipsum used since the 1500s is reproduced below for those interested. "
                    }
                }
            }
        }.ToArray();
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