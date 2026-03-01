using System.Data;
using Dapper;
using ResearchApps.Domain;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class WfTransRepo : IWfTransRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public WfTransRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<WfTransHistory>> WfTransSelectByRefIdAsync(string refId, int wfFormId, CancellationToken cancellationToken)
    {
        const string query = "WfTrans_SelectByRefId";
        var parameters = new DynamicParameters();
        parameters.Add("@RefId", refId);
        parameters.Add("@WfFormId", wfFormId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<WfTransHistory>(command);
    }
}
