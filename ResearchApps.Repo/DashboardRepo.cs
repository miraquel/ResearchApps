using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ResearchApps.Domain;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class DashboardRepo : IDashboardRepo
{
    private readonly string _connectionString;

    public DashboardRepo(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<DashboardStatistics> GetStatistics(string userId, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);

        var result = await connection.QueryFirstOrDefaultAsync<DashboardStatistics>(
            "DashboardGetStatistics",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result ?? new DashboardStatistics();
    }

    public async Task<List<RecentPr>> GetRecentPrs(string userId, int top, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);
        parameters.Add("@Top", top);

        var result = await connection.QueryAsync<RecentPr>(
            "DashboardGetRecentPrs",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result.ToList();
    }

    public async Task<List<PendingApproval>> GetPendingApprovals(string userId, int top, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);
        parameters.Add("@Top", top);

        var result = await connection.QueryAsync<PendingApproval>(
            "DashboardGetPendingApprovals",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result.ToList();
    }

    public async Task<List<TopItem>> GetTopItems(int top, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@Top", top);
        parameters.Add("@StartDate", startDate);
        parameters.Add("@EndDate", endDate);

        var result = await connection.QueryAsync<TopItem>(
            "DashboardGetTopItems",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result.ToList();
    }

    public async Task<List<PrTrend>> GetPrTrend(int months, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@Months", months);

        var result = await connection.QueryAsync<PrTrend>(
            "DashboardGetPrTrend",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result.ToList();
    }

    public async Task<List<BudgetByDepartment>> GetBudgetByDepartment(CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);

        var result = await connection.QueryAsync<BudgetByDepartment>(
            "DashboardGetBudgetByDepartment",
            commandType: CommandType.StoredProcedure
        );

        return result.ToList();
    }
}
