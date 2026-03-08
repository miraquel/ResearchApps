using Microsoft.AspNetCore.SignalR;
using ResearchApps.Service.Interface;
using ResearchApps.Web.Hubs;

namespace ResearchApps.Web.Services;

public partial class WorkflowNotificationService : IWorkflowNotificationService
{
    private readonly IHubContext<WorkflowHub, IWorkflowNotificationClient> _hubContext;
    private readonly ILogger<WorkflowNotificationService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WorkflowNotificationService(
        IHubContext<WorkflowHub, IWorkflowNotificationClient> hubContext,
        ILogger<WorkflowNotificationService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _hubContext = hubContext;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task NotifySubmitted(string entityType, string entityId, int recId, string submittedBy, string? nextApprover)
    {
        LogSubmitted(entityType, entityId, submittedBy, nextApprover ?? "N/A");

        var displayName = WorkflowEntityRegistry.GetDisplayName(entityType);
        var notification = new WorkflowNotification
        {
            Type = $"{entityType}Submitted",
            EntityId = entityId,
            RecId = recId,
            ActionBy = submittedBy,
            NextApprover = nextApprover,
            Message = $"{displayName} {entityId} has been submitted for approval by {submittedBy}"
        };

        if (!string.IsNullOrEmpty(nextApprover))
        {
            await StoreNotificationAsync(
                entityType, nextApprover, "Pending Approval",
                $"{displayName} {entityId} requires your approval",
                "PendingApproval", entityId, recId);

            await UserGroup(nextApprover).ReceivePendingApproval(notification);
        }

        await EntityGroup(entityType, entityId).EntityStatusChanged(notification);
    }

    public async Task NotifyApproved(string entityType, string entityId, int recId, string approvedBy, string? nextApprover, bool isFullyApproved)
    {
        LogApproved(entityType, entityId, approvedBy, nextApprover ?? "N/A", isFullyApproved);

        var displayName = WorkflowEntityRegistry.GetDisplayName(entityType);
        var notification = new WorkflowNotification
        {
            Type = isFullyApproved ? $"{entityType}FullyApproved" : $"{entityType}Approved",
            EntityId = entityId,
            RecId = recId,
            ActionBy = approvedBy,
            NextApprover = nextApprover,
            IsFullyApproved = isFullyApproved,
            Message = isFullyApproved
                ? $"{displayName} {entityId} has been fully approved"
                : $"{displayName} {entityId} has been approved by {approvedBy}, pending next approval"
        };

        if (!isFullyApproved && !string.IsNullOrEmpty(nextApprover))
        {
            await StoreNotificationAsync(
                entityType, nextApprover, "Pending Approval",
                $"{displayName} {entityId} requires your approval (approved by {approvedBy})",
                "PendingApproval", entityId, recId);

            await UserGroup(nextApprover).ReceivePendingApproval(notification);
        }

        await EntityGroup(entityType, entityId).EntityStatusChanged(notification);
    }

    public async Task NotifyRejected(string entityType, string entityId, int recId, string rejectedBy, string reason, string createdBy)
    {
        LogRejected(entityType, entityId, rejectedBy, createdBy);

        var displayName = WorkflowEntityRegistry.GetDisplayName(entityType);
        var notification = new WorkflowNotification
        {
            Type = $"{entityType}Rejected",
            EntityId = entityId,
            RecId = recId,
            ActionBy = rejectedBy,
            Reason = reason,
            Message = $"{displayName} {entityId} has been rejected by {rejectedBy}. Reason: {reason}"
        };

        if (!string.IsNullOrEmpty(createdBy))
        {
            await StoreNotificationAsync(
                entityType, createdBy, $"{displayName} Rejected",
                $"Your {displayName} {entityId} has been rejected by {rejectedBy}. Reason: {reason}",
                $"{entityType}Rejected", entityId, recId);

            await UserGroup(createdBy).ReceiveRejected(notification);
        }

        await EntityGroup(entityType, entityId).EntityStatusChanged(notification);
    }

    public async Task NotifyRecalled(string entityType, string entityId, int recId, string recalledBy)
    {
        LogRecalled(entityType, entityId, recalledBy);

        var displayName = WorkflowEntityRegistry.GetDisplayName(entityType);
        var notification = new WorkflowNotification
        {
            Type = $"{entityType}Recalled",
            EntityId = entityId,
            RecId = recId,
            ActionBy = recalledBy,
            Message = $"{displayName} {entityId} has been recalled by {recalledBy}"
        };

        await EntityGroup(entityType, entityId).EntityStatusChanged(notification);
    }

    public async Task NotifyStatusChanged(string entityType, string entityId, int recId, string actionBy, string statusAction)
    {
        LogStatusChanged(entityType, entityId, actionBy, statusAction);

        var displayName = WorkflowEntityRegistry.GetDisplayName(entityType);
        var notification = new WorkflowNotification
        {
            Type = $"{entityType}{statusAction}",
            EntityId = entityId,
            RecId = recId,
            ActionBy = actionBy,
            Message = $"{displayName} {entityId} has been {statusAction.ToLowerInvariant()} by {actionBy}"
        };

        await EntityGroup(entityType, entityId).EntityStatusChanged(notification);
    }

    public async Task NotifyUser(string userId, string message, string notificationType, object? data = null)
    {
        var notification = new GenericNotification
        {
            Type = notificationType,
            Message = message,
            Data = data
        };

        await UserGroup(userId).ReceiveNotification(notification);
    }

    private IWorkflowNotificationClient UserGroup(string userId) =>
        _hubContext.Clients.Group($"user_{userId}");

    private IWorkflowNotificationClient EntityGroup(string entityType, string entityId) =>
        _hubContext.Clients.Group($"{entityType}_{entityId}");

    private async Task StoreNotificationAsync(
        string entityType, string userId, string title, string message,
        string notificationType, string entityId, int recId)
    {
        try
        {
            var url = WorkflowEntityRegistry.GetUrl(entityType, recId);
            using var scope = _serviceScopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            await notificationService.CreateNotification(
                userId, title, message, notificationType, url, entityId, recId);
        }
        catch (Exception ex)
        {
            LogNotificationStorageError(ex, userId);
        }
    }

    [LoggerMessage(LogLevel.Information, "{EntityType} {EntityId} submitted by {SubmittedBy}, next approver: {NextApprover}")]
    partial void LogSubmitted(string entityType, string entityId, string submittedBy, string nextApprover);

    [LoggerMessage(LogLevel.Information, "{EntityType} {EntityId} approved by {ApprovedBy}, next approver: {NextApprover}, fully approved: {IsFullyApproved}")]
    partial void LogApproved(string entityType, string entityId, string approvedBy, string nextApprover, bool isFullyApproved);

    [LoggerMessage(LogLevel.Information, "{EntityType} {EntityId} rejected by {RejectedBy}, notifying creator: {CreatedBy}")]
    partial void LogRejected(string entityType, string entityId, string rejectedBy, string createdBy);

    [LoggerMessage(LogLevel.Information, "{EntityType} {EntityId} recalled by {RecalledBy}")]
    partial void LogRecalled(string entityType, string entityId, string recalledBy);

    [LoggerMessage(LogLevel.Information, "{EntityType} {EntityId} status changed to {StatusAction} by {ActionBy}")]
    partial void LogStatusChanged(string entityType, string entityId, string actionBy, string statusAction);

    [LoggerMessage(LogLevel.Error, "Failed to store notification in database for user {UserId}")]
    partial void LogNotificationStorageError(Exception ex, string userId);
}
