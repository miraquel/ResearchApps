using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface INotificationService
{
    // Create a new notification
    Task<ServiceResponse<int>> CreateNotification(string userId, string title, string message, string notificationType, 
        string? url = null, string? refId = null, int? refRecId = null, CancellationToken cancellationToken = default);
    
    // Get notifications for the current user
    Task<ServiceResponse<IEnumerable<NotificationVm>>> GetNotifications(int take = 20, CancellationToken cancellationToken = default);
    
    // Get unread notifications for the current user
    Task<ServiceResponse<IEnumerable<NotificationVm>>> GetUnreadNotifications(CancellationToken cancellationToken = default);
    
    // Mark a notification as read
    Task<ServiceResponse> MarkAsRead(int notificationId, CancellationToken cancellationToken = default);
    
    // Mark all notifications as read for the current user
    Task<ServiceResponse> MarkAllAsRead(CancellationToken cancellationToken = default);
    
    // Get notification counts for the current user
    Task<ServiceResponse<NotificationCountVm>> GetNotificationCount(CancellationToken cancellationToken = default);
    
    // Delete a notification
    Task<ServiceResponse> DeleteNotification(int notificationId, CancellationToken cancellationToken = default);
    
    // Create workflow notification (helper method)
    Task<ServiceResponse<int>> CreateWorkflowNotification(string userId, string title, string message, 
        string notificationType, string prId, int prRecId, CancellationToken cancellationToken = default);
}

