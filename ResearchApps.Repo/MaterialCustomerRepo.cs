using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class MaterialCustomerRepo : IMaterialCustomerRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public MaterialCustomerRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<MaterialCustomerHeader>> McSelect(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Mc_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    case "McId" or "CustomerName" or "SjNo" or "RefNo":
                        parameters.Add($"@{filter.Key}", strValue);
                        break;
                    case "McDate" or "CreatedDate" or "ModifiedDate" or "McDateFrom" or "McDateTo":
                    {
                        if (DateTime.TryParse(strValue, out var dateValue))
                        {
                            parameters.Add($"@{filter.Key}", dateValue);
                        }
                        break;
                    }
                    case "McStatusId" or "CustomerId" or "RecId":
                    {
                        if (int.TryParse(strValue, out var intValue))
                        {
                            parameters.Add($"@{filter.Key}", intValue);
                        }
                        break;
                    }
                    default:
                        parameters.Add($"@{filter.Key}", strValue);
                        break;
                }
            }
        }
        
        parameters.Add("@PageNumber", request.PageNumber);
        parameters.Add("@PageSize", request.PageSize);
        parameters.Add("@SortOrder", request.IsSortAscending ? "ASC" : "DESC");
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "McId" : request.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<MaterialCustomerHeader>();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<MaterialCustomerHeader>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<MaterialCustomerHeader> McSelectById(int id, CancellationToken cancellationToken)
    {
        const string query = "Mc_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", id);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryFirstOrDefaultAsync<MaterialCustomerHeader>(command);

        return result ?? throw new RepoException($"Material Customer with Id {id} not found.");
    }

    public async Task<(int RecId, string McId)> McInsert(MaterialCustomerHeader materialCustomer, CancellationToken cancellationToken)
    {
        const string query = "Mc_Insert";
        var parameters = new DynamicParameters();

        parameters.Add("@McDate", materialCustomer.McDate);
        parameters.Add("@CustomerId", materialCustomer.CustomerId);
        parameters.Add("@SjNo", materialCustomer.SjNo ?? string.Empty);
        parameters.Add("@RefNo", materialCustomer.RefNo ?? string.Empty);
        parameters.Add("@Notes", materialCustomer.Notes ?? string.Empty);
        parameters.Add("@McStatusId", materialCustomer.McStatusId);
        parameters.Add("@CreatedBy", materialCustomer.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryFirstAsync<dynamic>(command);
        
        // Check for inventory lock error
        if (result.RecId == -1)
        {
            throw new RepoException("Cannot create Material Customer. Inventory is locked for this period.");
        }

        return ((int)result.RecId, (string)result.McId);
    }

    public async Task McUpdate(MaterialCustomerHeader materialCustomer, CancellationToken cancellationToken)
    {
        const string query = "Mc_Update";
        var parameters = new DynamicParameters();

        parameters.Add("@RecId", materialCustomer.RecId);
        parameters.Add("@Notes", materialCustomer.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", materialCustomer.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task McDelete(int recId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Mc_Delete";
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

    public async Task<IEnumerable<MaterialCustomerLine>> McLineSelectByMc(int mcRecId, CancellationToken cancellationToken)
    {
        const string query = "McLine_SelectByMc";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", mcRecId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<MaterialCustomerLine>(command);
    }

    public async Task<MaterialCustomerLine?> McLineSelectById(int mcLineId, CancellationToken cancellationToken)
    {
        const string query = "McLine_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@McLineId", mcLineId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<MaterialCustomerLine>(command);
    }

    public async Task<string> McLineInsert(MaterialCustomerLine materialCustomerLine, CancellationToken cancellationToken)
    {
        const string query = "McLine_Insert";
        var parameters = new DynamicParameters();

        parameters.Add("@RecId", materialCustomerLine.RecId);
        parameters.Add("@ItemId", materialCustomerLine.ItemId);
        parameters.Add("@WhId", materialCustomerLine.WhId);
        parameters.Add("@Qty", materialCustomerLine.Qty);
        parameters.Add("@Notes", materialCustomerLine.Notes ?? string.Empty);
        parameters.Add("@CreatedBy", materialCustomerLine.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QuerySingleAsync<string>(command);
        
        // Check for error (returns "-1:::message" on error)
        if (result.StartsWith("-1:::"))
        {
            throw new RepoException(result.Replace("-1:::", string.Empty));
        }

        return result;
    }

    public async Task<string> McLineDelete(int mcLineId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "McLine_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@McLineId", mcLineId);
        parameters.Add("@ModifiedBy", modifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QuerySingleAsync<string>(command);
        
        // Check for error (returns "-1:::message" on error)
        if (result.StartsWith("-1:::"))
        {
            throw new RepoException(result.Replace("-1:::", string.Empty));
        }

        return result;
    }
}
