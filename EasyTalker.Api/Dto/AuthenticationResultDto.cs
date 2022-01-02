using EasyTalker.Infrastructure.Dto;
using EasyTalker.Infrastructure.Dto.User;

namespace EasyTalker.Api.Dto;

public record AuthenticationResultDto(UserDto User, string AccessToken, string RefreshToken);