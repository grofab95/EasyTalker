using EasyTalker.Core.Dto.User;

namespace EasyTalker.Core.Dto;

public record AuthenticationResultDto(UserDto User, string AccessToken, string RefreshToken);