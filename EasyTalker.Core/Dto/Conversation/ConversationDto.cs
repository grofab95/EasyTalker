using System;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Core.Enums;

namespace EasyTalker.Core.Dto.Conversation;

public class ConversationDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string CreatorId { get; set; }
    public ConversationStatus Status { get; set; }
    public DateTime LastSeenAt { get; set; }
    public MessageDto LastMessage { get; set; }
    public ConversationParticipantDto[] Participants { get; set; }
}