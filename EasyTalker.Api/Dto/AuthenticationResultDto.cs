using EasyTalker.Infrastructure.Dto;
using EasyTalker.Infrastructure.Dto.User;

namespace EasyTalker.Api.Dto
{
    public record AuthenticationResultDto(UserDto UserDto, string AccessToken, string RefreshToken);
}