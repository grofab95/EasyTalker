using EasyTalker.Core.Dto.Message;

namespace EasyTalker.Core.Dto.Conversation;

public class ConversationCreateDto
{
    public string CreatorId { get; set; }
    public string[] ParticipantsId { get; set; }
    public string Title { get; set; }
}