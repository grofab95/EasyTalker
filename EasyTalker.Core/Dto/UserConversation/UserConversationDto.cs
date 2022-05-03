using System;

namespace EasyTalker.Core.Dto.UserConversation;

public class UserConversationDto
{
    public long Id { get; set; }
    public string UserId { get; set; }
    public long ConversationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string AccessStatus { get; set; }
    public DateTime LastSeenAt { get; set; }
}