using ResearchApps.Domain;

namespace ResearchApps.Repo.Interface;

public interface INotificationRepo
{
    // Notification_Insert - Insert a new notification
    Task<int> NotificationInsert(Notification notification, CancellationToken cancellationToken);
    
    // Notification_SelectByUserId - Get notifications for a user
    Task<IEnumerable<Notification>> NotificationSelectByUserId(string userId, int take, CancellationToken cancellationToken);
    
    // Notification_SelectUnreadByUserId - Get unread notifications for a user
    Task<IEnumerable<Notification>> NotificationSelectUnreadByUserId(string userId, CancellationToken cancellationToken);
    
    // Notification_MarkAsRead - Mark a notification as read
    Task NotificationMarkAsRead(int notificationId, string userId, CancellationToken cancellationToken);
    
    // Notification_MarkAllAsRead - Mark all notifications as read for a user
    Task NotificationMarkAllAsRead(string userId, CancellationToken cancellationToken);
    
    // Notification_GetCount - Get notification counts for a user
    Task<(int TotalCount, int UnreadCount)> NotificationGetCount(string userId, CancellationToken cancellationToken);
    
    // Notification_Delete - Delete a notification
    Task NotificationDelete(int notificationId, string userId, CancellationToken cancellationToken);
}

