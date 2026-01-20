using ResearchApps.Domain;

namespace ResearchApps.Repo.Interface;

public interface IDashboardRepo
{
    // Dashboard_GetStatistics
    Task<DashboardStatistics> GetStatistics(string userId, CancellationToken cancellationToken);
    
    // Dashboard_GetRecentPrs
    Task<List<RecentPr>> GetRecentPrs(string userId, int top, CancellationToken cancellationToken);
    
    // Dashboard_GetPendingApprovals
    Task<List<PendingApproval>> GetPendingApprovals(string userId, int top, CancellationToken cancellationToken);
    
    // Dashboard_GetTopItems
    Task<List<TopItem>> GetTopItems(int top, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken);
    
    // Dashboard_GetPrTrend
    Task<List<PrTrend>> GetPrTrend(int months, CancellationToken cancellationToken);
    
    // Dashboard_GetBudgetByDepartment
    Task<List<BudgetByDepartment>> GetBudgetByDepartment(CancellationToken cancellationToken);
}
