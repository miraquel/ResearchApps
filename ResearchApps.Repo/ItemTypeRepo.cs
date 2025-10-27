using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class ItemTypeRepo : IItemTypeRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public ItemTypeRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<ItemType>> ItemTypeSelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken)
    {
        const string query = "ItemTypeSelect";
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
        var items = result.Read<ItemType>().ToList();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<ItemType>(items, listRequest.PageNumber, listRequest.PageSize, totalCount);
    }

    public async Task<ItemType> ItemTypeSelectByIdAsync(int itemTypeId, CancellationToken cancellationToken)
    {
        const string query = "ItemTypeSelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemTypeId", itemTypeId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<ItemType>(command) ?? throw new RepoException($"ItemType with ID {itemTypeId} not found.");
    }

    public async Task<ItemType> ItemTypeInsertAsync(ItemType itemType, CancellationToken cancellationToken)
    {
        const string query = "ItemTypeInsert";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemTypeName", itemType.ItemTypeName);
        // parameters.Add("@StatusId", itemType.StatusId);
        parameters.Add("@CreatedBy", itemType.CreatedBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<ItemType>(command) ?? throw new RepoException<ItemType>("Failed to insert ItemType", itemType);
    }

    public async Task<ItemType> ItemTypeUpdateAsync(ItemType itemType, CancellationToken cancellationToken)
    {
        const string query = "ItemTypeUpdate";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemTypeId", itemType.ItemTypeId);
        parameters.Add("@ItemTypeName", itemType.ItemTypeName);
        parameters.Add("@StatusId", itemType.StatusId);
        parameters.Add("@ModifiedBy", itemType.ModifiedBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<ItemType>(command) ?? throw new RepoException<ItemType>("Failed to update ItemType", itemType);
    }

    public async Task ItemTypeDeleteAsync(int itemTypeId, CancellationToken cancellationToken)
    {
        const string query = "ItemTypeDelete";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemTypeId", itemTypeId);

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
            throw new RepoException($"ItemType with ID {itemTypeId} not found.");
        }
    }

    public async Task<IEnumerable<ItemType>> ItemTypeCbo(CboRequest pagedCboRequest,
        CancellationToken cancellationToken)
    {
        const string query = "ItemTypeCbo";
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(pagedCboRequest.Term))
        {
            parameters.Add("@Term", pagedCboRequest.Term.Contains('%') ? pagedCboRequest.Term : $"%{pagedCboRequest.Term}%");
        }
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<ItemType>(command);
    }
}