namespace EasyTalker.Core.Dto.Conversation;

public class ConversationDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string CreatorId { get; set; }
    public ConversationParticipantDto[] Participants { get; set; }
}