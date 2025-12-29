using System.Data;
using Microsoft.Extensions.Logging;
using ResearchApps.Domain;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class NotificationService : INotificationService
{
    private readonly INotificationRepo _notificationRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(INotificationRepo notificationRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto, ILogger<NotificationService> logger)
    {
        _notificationRepo = notificationRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<int>> CreateNotification(string userId, string title, string message, 
        string notificationType, string? url = null, string? refId = null, int? refRecId = null, 
        CancellationToken cancellationToken = default)
    {
        LogCreatingNotificationForUserUseridTypeType(userId, notificationType);
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Url = url,
            NotificationType = notificationType,
            RefId = refId,
            RefRecId = refRecId,
            IsRead = false,
            CreatedDate = DateTime.Now
        };

        var notificationId = await _notificationRepo.NotificationInsert(notification, cancellationToken);
        _dbTransaction.Commit();
        LogNotificationNotificationIdCreatedForUserUserid(notificationId, userId);
        
        return ServiceResponse<int>.Success(notificationId, "Notification created successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<NotificationVm>>> GetNotifications(int take = 20, 
        CancellationToken cancellationToken = default)
    {
        LogGettingNotificationsForUserUsernameTakeTake(_userClaimDto.Username, take);
        var notifications = await _notificationRepo.NotificationSelectByUserId(_userClaimDto.Username, take, cancellationToken);
        var notificationVms = notifications.Select(MapToVm);
        return ServiceResponse<IEnumerable<NotificationVm>>.Success(notificationVms, "Notifications retrieved successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<NotificationVm>>> GetUnreadNotifications(
        CancellationToken cancellationToken = default)
    {
        LogGettingUnreadNotificationsForUserUsername(_userClaimDto.Username);
        var notifications = await _notificationRepo.NotificationSelectUnreadByUserId(_userClaimDto.Username, cancellationToken);
        var notificationVms = notifications.Select(MapToVm);
        return ServiceResponse<IEnumerable<NotificationVm>>.Success(notificationVms, "Unread notifications retrieved successfully.");
    }

    public async Task<ServiceResponse> MarkAsRead(int notificationId, CancellationToken cancellationToken = default)
    {
        await _notificationRepo.NotificationMarkAsRead(notificationId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse.Success("Notification marked as read.");
    }

    public async Task<ServiceResponse> MarkAllAsRead(CancellationToken cancellationToken = default)
    {
        await _notificationRepo.NotificationMarkAllAsRead(_userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse.Success("All notifications marked as read.");
    }

    public async Task<ServiceResponse<NotificationCountVm>> GetNotificationCount(CancellationToken cancellationToken = default)
    {
        var (totalCount, unreadCount) = await _notificationRepo.NotificationGetCount(_userClaimDto.Username, cancellationToken);
        var countVm = new NotificationCountVm
        {
            TotalCount = totalCount,
            UnreadCount = unreadCount
        };
        return ServiceResponse<NotificationCountVm>.Success(countVm, "Notification count retrieved successfully.");
    }

    public async Task<ServiceResponse> DeleteNotification(int notificationId, CancellationToken cancellationToken = default)
    {
        await _notificationRepo.NotificationDelete(notificationId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse.Success("Notification deleted successfully.");
    }

    public async Task<ServiceResponse<int>> CreateWorkflowNotification(string userId, string title, string message, 
        string notificationType, string prId, int prRecId, CancellationToken cancellationToken = default)
    {
        var url = $"/Prs/Details/{prRecId}";
        return await CreateNotification(userId, title, message, notificationType, url, prId, prRecId, cancellationToken);
    }

    private static NotificationVm MapToVm(Notification notification)
    {
        return new NotificationVm
        {
            NotificationId = notification.NotificationId,
            UserId = notification.UserId,
            Title = notification.Title,
            Message = notification.Message,
            Url = notification.Url,
            NotificationType = notification.NotificationType,
            RefId = notification.RefId,
            RefRecId = notification.RefRecId,
            IsRead = notification.IsRead,
            CreatedDate = notification.CreatedDate,
            ReadDate = notification.ReadDate
        };
    }

    [LoggerMessage(LogLevel.Debug, "Creating notification for user: {userId}, type: {type}")]
    partial void LogCreatingNotificationForUserUseridTypeType(string userId, string type);

    [LoggerMessage(LogLevel.Information, "Notification {notificationId} created for user: {userId}")]
    partial void LogNotificationNotificationIdCreatedForUserUserid(int notificationId, string userId);

    [LoggerMessage(LogLevel.Debug, "Getting notifications for user: {username}, take: {take}")]
    partial void LogGettingNotificationsForUserUsernameTakeTake(string username, int take);

    [LoggerMessage(LogLevel.Debug, "Getting unread notifications for user: {username}")]
    partial void LogGettingUnreadNotificationsForUserUsername(string username);
}

