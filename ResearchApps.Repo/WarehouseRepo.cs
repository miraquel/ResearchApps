using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class WarehouseRepo : IWarehouseRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public WarehouseRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<Warehouse>> CboAsync()
    {
        const string query = "WhCbo";
        var command = new CommandDefinition(
            query,
            transaction: _dbTransaction,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryAsync<Warehouse>(command);
        return result;
    }

    public async Task DeleteAsync(int whId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "WhDelete";
        var parameters = new DynamicParameters();
        parameters.Add("@WhId", whId);
        parameters.Add("@ModifiedBy", modifiedBy);

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
            throw new RepoException($"ItemType with ID {whId} not found for deletion.");
        }
    }

    public async Task<Warehouse> InsertAsync(Warehouse warehouse, CancellationToken cancellationToken)
    {
        const string query = "WhInsert";
        var parameters = new DynamicParameters();
        parameters.Add("@WhName", warehouse.WhName);
        parameters.Add("@StatusId", warehouse.StatusId);
        parameters.Add("@CreatedBy", warehouse.CreatedBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QuerySingleAsync<Warehouse>(command);
        
        if (result == null)
        {
            throw new RepoException<Warehouse>("Failed to insert Warehouse", warehouse);
        }
        
        return result;
    }

    public async Task<PagedList<Warehouse>> SelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken)
    {
        const string query = "WhSelect";
        var parameters = new DynamicParameters();
        // loop Filters
        foreach (var filter in listRequest.Filters)
        {
            if (filter.Value is { } strValue && strValue.Contains('%'))
            {
                parameters.Add($"@{filter.Key}", strValue);
            }
            else
            {
                parameters.Add($"@{filter.Key}", $"%{filter.Value}%");
            }
        }
        
        parameters.Add("@PageNumber", listRequest.PageNumber);
        parameters.Add("@PageSize", listRequest.PageSize);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<Warehouse>().ToList();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<Warehouse>(items, listRequest.PageNumber, listRequest.PageSize, totalCount);
    }

    public async Task<Warehouse> SelectByIdAsync(int whId, CancellationToken cancellationToken)
    {
        const string query = "WhSelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@WhId", whId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<Warehouse>(command) ?? throw new RepoException($"Warehouse with ID {whId} not found.");
    }

    public async Task<Warehouse> UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken)
    {
        const string query = "WhUpdate";
        var parameters = new DynamicParameters();
        parameters.Add("@WhId", warehouse.WhId);
        parameters.Add("@WhName", warehouse.WhName);
        parameters.Add("@StatusId", warehouse.StatusId);
        parameters.Add("@ModifiedBy", warehouse.ModifiedBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QuerySingleAsync<Warehouse>(command);
        
        return result ?? throw new RepoException<Warehouse>("Failed to update Warehouse", warehouse);
    }
}