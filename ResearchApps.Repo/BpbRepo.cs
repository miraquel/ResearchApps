using System.Data;
using Dapper;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class BpbRepo : IBpbRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public BpbRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<BpbHeader>> BpbSelect(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Bpb_Select";
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            transaction: _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var items = await _dbConnection.QueryAsync<BpbHeader>(command);
        var itemsList = items.ToList();
        
        // Apply filters in memory since SP doesn't support pagination yet
        var filtered = ApplyFilters(itemsList, request);
        
        // Apply sorting
        filtered = ApplySorting(filtered, request);
        
        var totalCount = filtered.Count();
        
        // Apply pagination
        var pagedItems = filtered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);
        
        return new PagedList<BpbHeader>(pagedItems, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<IEnumerable<BpbHeader>> BpbSelectForExport(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Bpb_Select";
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            transaction: _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var items = await _dbConnection.QueryAsync<BpbHeader>(command);
        var itemsList = items.ToList();
        
        // Apply filters in memory
        var filtered = ApplyFilters(itemsList, request);
        
        // Apply sorting
        filtered = ApplySorting(filtered, request);
        
        return filtered;
    }

    public async Task<BpbHeader?> BpbSelectById(int id, CancellationToken cancellationToken)
    {
        const string query = "Bpb_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", id);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<BpbHeader>(command);
    }

    public async Task<IEnumerable<BpbHeader>> BpbSelectByProd(string prodId, CancellationToken cancellationToken)
    {
        const string query = "Bpb_Select";
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            transaction: _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var items = await _dbConnection.QueryAsync<BpbHeader>(command);
        
        // Filter by RefId (ProdId)
        return items.Where(x => x.RefId == prodId);
    }

    public async Task<(int RecId, string BpbId)> BpbInsert(BpbHeader bpb, CancellationToken cancellationToken)
    {
        const string query = "Bpb_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@BpbDate", bpb.BpbDate);
        parameters.Add("@Descr", bpb.Descr ?? string.Empty);
        parameters.Add("@RefType", bpb.RefType ?? "Production");
        parameters.Add("@RefId", bpb.RefId ?? string.Empty);
        parameters.Add("@Notes", bpb.Notes ?? string.Empty);
        parameters.Add("@BpbStatusId", bpb.BpbStatusId);
        parameters.Add("@CreatedBy", bpb.CreatedBy);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryFirstAsync<dynamic>(command);
        
        // Check if inventory is locked
        if (result.RecId == -1)
        {
            throw new InvalidOperationException("Inventory is locked for this period. Cannot create BPB.");
        }
        
        return ((int)result.RecId, (string)result.BpbId);
    }

    public async Task BpbUpdate(BpbHeader bpb, CancellationToken cancellationToken)
    {
        const string query = "Bpb_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", bpb.RecId);
        parameters.Add("@Notes", bpb.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", bpb.ModifiedBy);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task BpbDelete(int recId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Bpb_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);
        parameters.Add("@ModifiedBy", modifiedBy);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task<IEnumerable<BpbLine>> BpbLineSelectByBpb(int bpbRecId, CancellationToken cancellationToken)
    {
        const string query = "BpbLine_SelectByBpb";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", bpbRecId);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<BpbLine>(command);
    }

    public async Task<BpbLine?> BpbLineSelectById(int bpbLineId, CancellationToken cancellationToken)
    {
        const string query = "BpbLine_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@BpbLineId", bpbLineId);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<BpbLine>(command);
    }

    public async Task<string> BpbLineInsert(BpbLine bpbLine, CancellationToken cancellationToken)
    {
        const string query = "BpbLine_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", bpbLine.BpbRecId); // Header RecId
        parameters.Add("@ItemId", bpbLine.ItemId);
        parameters.Add("@WhId", bpbLine.WhId);
        parameters.Add("@Qty", bpbLine.Qty);
        parameters.Add("@ProdId", bpbLine.ProdId ?? string.Empty);
        parameters.Add("@Notes", bpbLine.Notes ?? string.Empty);
        parameters.Add("@CreatedBy", bpbLine.CreatedBy);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        // SP returns result message like "1:::BPB24-0001" or "-1:::error message"
        var result = await _dbConnection.QueryFirstAsync<string>(command);
        return result;
    }

    public async Task<string> BpbLineUpdate(BpbLine bpbLine, CancellationToken cancellationToken)
    {
        const string query = "BpbLine_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@BpbLineId", bpbLine.BpbLineId);
        parameters.Add("@ItemId", bpbLine.ItemId);
        parameters.Add("@WhId", bpbLine.WhId);
        parameters.Add("@Qty", bpbLine.Qty);
        parameters.Add("@Notes", bpbLine.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", bpbLine.ModifiedBy);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryFirstAsync<string>(command);
        return result;
    }

    public async Task<string> BpbLineDelete(int bpbLineId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "BpbLine_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@BpbLineId", bpbLineId);
        parameters.Add("@ModifiedBy", modifiedBy);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryFirstAsync<string>(command);
        return result;
    }

    public async Task<(decimal OnHand, decimal BufferStock)> GetStockInfo(int itemId, int whId, CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT ISNULL(a.Qty, 0) as OnHand, ISNULL(i.BufferStock, 0) as BufferStock
            FROM Item i
            LEFT JOIN InventSum a ON a.ItemId = i.ItemId AND a.WhId = @WhId
            WHERE i.ItemId = @ItemId";

        var parameters = new { ItemId = itemId, WhId = whId };

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken);

        var result = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(command);
        
        if (result == null)
            return (0, 0);
            
        return ((decimal)result.OnHand, (decimal)result.BufferStock);
    }

    private static IEnumerable<BpbHeader> ApplyFilters(IEnumerable<BpbHeader> items, PagedListRequest request)
    {
        foreach (var filter in request.Filters)
        {
            if (string.IsNullOrWhiteSpace(filter.Value))
                continue;

            items = filter.Key switch
            {
                "BpbId" => items.Where(x => x.BpbId.Contains(filter.Value, StringComparison.OrdinalIgnoreCase)),
                "RefId" => items.Where(x => x.RefId != null && x.RefId.Contains(filter.Value, StringComparison.OrdinalIgnoreCase)),
                "Descr" => items.Where(x => x.Descr != null && x.Descr.Contains(filter.Value, StringComparison.OrdinalIgnoreCase)),
                "BpbDateFrom" when DateTime.TryParse(filter.Value, out var dateFrom) => items.Where(x => x.BpbDate >= dateFrom),
                "BpbDateTo" when DateTime.TryParse(filter.Value, out var dateTo) => items.Where(x => x.BpbDate <= dateTo),
                "BpbStatusId" when int.TryParse(filter.Value, out var statusId) => items.Where(x => x.BpbStatusId == statusId),
                "CreatedBy" => items.Where(x => x.CreatedBy.Contains(filter.Value, StringComparison.OrdinalIgnoreCase)),
                _ => items
            };
        }

        return items;
    }

    private static IEnumerable<BpbHeader> ApplySorting(IEnumerable<BpbHeader> items, PagedListRequest request)
    {
        if (string.IsNullOrEmpty(request.SortBy))
        {
            return request.IsSortAscending 
                ? items.OrderBy(x => x.BpbId) 
                : items.OrderByDescending(x => x.BpbId);
        }

        return request.SortBy switch
        {
            "BpbId" => request.IsSortAscending ? items.OrderBy(x => x.BpbId) : items.OrderByDescending(x => x.BpbId),
            "BpbDate" => request.IsSortAscending ? items.OrderBy(x => x.BpbDate) : items.OrderByDescending(x => x.BpbDate),
            "RefId" => request.IsSortAscending ? items.OrderBy(x => x.RefId) : items.OrderByDescending(x => x.RefId),
            "Amount" => request.IsSortAscending ? items.OrderBy(x => x.Amount) : items.OrderByDescending(x => x.Amount),
            "BpbStatusId" => request.IsSortAscending ? items.OrderBy(x => x.BpbStatusId) : items.OrderByDescending(x => x.BpbStatusId),
            "CreatedDate" => request.IsSortAscending ? items.OrderBy(x => x.CreatedDate) : items.OrderByDescending(x => x.CreatedDate),
            _ => request.IsSortAscending ? items.OrderBy(x => x.BpbId) : items.OrderByDescending(x => x.BpbId)
        };
    }
}
