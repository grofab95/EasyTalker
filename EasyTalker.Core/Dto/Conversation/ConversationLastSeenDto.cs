using System;

namespace EasyTalker.Core.Dto.Conversation;

public class ConversationLastSeenDto
{
    public long ConversationId { get; set; }
    public DateTime LastSeenAt { get; set; }
}