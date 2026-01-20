using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class PrRepo : IPrRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public PrRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task PrDelete(int id, CancellationToken cancellationToken)
    {
        const string query = "Pr_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", id);
        parameters.Add("@ModifiedBy", "system");

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
            throw new RepoException($"PR with Id {id} could not be deleted.");
        }
    }

    public async Task<int> PrInsert(Pr pr, CancellationToken cancellationToken)
    {
        const string query = "Pr_Insert";
        var parameters = new DynamicParameters();

        parameters.Add("@PrDate", pr.PrDate);
        parameters.Add("@PrName", pr.PrName);
        parameters.Add("@BudgetId", pr.BudgetId);
        parameters.Add("@RequestDate", pr.RequestDate);
        parameters.Add("@Notes", pr.Notes ?? string.Empty);
        parameters.Add("@PrStatusId", pr.PrStatusId);
        
        if (!string.IsNullOrEmpty(pr.CreatedBy))
        {
            parameters.Add("@CreatedBy", pr.CreatedBy);
        }

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QuerySingleAsync<int>(command);
        
        return result;
    }

    public async Task<PagedList<Pr>> PrSelect(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Pr_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    // Special handling for date fields
                    case "PrDate" or "PrDateFrom" or "PrDateTo" or "CreatedDate" or "CreatedDateFrom" or "CreatedDateTo" or "ModifiedDate" or "ModifiedDateFrom" or "ModifiedDateTo":
                    {
                        if (DateTime.TryParse(strValue, out var dateValue))
                        {
                            parameters.Add($"@{filter.Key}", dateValue);
                        }
                        break;
                    }
                    // Special handling for numeric fields
                    case "Total":
                    {
                        if (decimal.TryParse(strValue, out var decimalValue))
                        {
                            parameters.Add($"@{filter.Key}", decimalValue);
                        }
                        break;
                    }
                    // Special handling for operator fields
                    case "TotalOperator":
                    {
                        parameters.Add($"@{filter.Key}", strValue);
                        break;
                    }
                    // Special handling for integer fields
                    case "PrStatusId" or "BudgetId" or "RecId":
                    {
                        if (int.TryParse(strValue, out var intValue))
                        {
                            parameters.Add($"@{filter.Key}", intValue);
                        }
                        break;
                    }
                    // Special handling for PrId (it's a string but might be passed as an integer)
                    case "PrId":
                    {
                        // PrId is varchar in database, keep as string
                        parameters.Add($"@{filter.Key}", strValue);
                        break;
                    }
                    // Default: string fields with LIKE pattern
                    default:
                        parameters.Add($"@{filter.Key}", strValue);
                        break;
                }
            }
        }
        
        parameters.Add("@PageNumber", request.PageNumber);
        parameters.Add("@PageSize", request.PageSize);
        parameters.Add("@SortOrder", request.IsSortAscending ? "ASC" : "DESC");
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "PrId" : request.SortBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<Pr>();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<Pr>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<Pr> PrSelectById(int id, CancellationToken cancellationToken)
    {
        const string query = "Pr_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", id);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstOrDefaultAsync<Pr>(command);
        
        if (result == null)
        {
            throw new RepoException<Pr>($"PR with Id {id} not found.", null);
        }

        return result;
    }

    public async Task PrUpdate(Pr pr, CancellationToken cancellationToken)
    {
        const string query = "Pr_Update";
        var parameters = new DynamicParameters();
        
        parameters.Add("@RecId", pr.RecId);
        parameters.Add("@PrDate", pr.PrDate);
        parameters.Add("@PrName", pr.PrName);
        parameters.Add("@BudgetId", pr.BudgetId);
        parameters.Add("@RequestDate", pr.RequestDate);
        parameters.Add("@Notes", pr.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", pr.ModifiedBy);

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
            throw new RepoException($"PR with Id {pr.RecId} could not be updated.");
        }
    }

    public async Task<Pr> PrSubmitById(int id, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Pr_SubmitById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", id);
        parameters.Add("@ModifiedBy", modifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstOrDefaultAsync<Pr>(command);
        
        if (result == null)
        {
            throw new RepoException($"PR with Id {id} could not be submitted.");
        }

        return result;
    }

    public async Task PrApproveById(int id, string notes, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Pr_ApproveById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", id);
        parameters.Add("@Notes", notes);
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

    public async Task PrRejectById(int id, string notes, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Pr_RejectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", id);
        parameters.Add("@Notes", notes);
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

    public async Task PrRecallById(int id, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Pr_RecallById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", id);
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
}