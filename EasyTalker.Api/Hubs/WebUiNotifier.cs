using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.Conversation;
using EasyTalker.Core.Dto.Message;
using EasyTalker.Core.Dto.User;
using Microsoft.AspNetCore.SignalR;

namespace EasyTalker.Api.Hubs;

public class WebUiNotifier : IWebUiNotifier
{
    private readonly IHubContext<WebUiHub> _hubContext;

    public WebUiNotifier(IHubContext<WebUiHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public void ConversationCreated(ConversationDto conversation) => _hubContext.Clients.All.SendAsync("ConversationCreated", conversation);
    
    public void ConversationUpdated(ConversationDto conversation) => _hubContext.Clients.All.SendAsync("ConversationUpdated", conversation);

    public void MessageCreated(MessageDto message) => _hubContext.Clients.All.SendAsync("MessageCreated", message);
    //public void MessageCreated(MessageDto message) => _hubContext.Clients.Group(message.ConversationId.ToString()).SendAsync("MessageCreated", message);

    public void UserRegistered(UserDto user) => _hubContext.Clients.All.SendAsync("UserRegistered", user);
}