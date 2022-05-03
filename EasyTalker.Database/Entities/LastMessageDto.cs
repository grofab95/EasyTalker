using System;
using EasyTalker.Core.Enums;

namespace EasyTalker.Database.Entities;

public class LastMessageDto
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public long ConversationId { get; set; }
    
    public string Text { get; set; }
    public MessageStatus Status { get; set; }
}