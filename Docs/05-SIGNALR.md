# Real-time Notifications (SignalR)

## Overview

SignalR provides real-time notifications for workflow events across all entities (PR, CO, PO, etc.) via a **single unified hub**.

## Hub Configuration

```csharp
// Program.cs
builder.Services.AddSignalR();
builder.Services.AddSingleton<IWorkflowNotificationService, WorkflowNotificationService>();

// Single hub for all workflow entities
app.MapHub<WorkflowHub>("/hubs/workflow", options =>
{
    options.AllowStatefulReconnects = true;
});
```

## Hub Structure

```csharp
// ResearchApps.Web/Hubs/WorkflowHub.cs
[Authorize]
public partial class WorkflowHub : Hub<IWorkflowNotificationClient>
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        await base.OnConnectedAsync();
    }

    public async Task JoinEntityGroup(string entityType, string entityId)
    {
        if (!EntityTypes.IsValid(entityType)) return;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{entityType}_{entityId}");
    }

    public async Task LeaveEntityGroup(string entityType, string entityId) { ... }
}
```

## Entity Types & Registry

```csharp
// EntityTypes.cs — add new entity types here
public static class EntityTypes
{
    public const string Pr = "pr";
    public const string Co = "co";
    public const string Po = "po";
}

// WorkflowEntityRegistry.cs — maps entity types to display names and URL templates
[EntityTypes.Pr] = new("PR", "/Prs/Details/{0}"),
[EntityTypes.Co] = new("Customer Order", "/CustomerOrders/Details/{0}"),
[EntityTypes.Po] = new("Purchase Order", "/Pos/Details/{0}"),
```

## Strongly-Typed Client Interface

```csharp
// IWorkflowNotificationClient.cs
public interface IWorkflowNotificationClient
{
    Task ReceivePendingApproval(WorkflowNotification notification);
    Task ReceiveRejected(WorkflowNotification notification);
    Task ReceiveNotification(GenericNotification notification);
    Task EntityStatusChanged(WorkflowNotification notification);
}
```

## Notification Service

```csharp
// IWorkflowNotificationService.cs — one interface for all entities
public interface IWorkflowNotificationService
{
    Task NotifySubmitted(string entityType, string entityId, int recId, string submittedBy, string? nextApprover);
    Task NotifyApproved(string entityType, string entityId, int recId, string approvedBy, string? nextApprover, bool isFullyApproved);
    Task NotifyRejected(string entityType, string entityId, int recId, string rejectedBy, string reason, string createdBy);
    Task NotifyRecalled(string entityType, string entityId, int recId, string recalledBy);
    Task NotifyStatusChanged(string entityType, string entityId, int recId, string actionBy, string statusAction);
    Task NotifyUser(string userId, string message, string notificationType, object? data = null);
}

// Usage in controllers:
await _notificationService.NotifySubmitted(EntityTypes.Pr, prId, recId, user, nextApprover);
await _notificationService.NotifyStatusChanged(EntityTypes.Co, coId, recId, user, "Closed");
```

## Client-Side Connection

```javascript
// workflow-notifications.js (loaded globally via _vendor_scripts.cshtml)
// Auto-initializes on DOMContentLoaded, connects to /hubs/workflow

// In Detail views:
WorkflowNotificationManager.joinEntityGroup('pr', '@Model.Header.PrId');
WorkflowNotificationManager.joinEntityGroup('co', '@Model.Header.CoId');
WorkflowNotificationManager.joinEntityGroup('po', '@Model.Header.PoId');

// Listen for status changes:
document.addEventListener('workflow:statusChanged', function(e) {
    var notification = e.detail;
    if (notification.entityId === myEntityId) {
        location.reload();
    }
});
```

## Notification Events

| Event | Recipient | Trigger |
|-------|-----------|---------|
| `ReceivePendingApproval` | Next approver (user group) | Entity submitted or partially approved |
| `ReceiveRejected` | Creator (user group) | Entity rejected |
| `EntityStatusChanged` | Entity detail viewers (entity group) | Any status change |
| `ReceiveNotification` | Target user (user group) | General notifications |

## DOM Events (JavaScript)

| Event | Detail | When |
|-------|--------|------|
| `workflow:pendingApproval` | `WorkflowNotification` | Pending approval received |
| `workflow:rejected` | `WorkflowNotification` | Rejection received |
| `workflow:statusChanged` | `WorkflowNotification` | Any entity status change |
| `workflow:notification` | `GenericNotification` | General notification |

## Adding Notifications for New Entities

1. Add constant to `EntityTypes`: `public const string Bpb = "bpb";`
2. Add registry entry in `WorkflowEntityRegistry`: `[EntityTypes.Bpb] = new("BPB", "/Bpbs/Details/{0}")`
3. Inject `IWorkflowNotificationService` in controller, call `NotifySubmitted(EntityTypes.Bpb, ...)`
4. Add `WorkflowNotificationManager.joinEntityGroup('bpb', bpbId)` in Details view
5. **Zero new files needed.**

## Persisted Notifications

Notifications are stored in database via `INotificationService.CreateNotification()`:

```csharp
// WorkflowNotificationService calls CreateNotification directly with entity-specific URL
var url = WorkflowEntityRegistry.GetUrl(entityType, recId); // e.g., "/Prs/Details/123"
await notificationService.CreateNotification(userId, title, message, type, url, entityId, recId);
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
