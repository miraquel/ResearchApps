using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class ItemRepo : IItemRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public ItemRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<Item>> CboAsync(CboRequest cboRequest, CancellationToken cancellationToken)
    {
        const string query = "Item_Cbo";
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
        var result = await _dbConnection.QueryAsync<Item>(command);
        return result;
    }

    public async Task DeleteAsync(int itemId, CancellationToken cancellationToken)
    {
        const string query = "Item_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemId", itemId);
        parameters.Add("@ModifiedBy", "Admin"); // TODO: Pass the actual user id
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var rowsAffected =  await _dbConnection.ExecuteAsync(command);
        if (rowsAffected == 0)
        {
            throw new RepoException<int>("Failed to delete Item", itemId);
        }
    }

    public async Task<Item> InsertAsync(Item item, CancellationToken cancellationToken)
    {
        const string query = "Item_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemName", item.ItemName);
        parameters.Add("@ItemTypeId", item.ItemTypeId);
        parameters.Add("@ItemDeptId", item.ItemDeptId);
        parameters.Add("@ItemGroup01Id", item.ItemGroup01Id);
        parameters.Add("@ItemGroup02Id", item.ItemGroup02Id);
        parameters.Add("@BufferStock", item.BufferStock);
        parameters.Add("@UnitId", item.UnitId);
        parameters.Add("@WhId", item.WhId);
        parameters.Add("@PurchasePrice", item.PurchasePrice);
        parameters.Add("@SalesPrice", item.SalesPrice);
        parameters.Add("@CostPrice", item.CostPrice);
        parameters.Add("@Image", item.Image);
        parameters.Add("@Notes", item.Notes);
        parameters.Add("@StatusId", item.StatusId);
        parameters.Add("@CreatedBy", item.CreatedBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<Item>(command) ?? throw new RepoException<Item>("Failed to insert Item", item);
    }

    public async Task<PagedList<Item>> SelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken)
    {
        const string query = "Item_Select";
        var parameters = new DynamicParameters();
        // loop Filters
        foreach (var filter in listRequest.Filters)
        {
            if (filter.Value is { } strValue && strValue.Contains('%'))
            {
                parameters.Add($"@{filter.Key}", strValue);
            }
            // if statusId then convert to int
            else if (filter.Key.Equals("StatusId", StringComparison.OrdinalIgnoreCase) && int.TryParse(filter.Value, out var statusId))
            {
                parameters.Add($"@{filter.Key}", statusId);
            }
            else
            {
                parameters.Add($"@{filter.Key}", $"%{filter.Value}%");
            }
        }
        
        parameters.Add("@PageNumber", listRequest.PageNumber);
        parameters.Add("@PageSize", listRequest.PageSize);
        parameters.Add("@SortOrder", listRequest.IsSortAscending ? "ASC" : "DESC");
        parameters.Add("@SortColumn", string.IsNullOrEmpty(listRequest.SortBy) ? "CustomerName" : listRequest.SortBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<Item>().ToList();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<Item>(items, listRequest.PageNumber, listRequest.PageSize, totalCount);
    }

    public async Task<Item> SelectByIdAsync(int itemId, CancellationToken cancellationToken)
    {
        const string query = "Item_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemId", itemId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryFirstOrDefaultAsync<Item>(command) ?? throw new RepoException($"Item with ID {itemId} not found.");
    }

    public async Task<Item> UpdateAsync(Item item, CancellationToken cancellationToken)
    {
        const string query = "Item_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemId", item.ItemId);
        parameters.Add("@ItemName", item.ItemName);
        parameters.Add("@ItemTypeId", item.ItemTypeId);
        parameters.Add("@ItemDeptId", item.ItemDeptId);
        parameters.Add("@ItemGroup01Id", item.ItemGroup01Id);
        parameters.Add("@ItemGroup02Id", item.ItemGroup02Id);
        parameters.Add("@BufferStock", item.BufferStock);
        parameters.Add("@UnitId", item.UnitId);
        parameters.Add("@WhId", item.WhId);
        parameters.Add("@PurchasePrice", item.PurchasePrice);
        parameters.Add("@SalesPrice", item.SalesPrice);
        parameters.Add("@CostPrice", item.CostPrice);
        parameters.Add("@Image", item.Image);
        parameters.Add("@Notes", item.Notes);
        parameters.Add("@StatusId", item.StatusId);
        parameters.Add("@ModifiedBy", item.ModifiedBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var rowsAffected =  await _dbConnection.ExecuteAsync(command);
        return rowsAffected == 0 ? throw new RepoException<Item>("Failed to update Item", item) : item;
    }
}