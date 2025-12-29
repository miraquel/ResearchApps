using Microsoft.AspNetCore.SignalR;
using ResearchApps.Service.Interface;
using ResearchApps.Web.Hubs;

namespace ResearchApps.Web.Services;

public interface IPrNotificationService
{
    Task NotifyPrSubmitted(string prId, int recId, string submittedBy, string? nextApprover);
    Task NotifyPrApproved(string prId, int recId, string approvedBy, string? nextApprover, bool isFullyApproved);
    Task NotifyPrRejected(string prId, int recId, string rejectedBy, string reason, string createdBy);
    Task NotifyPrRecalled(string prId, int recId, string recalledBy);
    Task NotifyUser(string userId, string message, string notificationType, object? data = null);
}

public class PrNotificationService : IPrNotificationService
{
    private readonly IHubContext<PrNotificationHub> _hubContext;
    private readonly ILogger<PrNotificationService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PrNotificationService(
        IHubContext<PrNotificationHub> hubContext,
        ILogger<PrNotificationService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _hubContext = hubContext;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task NotifyPrSubmitted(string prId, int recId, string submittedBy, string? nextApprover)
    {
        var notification = new
        {
            Type = "PrSubmitted",
            PrId = prId,
            RecId = recId,
            SubmittedBy = submittedBy,
            NextApprover = nextApprover,
            Message = $"PR {prId} has been submitted for approval by {submittedBy}",
            Timestamp = DateTime.UtcNow
        };

        // Store notification in database for next approver
        if (!string.IsNullOrEmpty(nextApprover))
        {
            await StoreNotificationAsync(
                nextApprover,
                "Pending Approval",
                $"PR {prId} requires your approval",
                "PendingApproval",
                prId,
                recId);
            
            // Send ONE notification only to the approver
            await _hubContext.Clients.Group($"user_{nextApprover}").SendAsync("ReceivePendingApproval", notification);
        }

        // Notify PR group (for anyone viewing the PR detail page) - this won't show toast, only updates UI
        await _hubContext.Clients.Group($"pr_{prId}").SendAsync("PrStatusChanged", notification);

        _logger.LogInformation("PR {PrId} submitted notification sent to approver {Approver}", prId, nextApprover);
    }

    public async Task NotifyPrApproved(string prId, int recId, string approvedBy, string? nextApprover, bool isFullyApproved)
    {
        var notification = new
        {
            Type = isFullyApproved ? "PrFullyApproved" : "PrApproved",
            PrId = prId,
            RecId = recId,
            ApprovedBy = approvedBy,
            NextApprover = nextApprover,
            IsFullyApproved = isFullyApproved,
            Message = isFullyApproved 
                ? $"PR {prId} has been fully approved" 
                : $"PR {prId} has been approved by {approvedBy}, pending next approval",
            Timestamp = DateTime.UtcNow
        };

        // Store notification in database for next approver (if not fully approved)
        if (!isFullyApproved && !string.IsNullOrEmpty(nextApprover))
        {
            await StoreNotificationAsync(
                nextApprover,
                "Pending Approval",
                $"PR {prId} requires your approval (approved by {approvedBy})",
                "PendingApproval",
                prId,
                recId);
            
            // Send ONE notification only to the next approver
            await _hubContext.Clients.Group($"user_{nextApprover}").SendAsync("ReceivePendingApproval", notification);
        }

        // Notify PR group (for anyone viewing the PR detail page) - this won't show toast, only updates UI
        await _hubContext.Clients.Group($"pr_{prId}").SendAsync("PrStatusChanged", notification);

        _logger.LogInformation("PR {PrId} approval notification sent. Fully approved: {IsFullyApproved}", prId, isFullyApproved);
    }

    public async Task NotifyPrRejected(string prId, int recId, string rejectedBy, string reason, string createdBy)
    {
        var notification = new
        {
            Type = "PrRejected",
            PrId = prId,
            RecId = recId,
            RejectedBy = rejectedBy,
            Reason = reason,
            Message = $"PR {prId} has been rejected by {rejectedBy}. Reason: {reason}",
            Timestamp = DateTime.UtcNow
        };

        // Store notification in database for creator
        if (!string.IsNullOrEmpty(createdBy))
        {
            await StoreNotificationAsync(
                createdBy,
                "PR Rejected",
                $"Your PR {prId} has been rejected by {rejectedBy}. Reason: {reason}",
                "PrRejected",
                prId,
                recId);
            
            // Send ONE notification only to the creator
            await _hubContext.Clients.Group($"user_{createdBy}").SendAsync("ReceivePrRejected", notification);
        }

        // Notify PR group (for anyone viewing the PR detail page) - this won't show toast, only updates UI
        await _hubContext.Clients.Group($"pr_{prId}").SendAsync("PrStatusChanged", notification);

        _logger.LogInformation("PR {PrId} rejection notification sent to creator {Creator}", prId, createdBy);
    }

    public async Task NotifyPrRecalled(string prId, int recId, string recalledBy)
    {
        var notification = new
        {
            Type = "PrRecalled",
            PrId = prId,
            RecId = recId,
            RecalledBy = recalledBy,
            Message = $"PR {prId} has been recalled by {recalledBy}",
            Timestamp = DateTime.UtcNow
        };

        // Notify PR group (for anyone viewing the PR detail page)
        await _hubContext.Clients.Group($"pr_{prId}").SendAsync("PrStatusChanged", notification);

        _logger.LogInformation("PR {PrId} recall notification sent", prId);
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

        _logger.LogInformation("Notification sent to user {UserId}: {Message}", userId, message);
    }

    private async Task StoreNotificationAsync(string userId, string title, string message, string notificationType, string prId, int recId)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            await notificationService.CreateWorkflowNotification(userId, title, message, notificationType, prId, recId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store notification in database for user {UserId}", userId);
        }
    }
}

