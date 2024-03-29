﻿using AutoMapper;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Core.Dto.User;
using EasyTalker.Database.Entities;

namespace EasyTalker.Database.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<UserDto, UserDb>();
        CreateMap<UserDb, UserDto>();
        
        CreateMap<MessageDto, MessageDb>();
        CreateMap<MessageDb, MessageDto>();
        
        CreateMap<MessageCreateDto, MessageDb>();
        CreateMap<MessageDb, MessageCreateDto>();
        
        CreateMap<ConversationDto, ConversationDb>();
        CreateMap<ConversationDb, ConversationDto>();
    }
}