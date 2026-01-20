namespace ResearchApps.Service.Tests;

public class DashboardServiceTests
{
    private readonly Mock<IDashboardRepo> _dashboardRepoMock;
    private readonly Mock<ILogger<DashboardService>> _loggerMock;
    private readonly DashboardService _sut;

    public DashboardServiceTests()
    {
        _dashboardRepoMock = new Mock<IDashboardRepo>();
        _loggerMock = new Mock<ILogger<DashboardService>>();
        _sut = new DashboardService(_dashboardRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetDashboardData_WithValidUserId_ReturnsCompleteDashboardData()
    {
        // Arrange
        const string userId = "user123";
        var cancellationToken = CancellationToken.None;

        var statistics = new DashboardStatistics();

        var recentPrs = new List<RecentPr>
        {
            new() { PrId = "PR001", PrName = "PR 1" },
            new() { PrId = "PR002", PrName = "PR 2" }
        };

        var pendingApprovals = new List<PendingApproval>
        {
            new() { PrId = "PR003", PrName = "PR 3" }
        };

        var topItems = new List<TopItem>
        {
            new() { ItemName = "Item 1" }
        };

        var prTrend = new List<PrTrend>
        {
            new() { MonthYear = "January 2026", MonthDate = new DateTime(2026, 1, 1), PrCount = 5, TotalAmount = 10000 }
        };

        var budgetByDepartment = new List<BudgetByDepartment>
        {
            new() { Department = "IT", TotalSpent = 50000, PrCount = 10, AvgAmount = 5000 }
        };

        _dashboardRepoMock.Setup(x => x.GetStatistics(userId, cancellationToken))
            .ReturnsAsync(statistics);
        _dashboardRepoMock.Setup(x => x.GetRecentPrs(userId, 10, cancellationToken))
            .ReturnsAsync(recentPrs);
        _dashboardRepoMock.Setup(x => x.GetPendingApprovals(userId, 10, cancellationToken))
            .ReturnsAsync(pendingApprovals);
        _dashboardRepoMock.Setup(x => x.GetTopItems(10, It.IsAny<DateTime>(), It.IsAny<DateTime>(), cancellationToken))
            .ReturnsAsync(topItems);
        _dashboardRepoMock.Setup(x => x.GetPrTrend(6, cancellationToken))
            .ReturnsAsync(prTrend);
        _dashboardRepoMock.Setup(x => x.GetBudgetByDepartment(cancellationToken))
            .ReturnsAsync(budgetByDepartment);

        // Act
        var result = await _sut.GetDashboardData(userId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.Statistics);
        Assert.NotNull(result.Data.RecentPrs);
        Assert.NotNull(result.Data.PendingApprovals);
        Assert.NotNull(result.Data.TopItems);
        Assert.NotNull(result.Data.PrTrend);
        Assert.NotNull(result.Data.BudgetByDepartment);
        Assert.Equal(2, result.Data.RecentPrs.Count);
        Assert.Single(result.Data.PendingApprovals);
    }

    [Fact]
    public async Task GetDashboardData_CallsAllRepositoryMethods()
    {
        // Arrange
        var userId = "user123";
        var cancellationToken = CancellationToken.None;

        _dashboardRepoMock.Setup(x => x.GetStatistics(userId, cancellationToken))
            .ReturnsAsync(new DashboardStatistics());
        _dashboardRepoMock.Setup(x => x.GetRecentPrs(userId, 10, cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPendingApprovals(userId, 10, cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetTopItems(10, It.IsAny<DateTime>(), It.IsAny<DateTime>(), cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPrTrend(6, cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetBudgetByDepartment(cancellationToken))
            .ReturnsAsync([]);

        // Act
        await _sut.GetDashboardData(userId, cancellationToken);

        // Assert
        _dashboardRepoMock.Verify(x => x.GetStatistics(userId, cancellationToken), Times.Once);
        _dashboardRepoMock.Verify(x => x.GetRecentPrs(userId, 10, cancellationToken), Times.Once);
        _dashboardRepoMock.Verify(x => x.GetPendingApprovals(userId, 10, cancellationToken), Times.Once);
        _dashboardRepoMock.Verify(x => x.GetTopItems(10, It.IsAny<DateTime>(), It.IsAny<DateTime>(), cancellationToken), Times.Once);
        _dashboardRepoMock.Verify(x => x.GetPrTrend(6, cancellationToken), Times.Once);
        _dashboardRepoMock.Verify(x => x.GetBudgetByDepartment(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetDashboardData_WithEmptyResults_ReturnsEmptyCollections()
    {
        // Arrange
        var userId = "user123";
        var cancellationToken = CancellationToken.None;

        _dashboardRepoMock.Setup(x => x.GetStatistics(userId, cancellationToken))
            .ReturnsAsync(new DashboardStatistics());
        _dashboardRepoMock.Setup(x => x.GetRecentPrs(userId, 10, cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPendingApprovals(userId, 10, cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetTopItems(10, It.IsAny<DateTime>(), It.IsAny<DateTime>(), cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPrTrend(6, cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetBudgetByDepartment(cancellationToken))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.GetDashboardData(userId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.RecentPrs);
        Assert.Empty(result.Data.PendingApprovals);
        Assert.Empty(result.Data.TopItems);
        Assert.Empty(result.Data.PrTrend);
        Assert.Empty(result.Data.BudgetByDepartment);
    }

    [Fact]
    public async Task GetDashboardData_WhenGetStatisticsThrowsException_PropagatesException()
    {
        // Arrange
        var userId = "user123";
        var cancellationToken = CancellationToken.None;

        _dashboardRepoMock.Setup(x => x.GetStatistics(userId, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.GetDashboardData(userId, cancellationToken));
    }

    [Fact]
    public async Task GetDashboardData_UsesCorrectDateRangeForTopItems()
    {
        // Arrange
        var userId = "user123";
        var cancellationToken = CancellationToken.None;
        DateTime? capturedStartDate = null;
        DateTime? capturedEndDate = null;

        _dashboardRepoMock.Setup(x => x.GetStatistics(userId, cancellationToken))
            .ReturnsAsync(new DashboardStatistics());
        _dashboardRepoMock.Setup(x => x.GetRecentPrs(userId, 10, cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPendingApprovals(userId, 10, cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock
            .Setup(x => x.GetTopItems(10, It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), cancellationToken))
            .Callback<int, DateTime?, DateTime?, CancellationToken>((_, start, end, _) =>
            {
                capturedStartDate = start;
                capturedEndDate = end;
            })
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPrTrend(6, cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetBudgetByDepartment(cancellationToken))
            .ReturnsAsync([]);

        // Act
        await _sut.GetDashboardData(userId, cancellationToken);

        // Assert
        Assert.NotNull(capturedStartDate);
        Assert.NotNull(capturedEndDate);
        // Verify date range is approximately 3 months (allow some tolerance)
        var diff = capturedEndDate.Value - capturedStartDate.Value;
        Assert.True(diff.TotalDays is >= 85 and <= 95);
    }

    [Fact]
    public async Task GetDashboardData_WithNullUserId_StillCallsRepo()
    {
        // Arrange
        string? userId = null;
        var cancellationToken = CancellationToken.None;

        _dashboardRepoMock.Setup(x => x.GetStatistics(It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(new DashboardStatistics());
        _dashboardRepoMock.Setup(x => x.GetRecentPrs(It.IsAny<string>(), 10, cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPendingApprovals(It.IsAny<string>(), 10, cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetTopItems(10, It.IsAny<DateTime>(), It.IsAny<DateTime>(), cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPrTrend(6, cancellationToken))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetBudgetByDepartment(cancellationToken))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.GetDashboardData(userId!, cancellationToken);

        // Assert
        Assert.NotNull(result);
    }
}






