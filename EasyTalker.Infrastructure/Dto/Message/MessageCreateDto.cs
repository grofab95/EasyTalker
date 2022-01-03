namespace EasyTalker.Infrastructure.Dto.Message;

public class MessageCreateDto
{
    public long Id { get; set; }
    public string SenderId { get; set; }
    public long ConversationId { get; set; }
    
    public string Text { get; set; }
    public string Status { get; set; }
}