using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class PsRepo : IPsRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public PsRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<PsHeader>> PsSelect(CancellationToken cancellationToken)
    {
        const string query = "Ps_Select";

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            transaction: _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<PsHeader>(command);
    }

    public async Task<PagedList<PsHeader>> PsSelect(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Ps_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    // Special handling for date fields
                    case "PsDate" or "CreatedDate" or "ModifiedDate" or "PsDateFrom" or "PsDateTo":
                    {
                        if (DateTime.TryParse(strValue, out var dateValue))
                        {
                            parameters.Add($"@{filter.Key}", dateValue);
                        }
                        break;
                    }
                    // Special handling for numeric fields
                    case "Amount":
                    {
                        if (decimal.TryParse(strValue, out var decimalValue))
                        {
                            parameters.Add($"@{filter.Key}", decimalValue);
                        }
                        break;
                    }
                    // Special handling for operator fields
                    case "AmountOperator":
                    {
                        parameters.Add($"@{filter.Key}", strValue);
                        break;
                    }
                    // Special handling for integer fields
                    case "PsStatusId" or "RecId":
                    {
                        if (int.TryParse(strValue, out var intValue))
                        {
                            parameters.Add($"@{filter.Key}", intValue);
                        }
                        break;
                    }
                    // Default: string fields
                    default:
                        parameters.Add($"@{filter.Key}", strValue);
                        break;
                }
            }
        }
        
        parameters.Add("@PageNumber", request.PageNumber);
        parameters.Add("@PageSize", request.PageSize);
        parameters.Add("@SortOrder", request.IsSortAscending ? "ASC" : "DESC");
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "PsId" : request.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<PsHeader>();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<PsHeader>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<PsHeader> PsSelectById(int id, CancellationToken cancellationToken)
    {
        const string query = "Ps_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", id);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstOrDefaultAsync<PsHeader>(command);
        
        return result ?? throw new RepoException($"Penyesuaian Stock with Id {id} not found.");
    }

    public async Task<(int RecId, string PsId)> PsInsert(PsHeader ps, CancellationToken cancellationToken)
    {
        const string query = "Ps_Insert";
        var parameters = new DynamicParameters();
        
        parameters.Add("@PsDate", ps.PsDate);
        parameters.Add("@Descr", ps.Descr ?? string.Empty);
        parameters.Add("@Notes", ps.Notes ?? string.Empty);
        parameters.Add("@PsStatusId", ps.PsStatusId);
        parameters.Add("@CreatedBy", ps.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstAsync<dynamic>(command);
        
        // Check for InventLock (returns RecId = -1)
        int recId = (int)result.RecId;
        if (recId == -1)
        {
            throw new RepoException("Inventory period is locked. Cannot create stock adjustment for this period.");
        }
        
        return (recId, (string)result.PsId);
    }

    public async Task PsUpdate(PsHeader ps, CancellationToken cancellationToken)
    {
        const string query = "Ps_Update";
        var parameters = new DynamicParameters();
        
        parameters.Add("@RecId", ps.RecId);
        parameters.Add("@Notes", ps.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", ps.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task<string> PsDelete(int recId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Ps_Delete";
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
        
        return await _dbConnection.QuerySingleAsync<string>(command);
    }

    public async Task<IEnumerable<PsLine>> PsLineSelectByPs(int psRecId, CancellationToken cancellationToken)
    {
        const string query = "PsLine_SelectByPs";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", psRecId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<PsLine>(command);
    }

    public async Task<PsLine?> PsLineSelectById(int psLineId, CancellationToken cancellationToken)
    {
        const string query = "PsLine_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@PsLineId", psLineId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<PsLine>(command);
    }

    public async Task<string> PsLineInsert(PsLine psLine, CancellationToken cancellationToken)
    {
        const string query = "PsLine_Insert";
        var parameters = new DynamicParameters();
        
        parameters.Add("@RecId", psLine.PsRecId);
        parameters.Add("@ItemId", psLine.ItemId);
        parameters.Add("@WhId", psLine.WhId);
        parameters.Add("@Qty", psLine.Qty);
        parameters.Add("@Notes", psLine.Notes ?? string.Empty);
        parameters.Add("@CreatedBy", psLine.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QuerySingleAsync<string>(command);
    }

    public async Task<string> PsLineUpdate(PsLine psLine, CancellationToken cancellationToken)
    {
        const string query = "PsLine_Update";
        var parameters = new DynamicParameters();
        
        parameters.Add("@PsLineId", psLine.PsLineId);
        parameters.Add("@ItemId", psLine.ItemId);
        parameters.Add("@WhId", psLine.WhId);
        parameters.Add("@Qty", psLine.Qty);
        parameters.Add("@Notes", psLine.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", psLine.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QuerySingleAsync<string>(command);
    }

    public async Task<string> PsLineDelete(int psLineId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "PsLine_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@PsLineId", psLineId);
        parameters.Add("@ModifiedBy", modifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QuerySingleAsync<string>(command);
    }
}
