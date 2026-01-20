using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class SalesInvoiceRepo : ISalesInvoiceRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public SalesInvoiceRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<SalesInvoiceHeader>> SiSelect(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Si_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    // Special handling for date fields
                    case "SiDate" or "CreatedDate" or "ModifiedDate" or "SiDateFrom" or "SiDateTo":
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
                    case "SiStatusId" or "CustomerId" or "RecId":
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
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "SiId" : request.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<SalesInvoiceHeader>();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<SalesInvoiceHeader>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<SalesInvoiceHeader> SiSelectById(int id, CancellationToken cancellationToken)
    {
        const string query = "Si_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", id);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstOrDefaultAsync<SalesInvoiceHeader>(command);
        
        return result ?? throw new RepoException($"Sales Invoice with Id {id} not found.");
    }

    public async Task<(int RecId, string SiId)> SiInsert(SalesInvoiceHeader salesInvoice, CancellationToken cancellationToken)
    {
        const string query = "Si_Insert";
        var parameters = new DynamicParameters();
        
        parameters.Add("@SiDate", salesInvoice.SiDate);
        parameters.Add("@CustomerId", salesInvoice.CustomerId);
        parameters.Add("@PoNo", salesInvoice.PoNo ?? string.Empty);
        parameters.Add("@TaxNo", salesInvoice.TaxNo ?? string.Empty);
        parameters.Add("@Notes", salesInvoice.Notes ?? string.Empty);
        parameters.Add("@SiStatusId", salesInvoice.SiStatusId);
        parameters.Add("@CreatedBy", salesInvoice.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstAsync<dynamic>(command);
        return ((int)result.RecId, (string)result.SiId);
    }

    public async Task SiUpdate(SalesInvoiceHeader salesInvoice, CancellationToken cancellationToken)
    {
        const string query = "Si_Update";
        var parameters = new DynamicParameters();
        
        parameters.Add("@RecId", salesInvoice.RecId);
        parameters.Add("@SiDate", salesInvoice.SiDate);
        parameters.Add("@CustomerId", salesInvoice.CustomerId);
        parameters.Add("@PoNo", salesInvoice.PoNo ?? string.Empty);
        parameters.Add("@TaxNo", salesInvoice.TaxNo ?? string.Empty);
        parameters.Add("@Notes", salesInvoice.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", salesInvoice.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        await _dbConnection.ExecuteAsync(command);
    }

    public async Task SiDelete(int recId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Si_Delete";
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

    public async Task<IEnumerable<SalesInvoiceLine>> SiLineSelectBySi(int siRecId, CancellationToken cancellationToken)
    {
        const string query = "SiLine_SelectBySi";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", siRecId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<SalesInvoiceLine>(command);
    }

    public async Task<SalesInvoiceLine?> SiLineSelectById(int siLineId, CancellationToken cancellationToken)
    {
        const string query = "SiLine_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@SiLineId", siLineId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<SalesInvoiceLine>(command);
    }

    public async Task<string> SiLineInsert(SalesInvoiceLine salesInvoiceLine, CancellationToken cancellationToken)
    {
        const string query = "SiLine_Insert";
        var parameters = new DynamicParameters();
        
        parameters.Add("@RecId", salesInvoiceLine.SiRecId);
        parameters.Add("@DoLineId", salesInvoiceLine.DoLineId);
        parameters.Add("@DoId", salesInvoiceLine.DoId);
        parameters.Add("@ItemId", salesInvoiceLine.ItemId);
        parameters.Add("@Qty", salesInvoiceLine.Qty);
        parameters.Add("@Price", salesInvoiceLine.Price);
        parameters.Add("@Notes", salesInvoiceLine.Notes ?? string.Empty);
        parameters.Add("@CreatedBy", salesInvoiceLine.CreatedBy);

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
}
