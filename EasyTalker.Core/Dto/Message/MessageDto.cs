using System;
using EasyTalker.Core.Dto.User;
using EasyTalker.Core.Enums;

namespace EasyTalker.Core.Dto.Message;

public class MessageDto
{
    public long Id { get; set; }
    public UserDto Sender { get; set; }
    
    public string Text { get; set; }
    public MessageStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}