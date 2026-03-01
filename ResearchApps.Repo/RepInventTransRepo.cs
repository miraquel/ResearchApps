using System.Data;
using Dapper;
using ResearchApps.Domain;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class RepInventTransRepo : IRepInventTransRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public RepInventTransRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<RepInventTransByItem>> RepInventTransByItem(int itemId, DateTime? startDate, DateTime? endDate, CancellationToken ct)
    {
        const string query = "Rep_InventTrans_ByItem";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemId", itemId);
        parameters.Add("@StartDate", startDate);
        parameters.Add("@EndDate", endDate);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<RepInventTransByItem>(command);
    }
}
