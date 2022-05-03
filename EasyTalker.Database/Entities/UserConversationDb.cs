using System;
using EasyTalker.Core.Enums;

namespace EasyTalker.Database.Entities;

public class UserConversationDb : EntityDb
{
    public string UserId { get; set; }
    public long ConversationId { get; set; }
    public ConversationAccessStatus AccessStatus { get; set; }
    public DateTime LastSeenAt { get; set; }

    public UserConversationDb(string userId, long conversationId)
    {
        UserId = userId;
        ConversationId = conversationId;
        AccessStatus = ConversationAccessStatus.ReadAndWrite;
    }
}