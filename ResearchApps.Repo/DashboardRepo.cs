using System.Data;
using Dapper;
using ResearchApps.Domain;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class DashboardRepo : IDashboardRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public DashboardRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<DashboardStatistics> GetStatistics(string userId, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);

        var result = await _dbConnection.QueryFirstOrDefaultAsync<DashboardStatistics>(
            new CommandDefinition("DashboardGetStatistics", parameters,
                transaction: _dbTransaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken)
        );

        return result ?? new DashboardStatistics();
    }

    public async Task<List<RecentPr>> GetRecentPrs(string userId, int top, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);
        parameters.Add("@Top", top);

        var result = await _dbConnection.QueryAsync<RecentPr>(
            new CommandDefinition("DashboardGetRecentPrs", parameters,
                transaction: _dbTransaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken)
        );

        return result.ToList();
    }

    public async Task<List<PendingApproval>> GetPendingApprovals(string userId, int top, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);
        parameters.Add("@Top", top);

        var result = await _dbConnection.QueryAsync<PendingApproval>(
            new CommandDefinition("DashboardGetPendingApprovals", parameters,
                transaction: _dbTransaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken)
        );

        return result.ToList();
    }

    public async Task<List<TopItem>> GetTopItems(int top, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Top", top);
        parameters.Add("@StartDate", startDate);
        parameters.Add("@EndDate", endDate);

        var result = await _dbConnection.QueryAsync<TopItem>(
            new CommandDefinition("DashboardGetTopItems", parameters,
                transaction: _dbTransaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken)
        );

        return result.ToList();
    }

    public async Task<List<PrTrend>> GetPrTrend(int months, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Months", months);

        var result = await _dbConnection.QueryAsync<PrTrend>(
            new CommandDefinition("DashboardGetPrTrend", parameters,
                transaction: _dbTransaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken)
        );

        return result.ToList();
    }

    public async Task<DashboardModuleCounts> GetModuleCounts(string userId, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);

        var result = await _dbConnection.QueryFirstOrDefaultAsync<DashboardModuleCounts>(
            new CommandDefinition("DashboardGetModuleCounts", parameters,
                transaction: _dbTransaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken)
        );

        return result ?? new DashboardModuleCounts();
    }

    public async Task<List<SalesTrend>> GetSalesTrend(int months, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Months", months);

        var result = await _dbConnection.QueryAsync<SalesTrend>(
            new CommandDefinition("DashboardGetSalesTrend", parameters,
                transaction: _dbTransaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken)
        );

        return result.ToList();
    }

    public async Task<List<BudgetByDepartment>> GetBudgetByDepartment(CancellationToken cancellationToken)
    {
        var result = await _dbConnection.QueryAsync<BudgetByDepartment>(
            new CommandDefinition("DashboardGetBudgetByDepartment",
                transaction: _dbTransaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken)
        );

        return result.ToList();
    }

    public async Task<List<LowStockItem>> GetLowStockItems(int top, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Top", top);

        var result = await _dbConnection.QueryAsync<LowStockItem>(
            new CommandDefinition("DashboardGetLowStockItems", parameters,
                transaction: _dbTransaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken)
        );

        return result.ToList();
    }
}
