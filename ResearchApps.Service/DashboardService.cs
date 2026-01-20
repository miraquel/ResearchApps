using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class DashboardService : IDashboardService
{
    private readonly IDashboardRepo _dashboardRepo;
    private readonly ILogger<DashboardService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public DashboardService(IDashboardRepo dashboardRepo, ILogger<DashboardService> logger)
    {
        _dashboardRepo = dashboardRepo;
        _logger = logger;
    }

    public async Task<ServiceResponse<DashboardVm>> GetDashboardData(string userId, CancellationToken cancellationToken)
    {
        LogRetrievingDashboardData(userId);
        var dashboardVm = new DashboardVm();

        // Get statistics
        var statistics = await _dashboardRepo.GetStatistics(userId, cancellationToken);
        dashboardVm.Statistics = _mapper.Map(statistics);

        // Get recent PRs
        var recentPrs = await _dashboardRepo.GetRecentPrs(userId, 10, cancellationToken);
        dashboardVm.RecentPrs = recentPrs.Select(_mapper.Map).ToList();

        // Get pending approvals
        var pendingApprovals = await _dashboardRepo.GetPendingApprovals(userId, 10, cancellationToken);
        dashboardVm.PendingApprovals = pendingApprovals.Select(_mapper.Map).ToList();

        // Get top items (last 3 months)
        var topItems = await _dashboardRepo.GetTopItems(10, DateTime.Now.AddMonths(-3), DateTime.Now, cancellationToken);
        dashboardVm.TopItems = topItems.Select(_mapper.Map).ToList();

        // Get PR trend (last 6 months)
        var prTrend = await _dashboardRepo.GetPrTrend(6, cancellationToken);
        dashboardVm.PrTrend = prTrend.Select(_mapper.Map).ToList();

        // Get budget by department
        var budgetByDepartment = await _dashboardRepo.GetBudgetByDepartment(cancellationToken);
        dashboardVm.BudgetByDepartment = budgetByDepartment.Select(_mapper.Map).ToList();

        LogDashboardDataRetrieved(userId);
        return ServiceResponse<DashboardVm>.Success(dashboardVm, "Dashboard data retrieved successfully.");
    }

    [LoggerMessage(LogLevel.Debug, "Retrieving dashboard data for user: {userId}")]
    partial void LogRetrievingDashboardData(string userId);

    [LoggerMessage(LogLevel.Debug, "Dashboard data retrieved successfully for user: {userId}")]
    partial void LogDashboardDataRetrieved(string userId);
}
