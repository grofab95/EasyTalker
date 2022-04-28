using System.Threading.Tasks;
using AutoMapper;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Core.Dto.User;
using EasyTalker.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTalker.Database.Store;

public class MessageStore : IMessageStore
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMapper _mapper;
    //private readonly UserManager<UserDb> _userManager;

    //public MessageStore(IServiceScopeFactory serviceScopeFactory, IMapper mapper, UserManager<UserDb> userManager)
    public MessageStore(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _mapper = mapper;
        //_userManager = userManager;
    }

    public async Task<MessageDto> Add(MessageCreateDto messageCreateDto)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();

        var mappedMessage = _mapper.Map<MessageDb>(messageCreateDto);
        var messageDb = await dbContext.Messages.AddAsync(mappedMessage);
        await dbContext.SaveChangesAsync();
        //var sender = await _userManager.FindByIdAsync(messageDb.Entity.SenderId);
        return new MessageDto
        {
            Id = messageDb.Entity.Id,
            ConversationId = messageDb.Entity.ConversationId,
            Status = messageDb.Entity.Status,
            Text = messageDb.Entity.Text,
            CreatedAt = messageDb.Entity.CreatedAt,
            //Sender = _mapper.Map<UserDto>(sender)
        };
    }
}