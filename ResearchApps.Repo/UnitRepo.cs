using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class UnitRepo : IUnitRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public UnitRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<IEnumerable<Unit>> UnitCboAsync(CboRequest cboRequest, CancellationToken cancellationToken)
    {
        const string query = "UnitCbo";
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
        var result = await _dbConnection.QueryAsync<Unit>(command);
        return result;
    }

    public async Task UnitDeleteAsync(int unitId, CancellationToken cancellationToken)
    {
        const string query = "UnitDelete";
        var parameters = new DynamicParameters();
        parameters.Add("@UnitId", unitId);
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
            throw new RepoException($"Unit with ID {unitId} not found for deletion.");
        }
    }

    public async Task<Unit> UnitInsertAsync(Unit unit, CancellationToken cancellationToken)
    {
        const string query = "UnitInsert";
        var parameters = new DynamicParameters();
        parameters.Add("@UnitName", unit.UnitName);
        parameters.Add("@StatusId", unit.StatusId);
        parameters.Add("@CreatedBy", unit.CreatedBy);
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        var result = await _dbConnection.QuerySingleAsync<Unit>(command);
        if (result == null)
        {
            throw new RepoException<Unit>("Failed to insert Unit", unit);
        }
        return result;
    }

    public async Task<PagedList<Unit>> UnitSelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken)
    {
        const string query = "UnitSelect";
        var parameters = new DynamicParameters();
        foreach (var filter in listRequest.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrEmpty(strValue) && strValue.Contains('%'))
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
        var items = result.Read<Unit>().ToList();
        var totalCount = result.ReadSingle<int>();
        return new PagedList<Unit>(items, listRequest.PageNumber, listRequest.PageSize, totalCount);
    }

    public async Task<Unit> UnitSelectByIdAsync(int unitId, CancellationToken cancellationToken)
    {
        const string query = "UnitSelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@UnitId", unitId);
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        return await _dbConnection.QueryFirstOrDefaultAsync<Unit>(command) ?? throw new RepoException($"Unit with ID {unitId} not found.");
    }

    public async Task<Unit> UnitUpdateAsync(Unit unit, CancellationToken cancellationToken)
    {
        const string query = "UnitUpdate";
        var parameters = new DynamicParameters();
        parameters.Add("@UnitId", unit.UnitId);
        parameters.Add("@UnitName", unit.UnitName);
        parameters.Add("@StatusId", unit.StatusId);
        parameters.Add("@ModifiedBy", unit.ModifiedBy);
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        var result = await _dbConnection.QuerySingleAsync<Unit>(command);
        return result ?? throw new RepoException<Unit>("Failed to update Unit", unit);
    }
}