using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ResearchApps.Web.Hubs;

/// <summary>
/// SignalR Hub for Customer Order real-time notifications
/// Handles connections, group membership, and message broadcasting for CO workflow events
/// </summary>
[Authorize]
public class CoNotificationHub : Hub
{
    private readonly ILogger<CoNotificationHub> _logger;

    public CoNotificationHub(ILogger<CoNotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects
    /// Automatically joins the user's personal notification group
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogDebug("User {UserId} connected to CO notification hub with connection {ConnectionId}", 
                userId, Context.ConnectionId);
        }
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects
    /// Removes the user from their personal notification group
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogDebug("User {UserId} disconnected from CO notification hub", userId);
        }

        if (exception != null)
        {
            _logger.LogWarning(exception, "User disconnected with error");
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Client-callable method to join a specific Customer Order's notification group
    /// Used when viewing a CO detail page to receive real-time status updates
    /// </summary>
    /// <param name="coId">Customer Order ID to subscribe to</param>
    public async Task JoinCoGroup(string coId)
    {
        if (string.IsNullOrEmpty(coId)) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"co_{coId}");
        _logger.LogDebug("Connection {ConnectionId} joined CO group: co_{CoId}", 
            Context.ConnectionId, coId);
    }

    /// <summary>
    /// Client-callable method to leave a specific Customer Order's notification group
    /// Should be called when navigating away from a CO detail page
    /// </summary>
    /// <param name="coId">Customer Order ID to unsubscribe from</param>
    public async Task LeaveCoGroup(string coId)
    {
        if (string.IsNullOrEmpty(coId)) return;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"co_{coId}");
        _logger.LogDebug("Connection {ConnectionId} left CO group: co_{CoId}", 
            Context.ConnectionId, coId);
    }
}
