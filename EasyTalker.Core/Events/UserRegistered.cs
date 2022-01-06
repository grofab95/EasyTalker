using EasyTalker.Core.Dto.User;

namespace EasyTalker.Core.Events;

public record UserRegistered(UserDto User);