using Microsoft.AspNetCore.SignalR;
using ResearchApps.Service.Interface;
using ResearchApps.Web.Hubs;

namespace ResearchApps.Web.Services;

/// <summary>
/// Customer Order notification service implementation
/// Sends real-time SignalR notifications and persists to database
/// </summary>
public partial class CoNotificationService : ICoNotificationService
{
    private readonly IHubContext<CoNotificationHub> _hubContext;
    private readonly ILogger<CoNotificationService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CoNotificationService(
        IHubContext<CoNotificationHub> hubContext,
        ILogger<CoNotificationService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _hubContext = hubContext;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task NotifyCoSubmitted(string coId, int recId, string submittedBy, string? nextApprover)
    {
        LogCoSubmitted(coId, submittedBy, nextApprover ?? "N/A");

        var notification = new
        {
            Type = "CoSubmitted",
            CoId = coId,
            RecId = recId,
            SubmittedBy = submittedBy,
            NextApprover = nextApprover,
            Message = $"Customer Order {coId} has been submitted for approval by {submittedBy}",
            Timestamp = DateTime.UtcNow
        };

        // Notify CO group for UI updates
        await _hubContext.Clients.Group($"co_{coId}").SendAsync("CoStatusChanged", notification);

        // Notify next approver if assigned
        if (!string.IsNullOrEmpty(nextApprover))
        {
            var approverNotification = new
            {
                Type = "PendingApproval",
                CoId = coId,
                RecId = recId,
                Message = $"Customer Order {coId} is pending your approval (submitted by {submittedBy})",
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"user_{nextApprover}").SendAsync("ReceivePendingApproval", approverNotification);

            // Store notification in database
            await StoreNotificationAsync(
                nextApprover,
                "Pending Approval",
                $"Customer Order {coId} is pending your approval",
                "PendingApproval",
                coId,
                recId);
        }
    }

    public async Task NotifyCoApproved(string coId, int recId, string approvedBy, string? nextApprover, bool isFullyApproved)
    {
        LogCoApproved(coId, approvedBy, nextApprover ?? "N/A", isFullyApproved);

        var notification = new
        {
            Type = isFullyApproved ? "CoFullyApproved" : "CoApproved",
            CoId = coId,
            RecId = recId,
            ApprovedBy = approvedBy,
            NextApprover = nextApprover,
            IsFullyApproved = isFullyApproved,
            Message = isFullyApproved
                ? $"Customer Order {coId} has been fully approved"
                : $"Customer Order {coId} has been approved by {approvedBy}",
            Timestamp = DateTime.UtcNow
        };

        // Notify CO group for UI updates
        await _hubContext.Clients.Group($"co_{coId}").SendAsync("CoStatusChanged", notification);

        // If not fully approved, notify next approver
        if (!isFullyApproved && !string.IsNullOrEmpty(nextApprover))
        {
            var approverNotification = new
            {
                Type = "PendingApproval",
                CoId = coId,
                RecId = recId,
                Message = $"Customer Order {coId} is pending your approval (approved by {approvedBy})",
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"user_{nextApprover}").SendAsync("ReceivePendingApproval", approverNotification);

            await StoreNotificationAsync(
                nextApprover,
                "Pending Approval",
                $"Customer Order {coId} is pending your approval",
                "PendingApproval",
                coId,
                recId);
        }
    }

    public async Task NotifyCoRejected(string coId, int recId, string rejectedBy, string reason, string createdBy)
    {
        LogCoRejected(coId, rejectedBy, createdBy);

        var notification = new
        {
            Type = "CoRejected",
            CoId = coId,
            RecId = recId,
            RejectedBy = rejectedBy,
            Reason = reason,
            Message = $"Customer Order {coId} has been rejected by {rejectedBy}. Reason: {reason}",
            Timestamp = DateTime.UtcNow
        };

        // Notify CO group for UI updates
        await _hubContext.Clients.Group($"co_{coId}").SendAsync("CoStatusChanged", notification);

        // Notify creator
        if (!string.IsNullOrEmpty(createdBy))
        {
            await _hubContext.Clients.Group($"user_{createdBy}").SendAsync("ReceiveCoRejected", notification);

            await StoreNotificationAsync(
                createdBy,
                "Order Rejected",
                $"Customer Order {coId} has been rejected by {rejectedBy}",
                "CoRejected",
                coId,
                recId);
        }
    }

    public async Task NotifyCoRecalled(string coId, int recId, string recalledBy)
    {
        LogCoRecalled(coId, recalledBy);

        var notification = new
        {
            Type = "CoRecalled",
            CoId = coId,
            RecId = recId,
            RecalledBy = recalledBy,
            Message = $"Customer Order {coId} has been recalled by {recalledBy}",
            Timestamp = DateTime.UtcNow
        };

        // Notify CO group for UI updates
        await _hubContext.Clients.Group($"co_{coId}").SendAsync("CoStatusChanged", notification);
    }

    public async Task NotifyCoClosed(string coId, int recId, string closedBy)
    {
        LogCoClosed(coId, closedBy);

        var notification = new
        {
            Type = "CoClosed",
            CoId = coId,
            RecId = recId,
            ClosedBy = closedBy,
            Message = $"Customer Order {coId} has been closed by {closedBy}",
            Timestamp = DateTime.UtcNow
        };

        // Notify CO group for UI updates
        await _hubContext.Clients.Group($"co_{coId}").SendAsync("CoStatusChanged", notification);
    }

    public async Task NotifyUser(string userId, string message, string notificationType, object? data = null)
    {
        var notification = new
        {
            Type = notificationType,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        };

        await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", notification);
    }

    /// <summary>
    /// Store notification in database for persistence
    /// Uses IServiceScopeFactory to create a new scope (and new transaction)
    /// </summary>
    private async Task StoreNotificationAsync(string userId, string title, string message, 
        string notificationType, string coId, int recId)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            
            await notificationService.CreateWorkflowNotification(
                userId, 
                title, 
                message, 
                notificationType, 
                coId, 
                recId);
        }
        catch (Exception ex)
        {
            LogNotificationStorageError(ex, userId);
        }
    }

    // LoggerMessage source-generated methods for high-performance logging
    [LoggerMessage(LogLevel.Information, "CO {CoId} submitted by {SubmittedBy}, next approver: {NextApprover}")]
    partial void LogCoSubmitted(string coId, string submittedBy, string nextApprover);

    [LoggerMessage(LogLevel.Information, "CO {CoId} approved by {ApprovedBy}, next approver: {NextApprover}, fully approved: {IsFullyApproved}")]
    partial void LogCoApproved(string coId, string approvedBy, string nextApprover, bool isFullyApproved);

    [LoggerMessage(LogLevel.Information, "CO {CoId} rejected by {RejectedBy}, notifying creator: {CreatedBy}")]
    partial void LogCoRejected(string coId, string rejectedBy, string createdBy);

    [LoggerMessage(LogLevel.Information, "CO {CoId} recalled by {RecalledBy}")]
    partial void LogCoRecalled(string coId, string recalledBy);

    [LoggerMessage(LogLevel.Information, "CO {CoId} closed by {ClosedBy}")]
    partial void LogCoClosed(string coId, string closedBy);

    [LoggerMessage(LogLevel.Error, "Failed to store notification in database for user {UserId}")]
    partial void LogNotificationStorageError(Exception ex, string userId);
}
