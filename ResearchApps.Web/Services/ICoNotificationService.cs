namespace ResearchApps.Web.Services;

/// <summary>
/// Interface for Customer Order notification service
/// Handles real-time SignalR notifications and database persistence for CO workflow events
/// </summary>
public interface ICoNotificationService
{
    /// <summary>
    /// Notify when a Customer Order is submitted for approval
    /// </summary>
    /// <param name="coId">Customer Order ID (e.g., "CO-2025-0001")</param>
    /// <param name="recId">Database record ID</param>
    /// <param name="submittedBy">Username who submitted</param>
    /// <param name="nextApprover">Username of the next approver (nullable)</param>
    Task NotifyCoSubmitted(string coId, int recId, string submittedBy, string? nextApprover);

    /// <summary>
    /// Notify when a Customer Order is approved
    /// </summary>
    /// <param name="coId">Customer Order ID</param>
    /// <param name="recId">Database record ID</param>
    /// <param name="approvedBy">Username who approved</param>
    /// <param name="nextApprover">Username of next approver (null if fully approved)</param>
    /// <param name="isFullyApproved">True if all approval levels complete</param>
    Task NotifyCoApproved(string coId, int recId, string approvedBy, string? nextApprover, bool isFullyApproved);

    /// <summary>
    /// Notify when a Customer Order is rejected
    /// </summary>
    /// <param name="coId">Customer Order ID</param>
    /// <param name="recId">Database record ID</param>
    /// <param name="rejectedBy">Username who rejected</param>
    /// <param name="reason">Rejection reason/notes</param>
    /// <param name="createdBy">Username of CO creator to notify</param>
    Task NotifyCoRejected(string coId, int recId, string rejectedBy, string reason, string createdBy);

    /// <summary>
    /// Notify when a Customer Order is recalled by creator
    /// </summary>
    /// <param name="coId">Customer Order ID</param>
    /// <param name="recId">Database record ID</param>
    /// <param name="recalledBy">Username who recalled</param>
    Task NotifyCoRecalled(string coId, int recId, string recalledBy);

    /// <summary>
    /// Notify when a Customer Order is closed
    /// </summary>
    /// <param name="coId">Customer Order ID</param>
    /// <param name="recId">Database record ID</param>
    /// <param name="closedBy">Username who closed</param>
    Task NotifyCoClosed(string coId, int recId, string closedBy);

    /// <summary>
    /// Send a generic notification to a specific user
    /// </summary>
    /// <param name="userId">Username to notify</param>
    /// <param name="message">Notification message</param>
    /// <param name="notificationType">Type of notification</param>
    /// <param name="data">Additional data payload (optional)</param>
    Task NotifyUser(string userId, string message, string notificationType, object? data = null);
}
