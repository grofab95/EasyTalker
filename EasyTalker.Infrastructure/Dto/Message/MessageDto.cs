using EasyTalker.Infrastructure.Dto.Conversation;
using EasyTalker.Infrastructure.Dto.User;

namespace EasyTalker.Infrastructure.Dto.Message;

public class MessageDto
{
    public long Id { get; set; }
    public UserDto Sender { get; set; }
    public ConversationDto Conversation { get; set; }
    
    public string Text { get; set; }
    public string Status { get; set; }
}