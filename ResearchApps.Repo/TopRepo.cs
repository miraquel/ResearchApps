using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class TopRepo : ITopRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public TopRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<Top>> SelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken)
    {
        const string query = "Top_Select";
        var parameters = new DynamicParameters();

        foreach (var filter in listRequest.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    case "StatusId":
                    {
                        if (int.TryParse(strValue, out var statusId))
                            parameters.Add($"@{filter.Key}", statusId);
                        break;
                    }
                    default:
                        parameters.Add($"@{filter.Key}", strValue);
                        break;
                }
            }
        }

        parameters.Add("@PageNumber", listRequest.PageNumber);
        parameters.Add("@PageSize", listRequest.PageSize);
        parameters.Add("@SortOrder", listRequest.IsSortAscending ? "ASC" : "DESC");
        parameters.Add("@SortColumn", string.IsNullOrEmpty(listRequest.SortBy) ? "TopId" : listRequest.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<Top>().ToList();
        var totalCount = result.ReadSingle<int>();

        return new PagedList<Top>(items, listRequest.PageNumber, listRequest.PageSize, totalCount);
    }

    public async Task<Top> SelectByIdAsync(int topId, CancellationToken cancellationToken)
    {
        const string query = "Top_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@TopId", topId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Top>(command)
               ?? throw new RepoException($"Top with ID {topId} not found.");
    }

    public async Task<Top> InsertAsync(Top top, CancellationToken cancellationToken)
    {
        const string query = "Top_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@TopName", top.TopName);
        parameters.Add("@TopDay", top.TopDay);
        parameters.Add("@StatusId", top.StatusId);
        parameters.Add("@CreatedBy", top.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Top>(command)
               ?? throw new RepoException<Top>("Failed to insert Top", top);
    }

    public async Task<Top> UpdateAsync(Top top, CancellationToken cancellationToken)
    {
        const string query = "Top_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@TopId", top.TopId);
        parameters.Add("@TopName", top.TopName);
        parameters.Add("@TopDay", top.TopDay);
        parameters.Add("@StatusId", top.StatusId);
        parameters.Add("@ModifiedBy", top.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Top>(command)
               ?? throw new RepoException<Top>("Failed to update Top", top);
    }

    public async Task DeleteAsync(int topId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Top_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@TopId", topId);
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

    public async Task<IEnumerable<Top>> CboAsync(CboRequest cboRequest, CancellationToken cancellationToken)
    {
        const string query = "Top_Cbo";
        var parameters = new DynamicParameters();

        if (cboRequest.Id > 0)
        {
            parameters.Add("@Id", cboRequest.Id);
        }

        if (!string.IsNullOrEmpty(cboRequest.Term))
        {
            parameters.Add("@Term", cboRequest.Term.Contains('%') ? cboRequest.Term : $"%{cboRequest.Term}%");
        }

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<Top>(command);
    }
}
