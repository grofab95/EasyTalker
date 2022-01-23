namespace EasyTalker.Core.Dto.Message;

public class MessageCreateDto
{
    public string SenderId { get; set; }
    public long ConversationId { get; set; }
    public string Text { get; set; }
    public string Status { get; set; }
}