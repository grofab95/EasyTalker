using EasyTalker.Core.Enums;

namespace EasyTalker.Database.Entities;

public class MessageDb : EntityDb
{
    public string SenderId { get; set; }
    public long ConversationId { get; set; }
    
    public string Text { get; set; }
    public MessageStatus Status { get; set; }
}