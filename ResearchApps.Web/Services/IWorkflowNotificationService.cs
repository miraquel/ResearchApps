namespace ResearchApps.Web.Services;

public interface IWorkflowNotificationService
{
    Task NotifySubmitted(string entityType, string entityId, int recId, string submittedBy, string? nextApprover);
    Task NotifyApproved(string entityType, string entityId, int recId, string approvedBy, string? nextApprover, bool isFullyApproved);
    Task NotifyRejected(string entityType, string entityId, int recId, string rejectedBy, string reason, string createdBy);
    Task NotifyRecalled(string entityType, string entityId, int recId, string recalledBy);
    Task NotifyStatusChanged(string entityType, string entityId, int recId, string actionBy, string statusAction);
    Task NotifyUser(string userId, string message, string notificationType, object? data = null);
}
