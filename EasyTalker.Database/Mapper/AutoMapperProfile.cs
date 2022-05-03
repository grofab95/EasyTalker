using AutoMapper;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Database.Entities;

namespace EasyTalker.Database.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<MessageDto, MessageDb>();
        CreateMap<MessageDb, MessageDto>();
        
        CreateMap<MessageCreateDto, MessageDb>();
        CreateMap<MessageDb, MessageCreateDto>();
        
        CreateMap<ConversationDto, ConversationDb>();
        CreateMap<ConversationDb, ConversationDto>();
    }
}