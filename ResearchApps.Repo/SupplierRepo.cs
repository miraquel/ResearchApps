using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class SupplierRepo : ISupplierRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public SupplierRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<Supplier>> SupplierSelect(CancellationToken cancellationToken)
    {
        const string query = "Supplier_Select";

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            transaction: _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<Supplier>(command);
    }

    public async Task<PagedList<Supplier>> SupplierSelect(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Supplier_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    // Special handling for date fields
                    case "CreatedDate" or "ModifiedDate":
                    {
                        if (DateTime.TryParse(strValue, out var dateValue))
                        {
                            parameters.Add($"@{filter.Key}", dateValue);
                        }
                        break;
                    }
                    // Special handling for integer fields
                    case "SupplierId" or "TopId" or "StatusId":
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
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "SupplierName" : request.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<Supplier>();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<Supplier>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<IEnumerable<Supplier>> SupplierSelectForExport(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Supplier_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    case "CreatedDate" or "ModifiedDate":
                    {
                        if (DateTime.TryParse(strValue, out var dateValue))
                        {
                            parameters.Add($"@{filter.Key}", dateValue);
                        }
                        break;
                    }
                    case "SupplierId" or "TopId" or "StatusId":
                    {
                        if (int.TryParse(strValue, out var intValue))
                        {
                            parameters.Add($"@{filter.Key}", intValue);
                        }
                        break;
                    }
                    case "IsPpn":
                    {
                        if (bool.TryParse(strValue, out var boolValue))
                        {
                            parameters.Add($"@{filter.Key}", boolValue);
                        }
                        break;
                    }
                    default:
                        parameters.Add($"@{filter.Key}", strValue);
                        break;
                }
            }
        }
        
        parameters.Add("@SortOrder", request.IsSortAscending ? "ASC" : "DESC");
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "SupplierName" : request.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        return result.Read<Supplier>();
    }

    public async Task<Supplier?> SupplierSelectById(int supplierId, CancellationToken cancellationToken)
    {
        const string query = "Supplier_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@SupplierId", supplierId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Supplier>(command);
    }

    public async Task<Supplier> SupplierInsert(Supplier supplier, CancellationToken cancellationToken)
    {
        const string query = "Supplier_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@SupplierName", supplier.SupplierName);
        parameters.Add("@Address", supplier.Address);
        parameters.Add("@City", supplier.City);
        parameters.Add("@Telp", supplier.Telp);
        parameters.Add("@Fax", supplier.Fax);
        parameters.Add("@Email", supplier.Email);
        parameters.Add("@TopId", supplier.TopId);
        parameters.Add("@IsPpn", supplier.IsPpn);
        parameters.Add("@Npwp", supplier.Npwp);
        parameters.Add("@Notes", supplier.Notes);
        parameters.Add("@StatusId", supplier.StatusId);
        parameters.Add("@CreatedBy", supplier.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryFirstAsync<Supplier>(command);
        
        if (result.SupplierId == -1)
        {
            throw new RepoException<Supplier>("Supplier name already exists", supplier);
        }

        return result;
    }

    public async Task<Supplier> SupplierUpdate(Supplier supplier, CancellationToken cancellationToken)
    {
        const string query = "Supplier_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@SupplierId", supplier.SupplierId);
        parameters.Add("@SupplierName", supplier.SupplierName);
        parameters.Add("@Address", supplier.Address);
        parameters.Add("@City", supplier.City);
        parameters.Add("@Telp", supplier.Telp);
        parameters.Add("@Fax", supplier.Fax);
        parameters.Add("@Email", supplier.Email);
        parameters.Add("@TopId", supplier.TopId);
        parameters.Add("@IsPpn", supplier.IsPpn);
        parameters.Add("@Npwp", supplier.Npwp);
        parameters.Add("@Notes", supplier.Notes);
        parameters.Add("@StatusId", supplier.StatusId);
        parameters.Add("@ModifiedBy", supplier.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryFirstAsync<Supplier>(command);
        
        if (result.SupplierId == -1)
        {
            throw new RepoException<Supplier>("Supplier name already exists", supplier);
        }

        return result;
    }

    public async Task SupplierDelete(int supplierId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Supplier_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@SupplierId", supplierId);
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

    public async Task<IEnumerable<Supplier>> SupplierCbo(CancellationToken cancellationToken)
    {
        const string query = "SupplierCbo";

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            transaction: _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<Supplier>(command);
    }
}
