using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class ItemGroup02Repo : IItemGroup02Repo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;
    
    public ItemGroup02Repo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<ItemGroup02>> CboAsync(CboRequest cboRequest, CancellationToken cancellationToken)
    {
        const string query = "ItemGroup02_Cbo";
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
        var result = await _dbConnection.QueryAsync<ItemGroup02>(command);
        return result;
    }

    public async Task<ItemGroup02> SelectByIdAsync(int itemGroup02Id, CancellationToken cancellationToken)
    {
        const string query = "ItemGroup02_SelectById";
        
        var parameters = new DynamicParameters();
        parameters.Add("@ItemGroup02Id", itemGroup02Id);
        
        var command = new CommandDefinition(
            query,
            parameters,
            transaction: _dbTransaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var result = await _dbConnection.QuerySingleOrDefaultAsync<ItemGroup02>(command);
        return result ?? throw new KeyNotFoundException($"ItemGroup02 with ID {itemGroup02Id} not found.");
    }

    public async Task<PagedList<ItemGroup02>> SelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken)
    {
        const string query = "ItemGroup02_Select";
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
        parameters.Add("@SortColumn", string.IsNullOrEmpty(listRequest.SortBy) ? "ItemGroup02Name" : listRequest.SortBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<ItemGroup02>().ToList();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<ItemGroup02>(items, listRequest.PageNumber, listRequest.PageSize, totalCount);
    }

    public async Task<ItemGroup02> InsertAsync(ItemGroup02 itemGroup02, CancellationToken cancellationToken)
    {
        const string query = "ItemGroup02_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemGroup02Name", itemGroup02.ItemGroup02Name);
        //parameters.Add("@StatusId", itemGroup02.StatusId);
        parameters.Add("@CreatedBy", itemGroup02.CreatedBy);
        
        var command = new CommandDefinition(
            query,
            parameters,
            transaction: _dbTransaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await _dbConnection.QuerySingleOrDefaultAsync<ItemGroup02>(command) ?? throw new RepoException("Failed to insert ItemGroup02.");
    }

    public async Task<ItemGroup02> UpdateAsync(ItemGroup02 itemGroup02, CancellationToken cancellationToken)
    {
        const string query = "ItemGroup02_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemGroup02Id", itemGroup02.ItemGroup02Id);
        parameters.Add("@ItemGroup02Name", itemGroup02.ItemGroup02Name);
        parameters.Add("@StatusId", itemGroup02.StatusId);
        parameters.Add("@ModifiedBy", itemGroup02.ModifiedBy);
        
        var command = new CommandDefinition(
            query,
            parameters,
            transaction: _dbTransaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
        
        await _dbConnection.ExecuteAsync(command);
        return itemGroup02;
    }

    public async Task DeleteAsync(int itemGroup02Id, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "ItemGroup02_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemGroup02Id", itemGroup02Id);
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
