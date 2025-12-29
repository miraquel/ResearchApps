using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class ReportParameterRepo : IReportParameterRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public ReportParameterRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task DeleteAsync(int parameterId, CancellationToken cancellationToken)
    {
        const string query = "ReportParameterDelete";
        var parameters = new DynamicParameters();
        parameters.Add("@ParameterId", parameterId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task DeleteByReportIdAsync(int reportId, CancellationToken cancellationToken)
    {
        const string query = "ReportParameterDeleteByReportId";
        var parameters = new DynamicParameters();
        parameters.Add("@ReportId", reportId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task<ReportParameter> InsertAsync(ReportParameter parameter, CancellationToken cancellationToken)
    {
        const string query = "ReportParameterInsert";
        var parameters = new DynamicParameters();
        parameters.Add("@ReportId", parameter.ReportId);
        parameters.Add("@ParameterName", parameter.ParameterName);
        parameters.Add("@DisplayLabel", parameter.DisplayLabel);
        parameters.Add("@DataType", (int)parameter.DataType);
        parameters.Add("@DefaultValue", parameter.DefaultValue);
        parameters.Add("@IsRequired", parameter.IsRequired);
        parameters.Add("@DisplayOrder", parameter.DisplayOrder);
        parameters.Add("@LookupSource", parameter.LookupSource);
        parameters.Add("@Placeholder", parameter.Placeholder);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QuerySingleAsync<ReportParameter>(command);
        
        if (result == null)
        {
            throw new RepoException<ReportParameter>("Failed to insert ReportParameter", parameter);
        }
        
        return result;
    }

    public async Task<ReportParameter?> SelectByIdAsync(int parameterId, CancellationToken cancellationToken)
    {
        const string query = "ReportParameterSelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@ParameterId", parameterId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<ReportParameter>(command);
    }

    public async Task<IEnumerable<ReportParameter>> SelectByReportIdAsync(int reportId, CancellationToken cancellationToken)
    {
        const string query = "ReportParameterSelectByReportId";
        var parameters = new DynamicParameters();
        parameters.Add("@ReportId", reportId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<ReportParameter>(command);
    }

    public async Task<ReportParameter> UpdateAsync(ReportParameter parameter, CancellationToken cancellationToken)
    {
        const string query = "ReportParameterUpdate";
        var parameters = new DynamicParameters();
        parameters.Add("@ParameterId", parameter.ParameterId);
        parameters.Add("@ReportId", parameter.ReportId);
        parameters.Add("@ParameterName", parameter.ParameterName);
        parameters.Add("@DisplayLabel", parameter.DisplayLabel);
        parameters.Add("@DataType", (int)parameter.DataType);
        parameters.Add("@DefaultValue", parameter.DefaultValue);
        parameters.Add("@IsRequired", parameter.IsRequired);
        parameters.Add("@DisplayOrder", parameter.DisplayOrder);
        parameters.Add("@LookupSource", parameter.LookupSource);
        parameters.Add("@Placeholder", parameter.Placeholder);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QuerySingleAsync<ReportParameter>(command);
        
        return result ?? throw new RepoException<ReportParameter>("Failed to update ReportParameter", parameter);
    }
}

