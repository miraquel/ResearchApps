using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ResearchApps.Web.Hubs;

[Authorize]
public class PrNotificationHub : Hub
{
    /// <summary>
    /// Called when a new client connects to the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            // Add user to their personal group for targeted notifications
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join a specific PR notification group
    /// </summary>
    public async Task JoinPrGroup(string prId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"pr_{prId}");
    }

    /// <summary>
    /// Leave a specific PR notification group
    /// </summary>
    public async Task LeavePrGroup(string prId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"pr_{prId}");
    }
}

