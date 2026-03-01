using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class GoodsReceiptRepo : IGoodsReceiptRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public GoodsReceiptRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<GoodsReceiptHeader>> GrSelect(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Gr_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    // Special handling for date fields
                    case "GrDate" or "CreatedDate" or "ModifiedDate" or "GrDateFrom" or "GrDateTo":
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
                    case "GrStatusId" or "SupplierId" or "PoRecId" or "RecId":
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
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "GrId" : request.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<GoodsReceiptHeader>();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<GoodsReceiptHeader>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<IEnumerable<GoodsReceiptHeader>> GrSelectForExport(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Gr_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    // Special handling for date fields
                    case "GrDate" or "CreatedDate" or "ModifiedDate" or "GrDateFrom" or "GrDateTo":
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
                    case "GrStatusId" or "SupplierId" or "RecId":
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
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "GrId" : request.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<GoodsReceiptHeader>();
        // Skip reading total count
        
        return items;
    }

    public async Task<GoodsReceiptHeader> GrSelectById(int id, CancellationToken cancellationToken)
    {
        const string query = "Gr_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", id);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstOrDefaultAsync<GoodsReceiptHeader>(command);
        
        return result ?? throw new RepoException($"Goods Receipt with Id {id} not found.");
    }

    public async Task<int> GrInsert(GoodsReceiptHeader goodsReceipt, CancellationToken cancellationToken)
    {
        const string query = "Gr_Insert";
        var parameters = new DynamicParameters();
        
        parameters.Add("@SupplierId", goodsReceipt.SupplierId);
        parameters.Add("@GrDate", goodsReceipt.GrDate);
        parameters.Add("@RefNo", goodsReceipt.RefNo ?? string.Empty);
        parameters.Add("@SubTotal", goodsReceipt.SubTotal);
        parameters.Add("@Ppn", goodsReceipt.Ppn);
        parameters.Add("@Total", goodsReceipt.Total);
        parameters.Add("@Notes", goodsReceipt.Notes ?? string.Empty);
        parameters.Add("@GrStatusId", goodsReceipt.GrStatusId);
        parameters.Add("@CreatedBy", goodsReceipt.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var recId = await _dbConnection.QueryFirstAsync<int>(command);
        return recId;
    }

    public async Task GrUpdate(GoodsReceiptHeader goodsReceipt, CancellationToken cancellationToken)
    {
        const string query = "Gr_Update";
        var parameters = new DynamicParameters();
        
        parameters.Add("@RecId", goodsReceipt.RecId);
        parameters.Add("@GrDate", goodsReceipt.GrDate);
        parameters.Add("@RefNo", goodsReceipt.RefNo ?? string.Empty);
        parameters.Add("@Notes", goodsReceipt.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", goodsReceipt.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task GrDelete(int recId, CancellationToken cancellationToken)
    {
        const string query = "Gr_Delete";
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

    public async Task<IEnumerable<GoodsReceiptLine>> GrLineSelectByGr(int grRecId, CancellationToken cancellationToken)
    {
        const string query = "GrLine_SelectByGr";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", grRecId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<GoodsReceiptLine>(command);
    }

    public async Task<GoodsReceiptLine?> GrLineSelectById(int grLineId, CancellationToken cancellationToken)
    {
        const string query = "GrLine_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@GrLineId", grLineId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<GoodsReceiptLine>(command);
    }

    public async Task<int> GrLineInsert(GoodsReceiptLine goodsReceiptLine, CancellationToken cancellationToken)
    {
        const string query = "GrLine_Insert";
        var parameters = new DynamicParameters();
        
        parameters.Add("@RecId", goodsReceiptLine.GrRecId);
        parameters.Add("@PoLineId", goodsReceiptLine.PoLineId);
        parameters.Add("@Qty", goodsReceiptLine.Qty);
        parameters.Add("@WhId", goodsReceiptLine.WhId);
        parameters.Add("@Notes", goodsReceiptLine.Notes ?? string.Empty);
        parameters.Add("@CreatedBy", goodsReceiptLine.CreatedBy);

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

    public async Task GrLineUpdate(GoodsReceiptLine goodsReceiptLine, CancellationToken cancellationToken)
    {
        const string query = "GrLine_Update";
        var parameters = new DynamicParameters();
        
        parameters.Add("@GrLineId", goodsReceiptLine.GrLineId);
        parameters.Add("@PoLineId", goodsReceiptLine.PoLineId);
        parameters.Add("@Qty", goodsReceiptLine.Qty);
        parameters.Add("@WhId", goodsReceiptLine.WhId);
        parameters.Add("@Notes", goodsReceiptLine.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", goodsReceiptLine.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task GrLineDelete(int grLineId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "GrLine_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@GrLineId", grLineId);
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

    public async Task<IEnumerable<PoLineOutstanding>> PoOsSelectBySupplier(int supplierId, CancellationToken cancellationToken)
    {
        const string query = "Po_OsSelect";
        var parameters = new DynamicParameters();
        parameters.Add("@SupplierId", supplierId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<PoLineOutstanding>(command);
    }

    public async Task<PoLineOutstanding?> PoOsSelectById(int poLineId, CancellationToken cancellationToken)
    {
        const string query = "Po_OsSelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@PoLineId", poLineId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<PoLineOutstanding>(command);
    }

    public async Task<IEnumerable<GrReportItem>> GrRpt(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        const string query = "Gr_Rpt";
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
        
        return await _dbConnection.QueryAsync<GrReportItem>(command);
    }
}
