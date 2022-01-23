using System;

namespace EasyTalker.Database.Views;

public class ConversationInfosView
{
    public long ConversationId { get; set; }
    public string Title { get; set; }
    public string CreatorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public long? LastMessageId { get; set; }
    public string ConversationParticipantsString { get; set; }
}