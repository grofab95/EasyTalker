using System;
using System.Collections.Generic;
using EasyTalker.Infrastructure.Dto.Message;
using EasyTalker.Infrastructure.Dto.User;

namespace EasyTalker.Infrastructure.Dto.Conversation;

public class ConversationDto
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public string Title { get; set; }
    public IEnumerable<MessageDto> Messages { get; set; }
    public IEnumerable<UserDto> Participants { get; set; }
}