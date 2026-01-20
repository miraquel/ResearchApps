namespace ResearchApps.Service.Tests;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepo> _notificationRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly NotificationService _sut;

    public NotificationServiceTests()
    {
        _notificationRepoMock = new Mock<INotificationRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<NotificationService>>();
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
        // Arrange
        var userId = "user123";
        var title = "Test Notification";
        var message = "Test Message";
        var notificationType = "Info";
        var notificationId = 1;
        var cancellationToken = CancellationToken.None;

        _notificationRepoMock
            .Setup(x => x.NotificationInsert(It.IsAny<Notification>(), cancellationToken))
            .ReturnsAsync(notificationId);

        // Act
        var result = await _sut.CreateNotification(userId, title, message, notificationType, 
            null, null, null, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(notificationId, result.Data);
        Assert.Equal("Notification created successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CreateNotification_SetsIsReadToFalseByDefault()
    {
        // Arrange
        var userId = "user123";
        var title = "Test";
        var message = "Test Message";
        var notificationType = "Info";
        Notification? capturedNotification = null;

        _notificationRepoMock
            .Setup(x => x.NotificationInsert(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Callback<Notification, CancellationToken>((n, _) => capturedNotification = n)
            .ReturnsAsync(1);

        // Act
        await _sut.CreateNotification(userId, title, message, notificationType);

        // Assert
        Assert.NotNull(capturedNotification);
        Assert.False(capturedNotification.IsRead);
    }

    [Fact]
    public async Task CreateNotification_WithOptionalParameters_SetsAllFields()
    {
        // Arrange
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

        // Act
        await _sut.CreateNotification(userId, title, message, notificationType, url, refId, refRecId);

        // Assert
        Assert.NotNull(capturedNotification);
        Assert.Equal(url, capturedNotification.Url);
        Assert.Equal(refId, capturedNotification.RefId);
        Assert.Equal(refRecId, capturedNotification.RefRecId);
    }

    [Fact]
    public async Task GetNotifications_WithDefaultTake_ReturnsNotifications()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var notifications = new List<Notification>
        {
            new() { NotificationId = 1, UserId = _userClaimDto.Username, Title = "Notification 1" },
            new() { NotificationId = 2, UserId = _userClaimDto.Username, Title = "Notification 2" }
        };

        _notificationRepoMock
            .Setup(x => x.NotificationSelectByUserId(_userClaimDto.Username, 20, cancellationToken))
            .ReturnsAsync(notifications);

        // Act
        var result = await _sut.GetNotifications(20, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Notifications retrieved successfully.", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task GetUnreadNotifications_ReturnsOnlyUnreadNotifications()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var unreadNotifications = new List<Notification>
        {
            new() { NotificationId = 1, UserId = _userClaimDto.Username, Title = "Unread 1", IsRead = false }
        };

        _notificationRepoMock
            .Setup(x => x.NotificationSelectUnreadByUserId(_userClaimDto.Username, cancellationToken))
            .ReturnsAsync(unreadNotifications);

        // Act
        var result = await _sut.GetUnreadNotifications(cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Unread notifications retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task MarkAsRead_WithValidId_CommitsTransaction()
    {
        // Arrange
        var notificationId = 1;
        var cancellationToken = CancellationToken.None;

        _notificationRepoMock
            .Setup(x => x.NotificationMarkAsRead(notificationId, _userClaimDto.Username, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.MarkAsRead(notificationId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Notification marked as read.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task MarkAllAsRead_CommitsTransaction()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        _notificationRepoMock
            .Setup(x => x.NotificationMarkAllAsRead(_userClaimDto.Username, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.MarkAllAsRead(cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("All notifications marked as read.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task GetNotificationCount_ReturnsCorrectCounts()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var totalCount = 10;
        var unreadCount = 3;

        _notificationRepoMock
            .Setup(x => x.NotificationGetCount(_userClaimDto.Username, cancellationToken))
            .ReturnsAsync((totalCount, unreadCount));

        // Act
        var result = await _sut.GetNotificationCount(cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        var data = result.Data;
        Assert.NotNull(data);
        Assert.Equal(totalCount, data.TotalCount);
        Assert.Equal(unreadCount, data.UnreadCount);
    }

    [Fact]
    public async Task DeleteNotification_WithValidId_CommitsTransaction()
    {
        // Arrange
        var notificationId = 1;
        var cancellationToken = CancellationToken.None;

        _notificationRepoMock
            .Setup(x => x.NotificationDelete(notificationId, _userClaimDto.Username, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteNotification(notificationId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Notification deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CreateWorkflowNotification_CreatesNotificationWithCorrectUrl()
    {
        // Arrange
        var userId = "user123";
        var title = "PR Approval";
        var message = "Please approve PR001";
        var notificationType = "Workflow";
        var prId = "PR001";
        var prRecId = 100;
        var notificationId = 1;
        Notification? capturedNotification = null;

        _notificationRepoMock
            .Setup(x => x.NotificationInsert(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Callback<Notification, CancellationToken>((n, _) => capturedNotification = n)
            .ReturnsAsync(notificationId);

        // Act
        var result = await _sut.CreateWorkflowNotification(userId, title, message, notificationType, prId, prRecId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedNotification);
        Assert.Equal($"/Prs/Details/{prRecId}", capturedNotification.Url);
        Assert.Equal(prId, capturedNotification.RefId);
        Assert.Equal(prRecId, capturedNotification.RefRecId);
    }

    [Fact]
    public async Task CreateNotification_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        // Arrange
        var userId = "user123";
        var title = "Test";
        var message = "Test Message";
        var notificationType = "Info";

        _notificationRepoMock
            .Setup(x => x.NotificationInsert(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.CreateNotification(userId, title, message, notificationType));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task GetNotifications_WithEmptyResults_ReturnsEmptyCollection()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        _notificationRepoMock
            .Setup(x => x.NotificationSelectByUserId(_userClaimDto.Username, 20, cancellationToken))
            .ReturnsAsync(new List<Notification>());

        // Act
        var result = await _sut.GetNotifications(20, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }
}



