using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class LocationRepo : ILocationRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public LocationRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<Location>> LocationCboAsync(CboRequest cboRequest, CancellationToken cancellationToken)
    {
        const string query = "LocationCbo";
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
        var result = await _dbConnection.QueryAsync<Location>(command);
        return result;
    }

    public async Task LocationDeleteAsync(int locationId, CancellationToken cancellationToken)
    {
        const string query = "LocationDelete";
        var parameters = new DynamicParameters();
        parameters.Add("@LocationId", locationId);
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
            throw new RepoException($"Location with ID {locationId} not found for deletion.");
        }
    }

    public async Task<Location> LocationInsertAsync(Location location, CancellationToken cancellationToken)
    {
        const string query = "LocationInsert";
        var parameters = new DynamicParameters();
        parameters.Add("@LocationName", location.LocationName);
        parameters.Add("@StatusId", location.StatusId);
        parameters.Add("@CreatedBy", location.CreatedBy);
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        var result = await _dbConnection.QuerySingleAsync<Location>(command);
        if (result == null)
        {
            throw new RepoException<Location>("Failed to insert Location", location);
        }
        return result;
    }

    public async Task<PagedList<Location>> LocationSelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken)
    {
        const string query = "LocationSelect";
        var parameters = new DynamicParameters();
        foreach (var filter in listRequest.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrEmpty(strValue) && strValue.Contains('%'))
            {
                parameters.Add($"@{filter.Key}", strValue);
            }
            else if (filter.Key.Equals("StatusId", StringComparison.OrdinalIgnoreCase) &&
                     int.TryParse(filter.Value, out var statusId))
            {
                parameters.Add("@StatusId", statusId);
            }
            else
            {
                parameters.Add($"@{filter.Key}", $"%{filter.Value}%");
            }
        }
        parameters.Add("@PageNumber", listRequest.PageNumber);
        parameters.Add("@PageSize", listRequest.PageSize);
        parameters.Add("@SortOrder", listRequest.IsSortAscending ? "ASC" : "DESC");
        parameters.Add("@SortColumn", string.IsNullOrEmpty(listRequest.SortBy) ? "LocationId" : listRequest.SortBy);
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<Location>().ToList();
        var totalCount = result.ReadSingle<int>();
        return new PagedList<Location>(items, listRequest.PageNumber, listRequest.PageSize, totalCount);
    }

    public async Task<Location> LocationSelectByIdAsync(int locationId, CancellationToken cancellationToken)
    {
        const string query = "LocationSelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@LocationId", locationId);
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        return await _dbConnection.QueryFirstOrDefaultAsync<Location>(command) ?? throw new RepoException($"Location with ID {locationId} not found.");
    }

    public async Task<Location> LocationUpdateAsync(Location location, CancellationToken cancellationToken)
    {
        const string query = "LocationUpdate";
        var parameters = new DynamicParameters();
        parameters.Add("@LocationId", location.LocationId);
        parameters.Add("@LocationName", location.LocationName);
        parameters.Add("@StatusId", location.StatusId);
        parameters.Add("@ModifiedBy", location.ModifiedBy);
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        var result = await _dbConnection.QuerySingleAsync<Location>(command);
        return result ?? throw new RepoException<Location>("Failed to update Location", location);
    }
}
