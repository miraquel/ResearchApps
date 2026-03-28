namespace ResearchApps.Service.Tests;

public class DashboardServiceTests
{
    private readonly Mock<IDashboardRepo> _dashboardRepoMock;
    private readonly DashboardService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public DashboardServiceTests()
    {
        _dashboardRepoMock = new Mock<IDashboardRepo>();
        var loggerMock = new Mock<ILogger<DashboardService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        _sut = new DashboardService(_dashboardRepoMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task GetDashboardData_WithValidUserId_ReturnsCompleteDashboardData()
    {
        const string userId = "user123";

        var moduleCounts = new DashboardModuleCounts();

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

        var salesTrend = new List<SalesTrend>
        {
            new() { MonthYear = "January 2026", MonthDate = new DateTime(2026, 1, 1), CoCount = 3, CoTotal = 5000 }
        };

        var budgetByDepartment = new List<BudgetByDepartment>
        {
            new() { Department = "IT", TotalSpent = 50000, PrCount = 10, AvgAmount = 5000 }
        };

        var lowStockItems = new List<LowStockItem>
        {
            new() { ItemName = "Item Low", BufferStock = 100, CurrentStock = 10, Deficit = 90 }
        };

        _dashboardRepoMock.Setup(x => x.GetModuleCounts(userId, _ct))
            .ReturnsAsync(moduleCounts);
        _dashboardRepoMock.Setup(x => x.GetRecentPrs(userId, 5, _ct))
            .ReturnsAsync(recentPrs);
        _dashboardRepoMock.Setup(x => x.GetPendingApprovals(userId, 5, _ct))
            .ReturnsAsync(pendingApprovals);
        _dashboardRepoMock.Setup(x => x.GetTopItems(5, It.IsAny<DateTime>(), It.IsAny<DateTime>(), _ct))
            .ReturnsAsync(topItems);
        _dashboardRepoMock.Setup(x => x.GetPrTrend(6, _ct))
            .ReturnsAsync(prTrend);
        _dashboardRepoMock.Setup(x => x.GetSalesTrend(6, _ct))
            .ReturnsAsync(salesTrend);
        _dashboardRepoMock.Setup(x => x.GetBudgetByDepartment(_ct))
            .ReturnsAsync(budgetByDepartment);
        _dashboardRepoMock.Setup(x => x.GetLowStockItems(10, _ct))
            .ReturnsAsync(lowStockItems);

        var result = await _sut.GetDashboardData(userId, _ct);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data.ModuleCounts);
        Assert.NotNull(result.Data.RecentPrs);
        Assert.NotNull(result.Data.PendingApprovals);
        Assert.NotNull(result.Data.TopItems);
        Assert.NotNull(result.Data.PrTrend);
        Assert.NotNull(result.Data.SalesTrend);
        Assert.NotNull(result.Data.BudgetByDepartment);
        Assert.NotNull(result.Data.LowStockItems);
        Assert.Equal(2, result.Data.RecentPrs.Count);
        Assert.Single(result.Data.PendingApprovals);
    }

    [Fact]
    public async Task GetDashboardData_CallsAllRepositoryMethods()
    {
        var userId = "user123";

        _dashboardRepoMock.Setup(x => x.GetModuleCounts(userId, _ct))
            .ReturnsAsync(new DashboardModuleCounts());
        _dashboardRepoMock.Setup(x => x.GetRecentPrs(userId, 5, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPendingApprovals(userId, 5, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetTopItems(5, It.IsAny<DateTime>(), It.IsAny<DateTime>(), _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPrTrend(6, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetSalesTrend(6, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetBudgetByDepartment(_ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetLowStockItems(10, _ct))
            .ReturnsAsync([]);

        await _sut.GetDashboardData(userId, _ct);

        _dashboardRepoMock.Verify(x => x.GetModuleCounts(userId, _ct), Times.Once);
        _dashboardRepoMock.Verify(x => x.GetRecentPrs(userId, 5, _ct), Times.Once);
        _dashboardRepoMock.Verify(x => x.GetPendingApprovals(userId, 5, _ct), Times.Once);
        _dashboardRepoMock.Verify(x => x.GetTopItems(5, It.IsAny<DateTime>(), It.IsAny<DateTime>(), _ct), Times.Once);
        _dashboardRepoMock.Verify(x => x.GetPrTrend(6, _ct), Times.Once);
        _dashboardRepoMock.Verify(x => x.GetSalesTrend(6, _ct), Times.Once);
        _dashboardRepoMock.Verify(x => x.GetBudgetByDepartment(_ct), Times.Once);
        _dashboardRepoMock.Verify(x => x.GetLowStockItems(10, _ct), Times.Once);
    }

    [Fact]
    public async Task GetDashboardData_WithEmptyResults_ReturnsEmptyCollections()
    {
        var userId = "user123";

        _dashboardRepoMock.Setup(x => x.GetModuleCounts(userId, _ct))
            .ReturnsAsync(new DashboardModuleCounts());
        _dashboardRepoMock.Setup(x => x.GetRecentPrs(userId, 5, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPendingApprovals(userId, 5, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetTopItems(5, It.IsAny<DateTime>(), It.IsAny<DateTime>(), _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPrTrend(6, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetSalesTrend(6, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetBudgetByDepartment(_ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetLowStockItems(10, _ct))
            .ReturnsAsync([]);

        var result = await _sut.GetDashboardData(userId, _ct);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.RecentPrs);
        Assert.Empty(result.Data.PendingApprovals);
        Assert.Empty(result.Data.TopItems);
        Assert.Empty(result.Data.PrTrend);
        Assert.Empty(result.Data.SalesTrend);
        Assert.Empty(result.Data.BudgetByDepartment);
        Assert.Empty(result.Data.LowStockItems);
    }

    [Fact]
    public async Task GetDashboardData_WhenGetModuleCountsThrowsException_PropagatesException()
    {
        var userId = "user123";

        _dashboardRepoMock.Setup(x => x.GetModuleCounts(userId, _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.GetDashboardData(userId, _ct));
    }

    [Fact]
    public async Task GetDashboardData_UsesCorrectDateRangeForTopItems()
    {
        var userId = "user123";
        DateTime? capturedStartDate = null;
        DateTime? capturedEndDate = null;

        _dashboardRepoMock.Setup(x => x.GetModuleCounts(userId, _ct))
            .ReturnsAsync(new DashboardModuleCounts());
        _dashboardRepoMock.Setup(x => x.GetRecentPrs(userId, 5, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPendingApprovals(userId, 5, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock
            .Setup(x => x.GetTopItems(5, It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), _ct))
            .Callback<int, DateTime?, DateTime?, CancellationToken>((_, start, end, _) =>
            {
                capturedStartDate = start;
                capturedEndDate = end;
            })
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPrTrend(6, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetSalesTrend(6, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetBudgetByDepartment(_ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetLowStockItems(10, _ct))
            .ReturnsAsync([]);

        await _sut.GetDashboardData(userId, _ct);

        Assert.NotNull(capturedStartDate);
        Assert.NotNull(capturedEndDate);
        // Verify date range is approximately 3 months (allow some tolerance)
        var diff = capturedEndDate.Value - capturedStartDate.Value;
        Assert.True(diff.TotalDays is >= 85 and <= 95);
    }

    [Fact]
    public async Task GetDashboardData_WithNullUserId_StillCallsRepo()
    {
        string? userId = null;

        _dashboardRepoMock.Setup(x => x.GetModuleCounts(It.IsAny<string>(), _ct))
            .ReturnsAsync(new DashboardModuleCounts());
        _dashboardRepoMock.Setup(x => x.GetRecentPrs(It.IsAny<string>(), 5, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPendingApprovals(It.IsAny<string>(), 5, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetTopItems(5, It.IsAny<DateTime>(), It.IsAny<DateTime>(), _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetPrTrend(6, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetSalesTrend(6, _ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetBudgetByDepartment(_ct))
            .ReturnsAsync([]);
        _dashboardRepoMock.Setup(x => x.GetLowStockItems(10, _ct))
            .ReturnsAsync([]);

        var result = await _sut.GetDashboardData(userId!, _ct);

        Assert.NotNull(result);
    }
}
