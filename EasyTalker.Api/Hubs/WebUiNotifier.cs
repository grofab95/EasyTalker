using EasyTalker.Core.Adapters;
using EasyTalker.Core.Dto.Message;
using Microsoft.AspNetCore.SignalR;

namespace EasyTalker.Api.Hubs;

public class WebUiNotifier : IWebUiNotifier
{
    private readonly IHubContext<WebUiHub> _hubContext;

    public WebUiNotifier(IHubContext<WebUiHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public void MessageChanged(MessageDto message) => _hubContext.Clients.All.SendAsync("MessageChanged", message);
}