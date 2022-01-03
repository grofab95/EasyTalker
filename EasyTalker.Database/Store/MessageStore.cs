using System.Threading.Tasks;
using AutoMapper;
using EasyTalker.Core.Adapters;
using EasyTalker.Database.Entities;
using EasyTalker.Infrastructure.Dto.Message;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTalker.Database.Store;

public class MessageStore : IMessageStore
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMapper _mapper;

    public MessageStore(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _mapper = mapper;
    }

    public async Task<MessageDto> Add(MessageCreateDto messageCreateDto)
    {
        await using var dbContext = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<EasyTalkerContext>();

        var mappedMessage = _mapper.Map<MessageDb>(messageCreateDto);
        
        var messageDb = await dbContext.Messages.AddAsync(mappedMessage);
        await dbContext.SaveChangesAsync();
        
        return _mapper.Map<MessageDto>(messageDb);
    }
}