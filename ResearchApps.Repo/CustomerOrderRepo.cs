using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class CustomerOrderRepo : ICustomerOrderRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public CustomerOrderRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<CustomerOrderHeader>> CoSelect(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Co_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    // Special handling for date fields
                    case "CoDate" or "CreatedDate" or "ModifiedDate" or "CoDateFrom" or "CoDateTo":
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
                    case "CoStatusId" or "CustomerId" or "CoTypeId" or "RecId" or "Revision":
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
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "CoId" : request.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<CustomerOrderHeader>();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<CustomerOrderHeader>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<IEnumerable<CustomerOrderHeader>> CoSelectForExport(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Co_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    // Special handling for date fields
                    case "CoDate" or "CreatedDate" or "ModifiedDate" or "CoDateFrom" or "CoDateTo":
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
                    case "CoStatusId" or "CustomerId" or "CoTypeId" or "RecId" or "Revision":
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
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "CoId" : request.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<CustomerOrderHeader>();
        // Skip reading total count
        
        return items;
    }

    public async Task<CustomerOrderHeader> CoSelectById(int id, CancellationToken cancellationToken)
    {
        const string query = "Co_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", id);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstOrDefaultAsync<CustomerOrderHeader>(command);
        
        return result ?? throw new RepoException($"Customer Order with Id {id} not found.");
    }

    public async Task<(int RecId, string CoId)> CoInsert(CustomerOrderHeader customerOrder,
        CancellationToken cancellationToken)
    {
        const string query = "Co_Insert";
        var parameters = new DynamicParameters();
        
        parameters.Add("@CustomerId", customerOrder.CustomerId);
        parameters.Add("@CoDate", customerOrder.CoDate);
        parameters.Add("@PoCustomer", customerOrder.PoCustomer ?? string.Empty);
        parameters.Add("@RefNo", customerOrder.RefNo ?? string.Empty);
        parameters.Add("@CoTypeId", customerOrder.CoTypeId);
        parameters.Add("@SubTotal", customerOrder.SubTotal);
        parameters.Add("@Ppn", customerOrder.Ppn);
        parameters.Add("@Total", customerOrder.Total);
        parameters.Add("@Notes", customerOrder.Notes ?? string.Empty);
        parameters.Add("@CoStatusId", customerOrder.CoStatusId);
        parameters.Add("@CreatedBy", customerOrder.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstAsync<dynamic>(command);
        return ((int)result.RecId, (string)result.CoId);
    }

    public async Task CoUpdate(CustomerOrderHeader customerOrder, CancellationToken cancellationToken)
    {
        const string query = "Co_Update";
        var parameters = new DynamicParameters();
        
        parameters.Add("@RecId", customerOrder.RecId);
        parameters.Add("@CustomerId", customerOrder.CustomerId);
        parameters.Add("@CoDate", customerOrder.CoDate);
        parameters.Add("@PoCustomer", customerOrder.PoCustomer ?? string.Empty);
        parameters.Add("@RefNo", customerOrder.RefNo ?? string.Empty);
        parameters.Add("@CoTypeId", customerOrder.CoTypeId);
        parameters.Add("@Notes", customerOrder.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", customerOrder.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task CoDelete(int recId, CancellationToken cancellationToken)
    {
        const string query = "Co_Delete";
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

    public async Task CoSubmitById(int recId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Co_SubmitById";
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
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task CoRecallById(int recId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Co_RecallById";
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
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task CoRejectById(int recId, string modifiedBy, string notes, CancellationToken cancellationToken)
    {
        const string query = "Co_RejectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);
        parameters.Add("@ModifiedBy", modifiedBy);
        parameters.Add("@Notes", notes);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task CoCloseByNo(string coId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Co_CloseByNo";
        var parameters = new DynamicParameters();
        parameters.Add("@CoId", coId);
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

    public async Task CoApproveById(int recId, string modifiedBy, string notes, CancellationToken cancellationToken)
    {
        const string query = "Co_ApproveById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);
        parameters.Add("@ModifiedBy", modifiedBy);
        parameters.Add("@Notes", notes);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task<IEnumerable<CustomerOrderHeaderOutstanding>> CoHdOsSelect(int customerId, CancellationToken cancellationToken)
    {
        const string query = "Co_HdOsSelect";
        var parameters = new DynamicParameters();
        parameters.Add("@CustomerId", customerId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<CustomerOrderHeaderOutstanding>(command);
    }

    public async Task<IEnumerable<CustomerOrderLineOutstanding>> CoOsSelect(int customerId, CancellationToken cancellationToken)
    {
        const string query = "Co_OsSelect";
        var parameters = new DynamicParameters();
        parameters.Add("@CustomerId", customerId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<CustomerOrderLineOutstanding>(command);
    }

    public async Task<IEnumerable<CustomerOrderLineOutstanding>> CoOsById(int coRecId, CancellationToken cancellationToken)
    {
        const string query = "Co_OsById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", coRecId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<CustomerOrderLineOutstanding>(command);
    }

    public async Task<CustomerOrderLineOutstanding?> CoOsByCoLineId(int coLineId, CancellationToken cancellationToken)
    {
        const string query = "Co_OsByCoLineId";
        var parameters = new DynamicParameters();
        parameters.Add("@CoLineId", coLineId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<CustomerOrderLineOutstanding>(command);
    }

    public async Task<IEnumerable<CustomerOrderLine>> CoLineSelectByCo(int coRecId, CancellationToken cancellationToken)
    {
        const string query = "CoLine_SelectByCo";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", coRecId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<CustomerOrderLine>(command);
    }

    public async Task<CustomerOrderLine?> CoLineSelectById(int coLineId, CancellationToken cancellationToken)
    {
        const string query = "CoLine_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@CoLineId", coLineId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstOrDefaultAsync<CustomerOrderLine>(command);

        return result;
    }

    public async Task<string> CoLineInsert(CustomerOrderLine customerOrderLine, CancellationToken cancellationToken)
    {
        const string query = "CoLine_Insert";
        var parameters = new DynamicParameters();
        
        parameters.Add("@RecId", customerOrderLine.CoRecId);
        parameters.Add("@ItemId", customerOrderLine.ItemId);
        parameters.Add("@WantedDeliveryDate", customerOrderLine.WantedDeliveryDate);
        parameters.Add("@Qty", customerOrderLine.Qty);
        parameters.Add("@Price", customerOrderLine.Price);
        parameters.Add("@Notes", customerOrderLine.Notes ?? string.Empty);
        parameters.Add("@CreatedBy", customerOrderLine.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QuerySingleAsync<string>(command);
        return result;
    }

    public async Task CoLineUpdate(CustomerOrderLine customerOrderLine, CancellationToken cancellationToken)
    {
        const string query = "CoLine_Update";
        var parameters = new DynamicParameters();
        
        parameters.Add("@CoLineId", customerOrderLine.CoLineId);
        parameters.Add("@WantedDeliveryDate", customerOrderLine.WantedDeliveryDate);
        parameters.Add("@Qty", customerOrderLine.Qty);
        parameters.Add("@Price", customerOrderLine.Price);
        parameters.Add("@CreatedBy", customerOrderLine.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task CoLineDelete(int coLineId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "CoLine_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@CoLineId", coLineId);
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

    public async Task<IEnumerable<CustomerOrderType>> CoTypeCbo(CancellationToken cancellationToken)
    {
        const string query = "Co_TypeCbo";

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            null,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<CustomerOrderType>(command);
    }

    public async Task<IEnumerable<CoSummaryReportItem>> CoRptSummary(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        const string query = "Co_RptSummary";
        var parameters = new DynamicParameters();
        parameters.Add("@StartDate", startDate);
        parameters.Add("@EndDate", endDate);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<CoSummaryReportItem>(command);
    }
    
    public async Task<IEnumerable<CoDetailReportItem>> CoRptDetail(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        const string query = "Co_RptDetail";
        var parameters = new DynamicParameters();
        parameters.Add("@StartDate", startDate, DbType.DateTime);
        parameters.Add("@EndDate", endDate, DbType.DateTime);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<CoDetailReportItem>(command);
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
