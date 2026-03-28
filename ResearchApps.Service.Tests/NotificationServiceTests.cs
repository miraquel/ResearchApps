namespace ResearchApps.Service.Tests;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepo> _notificationRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly NotificationService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public NotificationServiceTests()
    {
        _notificationRepoMock = new Mock<INotificationRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<NotificationService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _userClaimDto = new UserClaimDto { Username = "testuser" };

        _sut = new NotificationService(
            _notificationRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task CreateNotification_WithValidData_ReturnsSuccessWithNotificationId()
    {
        var userId = "user123";
        var title = "Test Notification";
        var message = "Test Message";
        var notificationType = "Info";
        var notificationId = 1;

        _notificationRepoMock
            .Setup(x => x.NotificationInsert(It.IsAny<Notification>(), _ct))
            .ReturnsAsync(notificationId);

        var result = await _sut.CreateNotification(userId, title, message, notificationType,
            null, null, null, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal(notificationId, result.Data);
        Assert.Equal("Notification created successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CreateNotification_SetsIsReadToFalseByDefault()
    {
        var userId = "user123";
        var title = "Test";
        var message = "Test Message";
        var notificationType = "Info";
        Notification? capturedNotification = null;

        _notificationRepoMock
            .Setup(x => x.NotificationInsert(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Callback<Notification, CancellationToken>((n, _) => capturedNotification = n)
            .ReturnsAsync(1);

        await _sut.CreateNotification(userId, title, message, notificationType);

        Assert.NotNull(capturedNotification);
        Assert.False(capturedNotification.IsRead);
    }

    [Fact]
    public async Task CreateNotification_WithOptionalParameters_SetsAllFields()
    {
        var userId = "user123";
        var title = "Test";
        var message = "Test Message";
        var notificationType = "Info";
        var url = "/test/url";
        var refId = "REF001";
        var refRecId = 100;
        Notification? capturedNotification = null;

        _notificationRepoMock
            .Setup(x => x.NotificationInsert(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Callback<Notification, CancellationToken>((n, _) => capturedNotification = n)
            .ReturnsAsync(1);

        await _sut.CreateNotification(userId, title, message, notificationType, url, refId, refRecId);

        Assert.NotNull(capturedNotification);
        Assert.Equal(url, capturedNotification.Url);
        Assert.Equal(refId, capturedNotification.RefId);
        Assert.Equal(refRecId, capturedNotification.RefRecId);
    }

    [Fact]
    public async Task GetNotifications_WithDefaultTake_ReturnsNotifications()
    {
        var notifications = new List<Notification>
        {
            new() { NotificationId = 1, UserId = _userClaimDto.Username, Title = "Notification 1" },
            new() { NotificationId = 2, UserId = _userClaimDto.Username, Title = "Notification 2" }
        };

        _notificationRepoMock
            .Setup(x => x.NotificationSelectByUserId(_userClaimDto.Username, 20, _ct))
            .ReturnsAsync(notifications);

        var result = await _sut.GetNotifications(20, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Notifications retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task GetUnreadNotifications_ReturnsOnlyUnreadNotifications()
    {
        var unreadNotifications = new List<Notification>
        {
            new() { NotificationId = 1, UserId = _userClaimDto.Username, Title = "Unread 1", IsRead = false }
        };

        _notificationRepoMock
            .Setup(x => x.NotificationSelectUnreadByUserId(_userClaimDto.Username, _ct))
            .ReturnsAsync(unreadNotifications);

        var result = await _sut.GetUnreadNotifications(_ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Unread notifications retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task MarkAsRead_WithValidId_CommitsTransaction()
    {
        var notificationId = 1;

        _notificationRepoMock
            .Setup(x => x.NotificationMarkAsRead(notificationId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.MarkAsRead(notificationId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Notification marked as read.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task MarkAllAsRead_CommitsTransaction()
    {
        _notificationRepoMock
            .Setup(x => x.NotificationMarkAllAsRead(_userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.MarkAllAsRead(_ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("All notifications marked as read.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task GetNotificationCount_ReturnsCorrectCounts()
    {
        var totalCount = 10;
        var unreadCount = 3;

        _notificationRepoMock
            .Setup(x => x.NotificationGetCount(_userClaimDto.Username, _ct))
            .ReturnsAsync((totalCount, unreadCount));

        var result = await _sut.GetNotificationCount(_ct);

        Assert.True(result.IsSuccess);
        var data = result.Data;
        Assert.NotNull(data);
        Assert.Equal(totalCount, data.TotalCount);
        Assert.Equal(unreadCount, data.UnreadCount);
    }

    [Fact]
    public async Task DeleteNotification_WithValidId_CommitsTransaction()
    {
        var notificationId = 1;

        _notificationRepoMock
            .Setup(x => x.NotificationDelete(notificationId, _userClaimDto.Username, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.DeleteNotification(notificationId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Notification deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CreateNotification_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var userId = "user123";
        var title = "Test";
        var message = "Test Message";
        var notificationType = "Info";

        _notificationRepoMock
            .Setup(x => x.NotificationInsert(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.CreateNotification(userId, title, message, notificationType));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task GetNotifications_WithEmptyResults_ReturnsEmptyCollection()
    {
        _notificationRepoMock
            .Setup(x => x.NotificationSelectByUserId(_userClaimDto.Username, 20, _ct))
            .ReturnsAsync(new List<Notification>());

        var result = await _sut.GetNotifications(20, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }
}
