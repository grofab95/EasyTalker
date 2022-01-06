namespace EasyTalker.Core.Events;

public record UserConnectionStatusChanged(string UserId, bool IsOnline);