using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class DeliveryOrderRepo : IDeliveryOrderRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public DeliveryOrderRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<DeliveryOrderHeader>> DoSelect(PagedListRequest request,
        CancellationToken cancellationToken)
    {
        const string query = "Do_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    // Special handling for date fields
                    case "DoDateFrom" or "DoDateTo" or "CreatedDate" or "ModifiedDate":
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
                    // Special handling for integer fields
                    case "DoStatusId" or "CustomerId" or "RecId":
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
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "DoId" : request.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<DeliveryOrderHeader>();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<DeliveryOrderHeader>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<IEnumerable<DeliveryOrderHeader>> DoSelectForExport(PagedListRequest request,
        CancellationToken cancellationToken)
    {
        const string query = "Do_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    // Special handling for date fields
                    case "DoDateFrom" or "DoDateTo" or "CreatedDate" or "ModifiedDate":
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
                    // Special handling for integer fields
                    case "DoStatusId" or "CustomerId" or "RecId":
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
        
        // Set high page size to get all results
        parameters.Add("@PageNumber", 1);
        parameters.Add("@PageSize", int.MaxValue);
        parameters.Add("@SortOrder", request.IsSortAscending ? "ASC" : "DESC");
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "DoId" : request.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<DeliveryOrderHeader>();
        // Skip reading total count
        
        return items;
    }

    public async Task<DeliveryOrderHeader> DoSelectById(int id, CancellationToken cancellationToken)
    {
        const string query = "Do_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", id);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstOrDefaultAsync<DeliveryOrderHeader>(command);
        
        return result ?? throw new RepoException($"Delivery Order with Id {id} not found.");
    }
    
    public async Task<DeliveryOrder> 
        DoSelectCompositeById(int recId, CancellationToken cancellationToken)
    {
        const string query = "Do_SelectCompositeById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        await using var multi = await _dbConnection.QueryMultipleAsync(command);
        
        var header = await multi.ReadSingleOrDefaultAsync<DeliveryOrderHeader>() 
            ?? throw new RepoException($"Delivery Order with RecId {recId} not found.");
        var lines = await multi.ReadAsync<DeliveryOrderLine>();
        var outstanding = await multi.ReadAsync<DeliveryOrderLineOutstanding>();
        
        return new DeliveryOrder
        {
            Header = header,
            Lines = lines.AsList(),
            Outstanding = outstanding.AsList()
        };
    }

    public async Task<(int RecId, string DoId)> DoInsert(DeliveryOrderHeader deliveryOrder,
        CancellationToken cancellationToken)
    {
        const string query = "Do_Insert";
        var parameters = new DynamicParameters();
        
        parameters.Add("@DoDate", deliveryOrder.DoDate);
        parameters.Add("@Descr", deliveryOrder.Descr ?? string.Empty);
        parameters.Add("@CoId", deliveryOrder.CoId ?? string.Empty);
        parameters.Add("@RefId", deliveryOrder.RefId ?? string.Empty);
        parameters.Add("@Notes", deliveryOrder.Notes ?? string.Empty);
        parameters.Add("@DoStatusId", deliveryOrder.DoStatusId);
        parameters.Add("@CreatedBy", deliveryOrder.CreatedBy);
        parameters.Add("@CustomerId", deliveryOrder.CustomerId);
        parameters.Add("@Dn", deliveryOrder.Dn ?? string.Empty);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstAsync<dynamic>(command);
        return ((int)result.RecId, (string)result.DoId);
    }

    public async Task DoUpdate(DeliveryOrderHeader deliveryOrder, CancellationToken cancellationToken)
    {
        const string query = "Do_Update";
        var parameters = new DynamicParameters();
        
        parameters.Add("@RecId", deliveryOrder.RecId);
        parameters.Add("@DoDate", deliveryOrder.DoDate);
        parameters.Add("@Descr", deliveryOrder.Descr ?? string.Empty);
        parameters.Add("@CustomerId", deliveryOrder.CustomerId);
        parameters.Add("@Dn", deliveryOrder.Dn ?? string.Empty);
        parameters.Add("@CoId", deliveryOrder.CoId ?? string.Empty);
        parameters.Add("@RefId", deliveryOrder.RefId ?? string.Empty);
        parameters.Add("@Notes", deliveryOrder.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", deliveryOrder.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task DoDelete(int recId, CancellationToken cancellationToken)
    {
        const string query = "Do_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task<IEnumerable<DeliveryOrderHeaderOutstanding>> DoHdOsSelect(int customerId,
        CancellationToken cancellationToken)
    {
        const string query = "Do_HdOsSelect";
        var parameters = new DynamicParameters();
        parameters.Add("@CustomerId", customerId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<DeliveryOrderHeaderOutstanding>(command);
    }

    public async Task<IEnumerable<DeliveryOrderLineOutstanding>> DoOsSelect(int customerId,
        CancellationToken cancellationToken)
    {
        const string query = "Do_OsSelect";
        var parameters = new DynamicParameters();
        parameters.Add("@CustomerId", customerId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<DeliveryOrderLineOutstanding>(command);
    }

    public async Task<DeliveryOrderLineOutstanding> DoOsByDoLineId(int doLineId, CancellationToken cancellationToken)
    {
        const string query = "Do_OsByDoLineId";
        var parameters = new DynamicParameters();
        parameters.Add("@DoLineId", doLineId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstOrDefaultAsync<DeliveryOrderLineOutstanding>(command);
        
        return result ?? throw new RepoException($"DO Line with Id {doLineId} not found.");
    }

    public async Task<IEnumerable<DeliveryOrderLine>> DoLineSelectByDo(int doRecId, CancellationToken cancellationToken)
    {
        const string query = "DoLine_SelectByDo";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", doRecId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<DeliveryOrderLine>(command);
    }

    public async Task<DeliveryOrderLine> DoLineSelectById(int doLineId, CancellationToken cancellationToken)
    {
        const string query = "DoLine_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@DoLineId", doLineId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstOrDefaultAsync<DeliveryOrderLine>(command);
        
        if (result == null)
        {
            throw new RepoException($"DO Line with Id {doLineId} not found.");
        }

        return result;
    }

    public async Task<string> DoLineInsert(DeliveryOrderLine deliveryOrderLine, CancellationToken cancellationToken)
    {
        const string query = "DoLine_Insert";
        var parameters = new DynamicParameters();
        
        parameters.Add("@RecId", deliveryOrderLine.DoRecId);
        parameters.Add("@ItemId", deliveryOrderLine.ItemId);
        parameters.Add("@WhId", deliveryOrderLine.WhId);
        parameters.Add("@Qty", deliveryOrderLine.Qty);
        parameters.Add("@Notes", deliveryOrderLine.Notes ?? string.Empty);
        parameters.Add("@CreatedBy", deliveryOrderLine.CreatedBy);
        parameters.Add("@CoLineId", deliveryOrderLine.CoLineId);
        parameters.Add("@CoId", deliveryOrderLine.CoId ?? string.Empty);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QuerySingleAsync<string>(command);
    }

    public async Task DoLineUpdate(DeliveryOrderLine deliveryOrderLine, CancellationToken cancellationToken)
    {
        const string query = "DoLine_Update";
        var parameters = new DynamicParameters();
        
        parameters.Add("@DoLineId", deliveryOrderLine.DoLineId);
        parameters.Add("@Qty", deliveryOrderLine.Qty);
        parameters.Add("@WhId", deliveryOrderLine.WhId);
        parameters.Add("@Notes", deliveryOrderLine.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", deliveryOrderLine.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task DoLineDelete(int doLineId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "DoLine_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@DoLineId", doLineId);
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
    
    public async Task<IEnumerable<WfTransHistory>> WfTransSelectByRefId(string refId, int wfFormId, CancellationToken cancellationToken)
    {
        const string query = "WfTrans_SelectByRefId";
        var parameters = new DynamicParameters();
        parameters.Add("@RefId", refId);
        parameters.Add("@WfFormId", wfFormId);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<WfTransHistory>(command);
    }
}
