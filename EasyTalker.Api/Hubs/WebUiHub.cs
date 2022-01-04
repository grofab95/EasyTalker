using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace EasyTalker.Api.Hubs;

[Authorize]
public class WebUiHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Log.Information("Connected: ConnectionId={ConnectionId}", Context.ConnectionId);

        var userId = Context.UserIdentifier;
        if (userId != null)
            Groups.AddToGroupAsync(Context.ConnectionId, userId);

        //if (Context.User?.IsInRole(Roles))
        //Groups.AddToGroupAsync(Context.ConnectionId, "Admin");

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        Log.Information("Disconnected: ConnectionId={ConnectionId}", Context.ConnectionId);

        var userId = Context.UserIdentifier;
        if (userId != null)
            Groups.AddToGroupAsync(Context.ConnectionId, userId);

        return base.OnDisconnectedAsync(exception);
    }
}