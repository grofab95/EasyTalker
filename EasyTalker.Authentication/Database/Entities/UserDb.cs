using EasyTalker.Core.Dto.User;
using Microsoft.AspNetCore.Identity;

namespace EasyTalker.Authentication.Database.Entities;

public class UserDb : IdentityUser
{
    public bool IsActive { get; set; }
    public bool IsOnline { get; set; }

    public UserDto ToUserDto()
    {
        return new UserDto
        {
            Id = Id,
            UserName = UserName,
            Email = Email,
            IsActive = IsActive,
            IsOnline = IsOnline
        };
    }
}