using System.Data;
using Dapper;
using ResearchApps.Domain;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class InventLockRepo : IInventLockRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public InventLockRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<InventLock>> SelectByYearAsync(int year, CancellationToken cancellationToken)
    {
        const string query = "InventLock_Select";
        var parameters = new DynamicParameters();
        parameters.Add("@Year", year);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<InventLock>(command);
    }

    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        const string query = "InventLock_Close";

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            transaction: _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task OpenAsync(int recId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "InventLock_Open";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);
        parameters.Add("@ModifiedBy", modifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task RunClosingAsync(int year, int month, string createdBy, CancellationToken cancellationToken)
    {
        const string query = "Invent_Closing";
        var parameters = new DynamicParameters();
        parameters.Add("@Year", year);
        parameters.Add("@Month", month);
        parameters.Add("@CreatedBy", createdBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandTimeout: 300, // 5 minutes timeout for closing process
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }
}
