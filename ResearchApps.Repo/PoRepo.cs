using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class PoRepo : IPoRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public PoRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<Po>> PoSelect(PagedListRequest request, CancellationToken ct)
    {
        const string query = "Po_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    // Special handling for date fields
                    case "PoDate" or "CreatedDate" or "ModifiedDate" or "PoDateFrom" or "PoDateTo":
                    {
                        if (DateTime.TryParse(strValue, out var dateValue))
                        {
                            parameters.Add($"@{filter.Key}", dateValue);
                        }
                        break;
                    }
                    // Special handling for numeric fields
                    case "Total" or "SubTotal" or "Ppn":
                    {
                        if (decimal.TryParse(strValue, out var decimalValue))
                        {
                            parameters.Add($"@{filter.Key}", decimalValue);
                        }
                        break;
                    }
                    // Special handling for operator fields
                    case "TotalOperator" or "SubTotalOperator" or "PpnOperator":
                    {
                        parameters.Add($"@{filter.Key}", strValue);
                        break;
                    }
                    // Special handling for integer fields
                    case "PoStatusId" or "SupplierId" or "RecId":
                    {
                        if (int.TryParse(strValue, out var intValue))
                        {
                            parameters.Add($"@{filter.Key}", intValue);
                        }
                        break;
                    }
                    // Special handling for boolean fields
                    case "IsPpn":
                    {
                        if (bool.TryParse(strValue, out var boolValue))
                        {
                            parameters.Add($"@{filter.Key}", boolValue);
                        }
                        break;
                    }
                    // Default: string fields (don't add wildcards, stored proc handles it)
                    default:
                        parameters.Add($"@{filter.Key}", strValue);
                        break;
                }
            }
        }

        parameters.Add("@PageNumber", request.PageNumber);
        parameters.Add("@PageSize", request.PageSize);
        parameters.Add("@SortOrder", request.IsSortAscending ? "ASC" : "DESC");
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "PoId" : request.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<Po>();
        var totalCount = result.ReadSingle<int>();

        return new PagedList<Po>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<Po?> PoSelectById(int recId, CancellationToken ct)
    {
        const string query = "Po_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Po>(command);
    }

    public async Task<Po> PoInsert(Po po, CancellationToken ct)
    {
        const string query = "Po_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@SupplierId", po.SupplierId);
        parameters.Add("@Pic", po.Pic);
        parameters.Add("@PoDate", po.PoDate);
        parameters.Add("@RefNo", po.RefNo);
        parameters.Add("@SubTotal", po.SubTotal);
        parameters.Add("@Ppn", po.Ppn);
        parameters.Add("@Total", po.Total);
        parameters.Add("@Notes", po.Notes);
        parameters.Add("@PoStatusId", po.PoStatusId);
        parameters.Add("@CreatedBy", po.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Po>(command)
            ?? throw new RepoException<Po>("Failed to insert purchase order", po);
    }

    public async Task<Po> PoUpdate(Po po, CancellationToken ct)
    {
        const string query = "Po_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", po.RecId);
        parameters.Add("@SupplierId", po.SupplierId);
        parameters.Add("@Pic", po.Pic);
        parameters.Add("@PoDate", po.PoDate);
        parameters.Add("@IsPpn", po.IsPpn);
        parameters.Add("@Notes", po.Notes);
        parameters.Add("@ModifiedBy", po.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Po>(command)
            ?? throw new RepoException<Po>("Failed to update purchase order", po);
    }

    public async Task PoDelete(int recId, CancellationToken ct)
    {
        const string query = "Po_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task<Po> PoSubmitById(int recId, string modifiedBy, CancellationToken ct)
    {
        const string query = "Po_SubmitById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);
        parameters.Add("@ModifiedBy", modifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Po>(command)
            ?? throw new RepoException<Po>("Failed to submit purchase order", new Po { RecId = recId });
    }

    public async Task PoRecallById(int recId, string modifiedBy, CancellationToken ct)
    {
        const string query = "Po_RecallById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);
        parameters.Add("@ModifiedBy", modifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task PoApproveById(int recId, string? notes, string modifiedBy, CancellationToken ct)
    {
        const string query = "Po_ApproveById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);
        parameters.Add("@Notes", notes);
        parameters.Add("@ModifiedBy", modifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task PoRejectById(int recId, string? notes, string modifiedBy, CancellationToken ct)
    {
        const string query = "Po_RejectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);
        parameters.Add("@Notes", notes);
        parameters.Add("@ModifiedBy", modifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task PoCloseById(int recId, string modifiedBy, CancellationToken ct)
    {
        const string query = "Po_CloseById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);
        parameters.Add("@ModifiedBy", modifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task<IEnumerable<PoHeaderOutstanding>> PoOsSelect(int supplierId, CancellationToken ct)
    {
        const string query = "Po_OsSelect";
        var parameters = new DynamicParameters();
        parameters.Add("@SupplierId", supplierId);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<PoHeaderOutstanding>(command);
    }

    public async Task<IEnumerable<PoLineOutstanding>> PoOsSelectById(int recId, CancellationToken ct)
    {
        const string query = "Po_OsSelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<PoLineOutstanding>(command);
    }
}
