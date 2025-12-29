using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class ReportRepo : IReportRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public ReportRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<Report>> CboAsync()
    {
        const string query = "ReportCbo";
        var command = new CommandDefinition(
            query,
            transaction: _dbTransaction,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryAsync<Report>(command);
        return result;
    }

    public async Task DeleteAsync(int reportId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "ReportDelete";
        var parameters = new DynamicParameters();
        parameters.Add("@ReportId", reportId);
        parameters.Add("@ModifiedBy", modifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.ExecuteAsync(command);
        
        if (result == 0)
        {
            throw new RepoException($"Report with ID {reportId} not found for deletion.");
        }
    }

    public async Task<Report> InsertAsync(Report report, CancellationToken cancellationToken)
    {
        const string query = "ReportInsert";
        var parameters = new DynamicParameters();
        parameters.Add("@ReportName", report.ReportName);
        parameters.Add("@Description", report.Description);
        parameters.Add("@ReportType", (int)report.ReportType);
        parameters.Add("@SqlQuery", report.SqlQuery);
        parameters.Add("@TemplatePath", report.TemplatePath);
        parameters.Add("@StatusId", report.StatusId);
        parameters.Add("@PageSize", report.PageSize);
        parameters.Add("@Orientation", (int)report.Orientation);
        parameters.Add("@CreatedBy", report.CreatedBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QuerySingleAsync<Report>(command);
        
        if (result == null)
        {
            throw new RepoException<Report>("Failed to insert Report", report);
        }
        
        return result;
    }

    public async Task<PagedList<Report>> SelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken)
    {
        const string query = "ReportSelect";
        var parameters = new DynamicParameters();
        
        foreach (var filter in listRequest.Filters)
        {
            if (filter.Value is { } strValue && strValue.Contains('%'))
            {
                parameters.Add($"@{filter.Key}", strValue);
            }
            else
            {
                parameters.Add($"@{filter.Key}", $"%{filter.Value}%");
            }
        }
        
        parameters.Add("@PageNumber", listRequest.PageNumber);
        parameters.Add("@PageSize", listRequest.PageSize);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<Report>().ToList();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<Report>(items, listRequest.PageNumber, listRequest.PageSize, totalCount);
    }

    public async Task<Report?> SelectByIdAsync(int reportId, CancellationToken cancellationToken)
    {
        const string query = "ReportSelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@ReportId", reportId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<Report>(command);
    }

    public async Task<Report> UpdateAsync(Report report, CancellationToken cancellationToken)
    {
        const string query = "ReportUpdate";
        var parameters = new DynamicParameters();
        parameters.Add("@ReportId", report.ReportId);
        parameters.Add("@ReportName", report.ReportName);
        parameters.Add("@Description", report.Description);
        parameters.Add("@ReportType", (int)report.ReportType);
        parameters.Add("@SqlQuery", report.SqlQuery);
        parameters.Add("@TemplatePath", report.TemplatePath);
        parameters.Add("@StatusId", report.StatusId);
        parameters.Add("@PageSize", report.PageSize);
        parameters.Add("@Orientation", (int)report.Orientation);
        parameters.Add("@ModifiedBy", report.ModifiedBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QuerySingleAsync<Report>(command);
        
        return result ?? throw new RepoException<Report>("Failed to update Report", report);
    }
}

