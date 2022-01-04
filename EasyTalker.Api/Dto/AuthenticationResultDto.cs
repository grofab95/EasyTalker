using EasyTalker.Core.Dto.User;

namespace EasyTalker.Api.Dto;

public record AuthenticationResultDto(UserDto User, string AccessToken, string RefreshToken);