using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class WfFormRepo : IWfFormRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public WfFormRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<WfForm>> WfFormSelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken)
    {
        const string query = "WfForm_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in listRequest.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    case "WfFormId":
                    {
                        if (int.TryParse(strValue, out var intValue))
                            parameters.Add($"@{filter.Key}", intValue);
                        break;
                    }
                    default:
                        parameters.Add($"@{filter.Key}", $"%{filter.Value}%");
                        break;
                }
            }
        }

        parameters.Add("@PageNumber", listRequest.PageNumber);
        parameters.Add("@PageSize", listRequest.PageSize);
        parameters.Add("@SortOrder", listRequest.IsSortAscending ? "ASC" : "DESC");
        parameters.Add("@SortColumn", string.IsNullOrEmpty(listRequest.SortBy) ? "WfFormId" : listRequest.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<WfForm>().ToList();
        var totalCount = result.ReadSingle<int>();

        return new PagedList<WfForm>(items, listRequest.PageNumber, listRequest.PageSize, totalCount);
    }

    public async Task<WfForm> WfFormSelectByIdAsync(int wfFormId, CancellationToken cancellationToken)
    {
        const string query = "WfForm_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@WfFormId", wfFormId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<WfForm>(command)
               ?? throw new RepoException($"WfForm with ID {wfFormId} not found.");
    }

    public async Task<WfForm> WfFormInsertAsync(WfForm wfForm, CancellationToken cancellationToken)
    {
        const string query = "WfForm_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@FormName", wfForm.FormName);
        parameters.Add("@Initial", wfForm.Initial);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<WfForm>(command)
               ?? throw new RepoException<WfForm>("Failed to insert WfForm", wfForm);
    }

    public async Task<WfForm> WfFormUpdateAsync(WfForm wfForm, CancellationToken cancellationToken)
    {
        const string query = "WfForm_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@WfFormId", wfForm.WfFormId);
        parameters.Add("@FormName", wfForm.FormName);
        parameters.Add("@Initial", wfForm.Initial);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<WfForm>(command)
               ?? throw new RepoException<WfForm>("Failed to update WfForm", wfForm);
    }

    public async Task WfFormDeleteAsync(int wfFormId, CancellationToken cancellationToken)
    {
        const string query = "WfForm_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@WfFormId", wfFormId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.ExecuteAsync(command);
        if (result == 0)
            throw new RepoException($"WfForm with ID {wfFormId} not found.");
    }

    public async Task<IEnumerable<WfForm>> WfFormCboAsync(CboRequest cboRequest, CancellationToken cancellationToken)
    {
        const string query = "WfForm_Cbo";
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(cboRequest.Term))
        {
            parameters.Add("@Term", cboRequest.Term.Contains('%')
                ? cboRequest.Term
                : $"%{cboRequest.Term}%");
        }

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<WfForm>(command);
    }
}
