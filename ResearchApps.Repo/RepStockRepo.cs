using System.Data;
using Dapper;
using ResearchApps.Domain;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class RepStockRepo : IRepStockRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public RepStockRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<RepStockCardMonthly>> RepStockCardMonthly(int itemId, int year, int month, CancellationToken ct)
    {
        const string query = "Rep_StockCard_Monthly";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemId", itemId);
        parameters.Add("@Year", year);
        parameters.Add("@Month", month);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<RepStockCardMonthly>(command);
    }
}
