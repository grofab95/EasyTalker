using EasyTalker.Core.Dto.User;

namespace EasyTalker.Core.Dto.Authentications;

public record AuthenticationResultDto(UserDto User, string AccessToken);