using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class WfRepo : IWfRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public WfRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<Wf>> WfSelectByWfFormIdAsync(int wfFormId, CancellationToken cancellationToken)
    {
        const string query = "Wf_SelectByWfFormId";
        var parameters = new DynamicParameters();
        parameters.Add("@WfFormId", wfFormId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<Wf>(command);
    }

    public async Task<Wf> WfSelectByIdAsync(int wfId, CancellationToken cancellationToken)
    {
        const string query = "Wf_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@WfId", wfId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Wf>(command)
               ?? throw new RepoException($"Wf with ID {wfId} not found.");
    }

    public async Task<Wf> WfInsertAsync(Wf wf, CancellationToken cancellationToken)
    {
        const string query = "Wf_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@WfFormId", wf.WfFormId);
        parameters.Add("@Index", wf.Index);
        parameters.Add("@UserId", wf.UserId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Wf>(command)
               ?? throw new RepoException<Wf>("Failed to insert Wf", wf);
    }

    public async Task<Wf> WfUpdateAsync(Wf wf, CancellationToken cancellationToken)
    {
        const string query = "Wf_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@WfId", wf.WfId);
        parameters.Add("@WfFormId", wf.WfFormId);
        parameters.Add("@Index", wf.Index);
        parameters.Add("@UserId", wf.UserId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Wf>(command)
               ?? throw new RepoException<Wf>("Failed to update Wf", wf);
    }

    public async Task WfDeleteAsync(int wfId, CancellationToken cancellationToken)
    {
        const string query = "Wf_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@WfId", wfId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.ExecuteAsync(command);
        if (result == 0)
            throw new RepoException($"Wf with ID {wfId} not found.");
    }
}
