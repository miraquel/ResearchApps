using System.Data;
using Dapper;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class ItemGroup01Repo : IItemGroup01Repo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;
    
    public ItemGroup01Repo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<ItemGroup01>> CboAsync(CboRequest cboRequest, CancellationToken cancellationToken)
    {
        const string query = "ItemGroup01_Cbo";
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
        var result = await _dbConnection.QueryAsync<ItemGroup01>(command);
        return result;
    }

    public async Task<ItemGroup01> SelectByIdAsync(int itemGroup01Id, CancellationToken cancellationToken)
    {
        const string query = "ItemGroup01_SelectById";
        
        var parameters = new DynamicParameters();
        parameters.Add("@ItemGroup01Id", itemGroup01Id);
        
        var command = new CommandDefinition(
            query,
            parameters,
            transaction: _dbTransaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var result = await _dbConnection.QuerySingleOrDefaultAsync<ItemGroup01>(command);
        return result ?? throw new KeyNotFoundException($"ItemGroup01 with ID {itemGroup01Id} not found.");
    }

    public async Task<PagedList<ItemGroup01>> SelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken)
    {
        const string query = "ItemGroup01_Select";
        var parameters = new DynamicParameters();
        foreach (var filter in listRequest.Filters)
        {
            if (filter.Value is { } strValue && strValue.Contains('%'))
            {
                parameters.Add($"@{filter.Key}", strValue);
            }
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
        parameters.Add("@SortColumn", string.IsNullOrEmpty(listRequest.SortBy) ? "ItemGroup01Name" : listRequest.SortBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<ItemGroup01>().ToList();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<ItemGroup01>(items, listRequest.PageNumber, listRequest.PageSize, totalCount);
    }

    public async Task<ItemGroup01> InsertAsync(ItemGroup01 itemGroup01, CancellationToken cancellationToken)
    {
        const string query = "ItemGroup01_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemGroup01Name", itemGroup01.ItemGroup01Name);
        //parameters.Add("@StatusId", itemGroup01.StatusId);
        parameters.Add("@CreatedBy", itemGroup01.CreatedBy);
        
        var command = new CommandDefinition(
            query,
            parameters,
            transaction: _dbTransaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
        
        return await _dbConnection.QuerySingleOrDefaultAsync<ItemGroup01>(command) ?? throw new Exception("Failed to insert ItemGroup01.");
    }

    public async Task<ItemGroup01> UpdateAsync(ItemGroup01 itemGroup01, CancellationToken cancellationToken)
    {
        const string query = "ItemGroup01_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemGroup01Id", itemGroup01.ItemGroup01Id);
        parameters.Add("@ItemGroup01Name", itemGroup01.ItemGroup01Name);
        parameters.Add("@StatusId", itemGroup01.StatusId);
        parameters.Add("@ModifiedBy", itemGroup01.ModifiedBy);
        
        var command = new CommandDefinition(
            query,
            parameters,
            transaction: _dbTransaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
        
        await _dbConnection.ExecuteAsync(command);
        return itemGroup01;
    }

    public async Task DeleteAsync(int itemGroup01Id, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "ItemGroup01_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemGroup01Id", itemGroup01Id);
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
