# Real-time Notifications (SignalR)

## Overview

SignalR provides real-time notifications for workflow events (PR/CO submissions, approvals, rejections).

## Hub Configuration

```csharp
// Program.cs
builder.Services.AddSignalR();
builder.Services.AddScoped<IPrNotificationService, PrNotificationService>();
builder.Services.AddScoped<ICoNotificationService, CoNotificationService>();

// Map hubs
app.MapHub<PrNotificationHub>("/hubs/pr-notifications");
app.MapHub<CoNotificationHub>("/hubs/co-notifications");
```

## Hub Structure

```csharp
// ResearchApps.Web/Hubs/PrNotificationHub.cs
[Authorize]
public class PrNotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            // Add user to personal group for targeted notifications
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        await base.OnConnectedAsync();
    }

    public async Task JoinPrGroup(string prId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"pr_{prId}");
    }
}
```

## Notification Service

```csharp
// ResearchApps.Web/Services/PrNotificationService.cs
public class PrNotificationService : IPrNotificationService
{
    private readonly IHubContext<PrNotificationHub> _hubContext;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public async Task NotifyPrSubmitted(string prId, int recId, string submittedBy, string? nextApprover)
    {
        var notification = new
        {
            Type = "PrSubmitted",
            PrId = prId,
            Message = $"PR {prId} submitted by {submittedBy}",
            Timestamp = DateTime.UtcNow
        };

        // Store in database for persistence
        if (!string.IsNullOrEmpty(nextApprover))
        {
            await StoreNotificationAsync(nextApprover, "Pending Approval", 
                $"PR {prId} requires your approval", "PendingApproval", prId, recId);
            
            // Send real-time notification to approver only
            await _hubContext.Clients.Group($"user_{nextApprover}")
                .SendAsync("ReceivePendingApproval", notification);
        }

        // Notify PR detail page viewers
        await _hubContext.Clients.Group($"pr_{prId}")
            .SendAsync("PrStatusChanged", notification);
    }
}
```

## Client-Side Connection

```javascript
// In view
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/pr-notifications")
    .withAutomaticReconnect()
    .build();

connection.on("ReceivePendingApproval", (notification) => {
    showToast(notification.Message);
    refreshNotificationBadge();
});

connection.on("PrStatusChanged", (notification) => {
    refreshPrDetails();
});

await connection.start();
await connection.invoke("JoinPrGroup", prId);
```

## Notification Events

| Event | Recipient | Trigger |
|-------|-----------|---------|
| `ReceivePendingApproval` | Next approver | PR/CO submitted or partially approved |
| `PrStatusChanged` | PR detail viewers | Any status change |
| `ReceiveApprovalNotification` | Creator | PR/CO approved |
| `ReceiveRejectionNotification` | Creator | PR/CO rejected |

## Persisted Notifications

Notifications are stored in database via `INotificationService`:

```csharp
private async Task StoreNotificationAsync(
    string userId, 
    string title, 
    string message, 
    string type, 
    string referenceId, 
    int recId)
{
    using var scope = _serviceScopeFactory.CreateScope();
    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
    
    await notificationService.NotificationInsert(new NotificationVm
    {
        UserId = userId,
        Title = title,
        Message = message,
        NotificationType = type,
        ReferenceId = referenceId,
        ReferenceRecId = recId,
        IsRead = false
    }, CancellationToken.None);
}
```

## Notification API Endpoints

```csharp
// Controllers/Api/NotificationsController.cs
[HttpGet]
public async Task<IActionResult> GetNotifications();  // All for current user

[HttpGet("unread")]
public async Task<IActionResult> GetUnread();  // Unread only

[HttpPost("{id}/read")]
public async Task<IActionResult> MarkAsRead(int id);

[HttpPost("read-all")]
public async Task<IActionResult> MarkAllAsRead();
```
