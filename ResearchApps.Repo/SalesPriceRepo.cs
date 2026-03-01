using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class SalesPriceRepo : ISalesPriceRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    static SalesPriceRepo()
    {
        // Map SQL column "SalesPrice" to C# property "SalesPriceValue"
        // because C# does not allow a property with the same name as its enclosing type
        SqlMapper.SetTypeMap(typeof(SalesPrice), new CustomPropertyTypeMap(
            typeof(SalesPrice),
            (type, columnName) =>
            {
                if (string.Equals(columnName, "SalesPrice", StringComparison.OrdinalIgnoreCase))
                    return type.GetProperty("SalesPriceValue")!;

                return type.GetProperties()
                    .FirstOrDefault(p => string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));
            }
        ));
    }

    public SalesPriceRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<SalesPrice>> SelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken)
    {
        const string query = "SalesPrice_Select";
        var parameters = new DynamicParameters();

        foreach (var filter in listRequest.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    case "StatusId":
                    {
                        if (int.TryParse(strValue, out var statusId))
                            parameters.Add($"@{filter.Key}", statusId);
                        break;
                    }
                    case "StartDate" or "EndDate":
                    {
                        if (DateTime.TryParse(strValue, out var dateValue))
                            parameters.Add($"@{filter.Key}", dateValue);
                        break;
                    }
                    default:
                        parameters.Add($"@{filter.Key}", strValue);
                        break;
                }
            }
        }

        parameters.Add("@PageNumber", listRequest.PageNumber);
        parameters.Add("@PageSize", listRequest.PageSize);
        parameters.Add("@SortOrder", listRequest.IsSortAscending ? "ASC" : "DESC");
        parameters.Add("@SortColumn", string.IsNullOrEmpty(listRequest.SortBy) ? "RecId" : listRequest.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<SalesPrice>().ToList();
        var totalCount = result.ReadSingle<int>();

        return new PagedList<SalesPrice>(items, listRequest.PageNumber, listRequest.PageSize, totalCount);
    }

    public async Task<SalesPrice> SelectByIdAsync(int recId, CancellationToken cancellationToken)
    {
        const string query = "SalesPrice_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<SalesPrice>(command)
               ?? throw new RepoException($"SalesPrice with ID {recId} not found.");
    }

    public async Task<SalesPrice> InsertAsync(SalesPrice salesPrice, CancellationToken cancellationToken)
    {
        const string query = "SalesPrice_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@ItemId", salesPrice.ItemId);
        parameters.Add("@CustomerId", salesPrice.CustomerId);
        parameters.Add("@StartDate", salesPrice.StartDate);
        parameters.Add("@EndDate", salesPrice.EndDate);
        parameters.Add("@SalesPrice", salesPrice.SalesPriceValue);
        parameters.Add("@Notes", salesPrice.Notes);
        parameters.Add("@StatusId", salesPrice.StatusId);
        parameters.Add("@CreatedBy", salesPrice.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryFirstOrDefaultAsync<SalesPrice>(command)
                     ?? throw new RepoException<SalesPrice>("Failed to insert SalesPrice", salesPrice);

        // SP does not return RecId in SELECT; retrieve it via @@IDENTITY
        var recId = await _dbConnection.ExecuteScalarAsync<int>(
            "SELECT CAST(@@IDENTITY AS INT)",
            transaction: _dbTransaction);
        result.RecId = recId;

        return result;
    }

    public async Task<SalesPrice> UpdateAsync(SalesPrice salesPrice, CancellationToken cancellationToken)
    {
        const string query = "SalesPrice_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", salesPrice.RecId);
        parameters.Add("@ItemId", salesPrice.ItemId);
        parameters.Add("@CustomerId", salesPrice.CustomerId);
        parameters.Add("@StartDate", salesPrice.StartDate);
        parameters.Add("@EndDate", salesPrice.EndDate);
        parameters.Add("@SalesPrice", salesPrice.SalesPriceValue);
        parameters.Add("@Notes", salesPrice.Notes);
        parameters.Add("@StatusId", salesPrice.StatusId);
        parameters.Add("@ModifiedBy", salesPrice.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<SalesPrice>(command)
               ?? throw new RepoException<SalesPrice>("Failed to update SalesPrice", salesPrice);
    }

    public async Task DeleteAsync(int recId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "SalesPrice_Delete";
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
}
