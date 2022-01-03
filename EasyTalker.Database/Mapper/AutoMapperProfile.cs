using AutoMapper;
using EasyTalker.Database.Entities;
using EasyTalker.Infrastructure.Dto.Conversation;
using EasyTalker.Infrastructure.Dto.Message;
using EasyTalker.Infrastructure.Dto.User;

namespace EasyTalker.Database.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<UserDto, UserDb>();
        CreateMap<UserDb, UserDto>();
        
        CreateMap<MessageDto, MessageDb>();
        CreateMap<MessageDb, MessageDto>();
        
        CreateMap<ConversationDto, ConversationDb>();
        CreateMap<ConversationDb, ConversationDto>();
    }
}