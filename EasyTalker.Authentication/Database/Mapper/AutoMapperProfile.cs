using AutoMapper;
using EasyTalker.Authentication.Database.Entities;
using EasyTalker.Core.Dto.User;

namespace EasyTalker.Authentication.Database.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<UserDto, UserDb>();
        CreateMap<UserDb, UserDto>();
    }
}