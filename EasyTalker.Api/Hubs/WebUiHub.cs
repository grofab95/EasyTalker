using System;
using System.Threading.Tasks;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace EasyTalker.Api.Hubs;

[Authorize]
public class WebUiHub : Hub
{
    private readonly IUserStore _userStore;

    public WebUiHub(IUserStore userStore)
    {
        _userStore = userStore;
    }
    
    public override Task OnConnectedAsync()
    {
        Log.Information("Connected: ConnectionId={ConnectionId}", Context.ConnectionId);

        var userId = Context.UserIdentifier;
        if (userId != null)
            Groups.AddToGroupAsync(Context.ConnectionId, userId);

        _userStore.UpdateUserConnectionStatus(userId, true);
        
        Clients.Others.SendAsync("UserConnectionStatusChanged", new UserConnectionStatusChanged(userId, true));
        
        return base.OnConnectedAsync();
    }
    
    public override Task OnDisconnectedAsync(Exception exception)
    {
        Log.Information("Disconnected: ConnectionId={ConnectionId}", Context.ConnectionId);

        var userId = Context.UserIdentifier;
        if (userId != null)
            Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);

        _userStore.UpdateUserConnectionStatus(userId, false);
        
        Clients.Others.SendAsync("UserConnectionStatusChanged", new UserConnectionStatusChanged(userId, false));
        
        return base.OnDisconnectedAsync(exception);
    }
}