using System.Data;
using Dapper;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class ItemDeptRepo : IItemDeptRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;
    
    public ItemDeptRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<ItemDept>> CboAsync(CboRequest cboRequest, CancellationToken cancellationToken)
    {
        const string query = "ItemDeptCbo";
        var parameters = new DynamicParameters();
        
        if (cboRequest.Id > 0)
        {
            parameters.Add("@Id", cboRequest.Id);
        }
        
        if (!string.IsNullOrEmpty(cboRequest.Term))
        {
            parameters.Add("@Term", cboRequest.Term);
        }
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        var result = await _dbConnection.QueryAsync<ItemDept>(command);
        return result;
    }

    public async Task<ItemDept> SelectByIdAsync(int itemDeptId, CancellationToken cancellationToken)
    {
        const string query = "ItemDeptSelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemDeptId", itemDeptId);
        
        var command = new CommandDefinition(
            query,
            parameters,
            transaction: _dbTransaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
        
        var result = await _dbConnection.QuerySingleOrDefaultAsync<ItemDept>(command);
        return result ?? throw new KeyNotFoundException($"ItemDept with ID {itemDeptId} not found.");
    }

    public async Task<PagedList<ItemDept>> SelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken)
    {
        const string query = "ItemDeptSelect";
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
        var items = result.Read<ItemDept>().ToList();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<ItemDept>(items, listRequest.PageNumber, listRequest.PageSize, totalCount);
    }

    public async Task<ItemDept> InsertAsync(ItemDept itemDept, CancellationToken cancellationToken)
    {
        const string query = "ItemDeptInsert";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemDeptName", itemDept.ItemDeptName);
        parameters.Add("@StatusId", itemDept.StatusId);
        parameters.Add("@CreatedBy", itemDept.CreatedBy);
        
        var command = new CommandDefinition(
            query,
            parameters,
            transaction: _dbTransaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
        
        await _dbConnection.ExecuteAsync(command);
        itemDept.ItemDeptId = parameters.Get<int>("@ItemDeptId");
        return itemDept;
    }

    public async Task<ItemDept> UpdateAsync(ItemDept itemDept, CancellationToken cancellationToken)
    {
        const string query = "ItemDeptUpdate";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemDeptId", itemDept.ItemDeptId);
        parameters.Add("@ItemDeptName", itemDept.ItemDeptName);
        parameters.Add("@StatusId", itemDept.StatusId);
        parameters.Add("@ModifiedBy", itemDept.ModifiedBy);
        
        var command = new CommandDefinition(
            query,
            parameters,
            transaction: _dbTransaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
        
        await _dbConnection.ExecuteAsync(command);
        return itemDept;
    }

    public async Task DeleteAsync(int itemDeptId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "ItemDeptDelete";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemDeptId", itemDeptId);
        parameters.Add("@ModifiedBy", modifiedBy);
        
        var command = new CommandDefinition(
            query,
            parameters,
            transaction: _dbTransaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
        
        await _dbConnection.ExecuteAsync(command);
    }
}