using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ResearchApps.Web.Hubs;

[Authorize]
public partial class WorkflowHub : Hub<IWorkflowNotificationClient>
{
    private readonly ILogger<WorkflowHub> _logger;

    public WorkflowHub(ILogger<WorkflowHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            LogUserConnected(userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            LogUserDisconnectedWithError(exception);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinEntityGroup(string entityType, string entityId)
    {
        if (string.IsNullOrEmpty(entityType) || string.IsNullOrEmpty(entityId)) return;
        if (!EntityTypes.IsValid(entityType)) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"{entityType}_{entityId}");
        LogJoinedGroup(Context.ConnectionId, entityType, entityId);
    }

    public async Task LeaveEntityGroup(string entityType, string entityId)
    {
        if (string.IsNullOrEmpty(entityType) || string.IsNullOrEmpty(entityId)) return;
        if (!EntityTypes.IsValid(entityType)) return;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{entityType}_{entityId}");
        LogLeftGroup(Context.ConnectionId, entityType, entityId);
    }

    [LoggerMessage(LogLevel.Debug, "User {UserId} connected to workflow hub with connection {ConnectionId}")]
    partial void LogUserConnected(string userId, string connectionId);

    [LoggerMessage(LogLevel.Warning, "User disconnected from workflow hub with error")]
    partial void LogUserDisconnectedWithError(Exception exception);

    [LoggerMessage(LogLevel.Debug, "Connection {ConnectionId} joined {EntityType} group: {EntityType}_{EntityId}")]
    partial void LogJoinedGroup(string connectionId, string entityType, string entityId);

    [LoggerMessage(LogLevel.Debug, "Connection {ConnectionId} left {EntityType} group: {EntityType}_{EntityId}")]
    partial void LogLeftGroup(string connectionId, string entityType, string entityId);
}
