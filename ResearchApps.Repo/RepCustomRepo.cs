using System.Data;
using Dapper;
using ResearchApps.Domain;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class RepCustomRepo : IRepCustomRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public RepCustomRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<RepTools>> RepTools(int year, int month, CancellationToken ct)
    {
        const string query = "Rep_Tools";
        var parameters = new DynamicParameters();
        parameters.Add("@Year", year);
        parameters.Add("@Month", month);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<RepTools>(command);
    }

    public async Task<IEnumerable<RepToolsAnalysis>> RepToolsAnalysis(int year, int month, CancellationToken ct)
    {
        const string query = "Rep_ToolsAnalysis";
        var parameters = new DynamicParameters();
        parameters.Add("@Year", year);
        parameters.Add("@Month", month);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<RepToolsAnalysis>(command);
    }
}
