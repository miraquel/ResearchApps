using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class PhpRepo : IPhpRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public PhpRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<PhpHeader>> PhpSelect(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Php_Select";
        var parameters = new DynamicParameters();

        // Add pagination parameters
        parameters.Add("@PageNumber", request.PageNumber);
        parameters.Add("@PageSize", request.PageSize);
        parameters.Add("@SortColumn", request.SortBy ?? "PhpDate");
        parameters.Add("@SortOrder", request.IsSortAscending ? "ASC" : "DESC");

        // Add filter parameters
        foreach (var filter in request.Filters)
        {
            parameters.Add($"@{filter.Key}", filter.Value);
        }

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        // Read multiple result sets (items + total count)
        using var multi = await _dbConnection.QueryMultipleAsync(command);
        var items = await multi.ReadAsync<PhpHeader>();
        var totalCount = await multi.ReadFirstAsync<int>();

        return new PagedList<PhpHeader>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<PhpHeader> PhpSelectById(int recId, CancellationToken cancellationToken)
    {
        const string query = "Php_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryFirstOrDefaultAsync<PhpHeader>(command);

        return result ?? throw new RepoException($"Php with RecId {recId} not found.");
    }

    public async Task<(int RecId, string PhpId)> PhpInsert(PhpHeader php, CancellationToken cancellationToken)
    {
        const string query = "Php_Insert";
        var parameters = new DynamicParameters();

        parameters.Add("@PhpDate", php.PhpDate);
        parameters.Add("@Descr", php.Descr ?? string.Empty);
        parameters.Add("@RefId", php.RefId ?? string.Empty);
        parameters.Add("@Notes", php.Notes ?? string.Empty);
        parameters.Add("@PhpStatusId", php.PhpStatusId);
        parameters.Add("@CreatedBy", php.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        // Php_Insert returns: SELECT SCOPE_IDENTITY() as RecId, @PhpId as PhpId
        var result = await _dbConnection.QueryFirstAsync<dynamic>(command);
        
        // Check for inventory lock error (-1)
        if (result.RecId == -1)
        {
            throw new RepoException("Cannot create PHP: Inventory period is locked for this date.");
        }
        
        return ((int)result.RecId, (string)result.PhpId);
    }

    public async Task PhpUpdate(PhpHeader php, CancellationToken cancellationToken)
    {
        const string query = "Php_Update";
        var parameters = new DynamicParameters();

        parameters.Add("@RecId", php.RecId);
        parameters.Add("@Notes", php.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", php.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task PhpDelete(int recId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Php_Delete";
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

    public async Task<IEnumerable<PhpLine>> PhpLineSelectByPhp(int phpRecId, CancellationToken cancellationToken)
    {
        const string query = "PhpLine_SelectByPhp";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", phpRecId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<PhpLine>(command);
    }

    public async Task<PhpLine?> PhpLineSelectById(int phpLineId, CancellationToken cancellationToken)
    {
        const string query = "PhpLine_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@PhpLineId", phpLineId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<PhpLine>(command);
    }

    public async Task<string> PhpLineInsert(PhpLine phpLine, CancellationToken cancellationToken)
    {
        const string query = "PhpLine_Insert";
        var parameters = new DynamicParameters();

        parameters.Add("@RecId", phpLine.PhpRecId);
        parameters.Add("@ItemId", phpLine.ItemId);
        parameters.Add("@WhId", phpLine.WhId);
        parameters.Add("@Qty", phpLine.Qty);
        parameters.Add("@ProdId", phpLine.ProdId ?? string.Empty);
        parameters.Add("@Notes", phpLine.Notes ?? string.Empty);
        parameters.Add("@CreatedBy", phpLine.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        // PhpLine_Insert returns PhpId or error message like '-1:::Error message'
        var result = await _dbConnection.QuerySingleAsync<string>(command);
        
        if (result.StartsWith("-1:::"))
        {
            var errorMessage = result.Substring(5); // Remove '-1:::'
            throw new RepoException(errorMessage);
        }
        
        return result;
    }

    public async Task<string> PhpLineUpdate(PhpLine phpLine, CancellationToken cancellationToken)
    {
        const string query = "PhpLine_Update";
        var parameters = new DynamicParameters();

        parameters.Add("@PhpLineId", phpLine.PhpLineId);
        parameters.Add("@ItemId", phpLine.ItemId);
        parameters.Add("@WhId", phpLine.WhId);
        parameters.Add("@Qty", phpLine.Qty);
        parameters.Add("@Notes", phpLine.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", phpLine.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        // PhpLine_Update returns PhpId
        return await _dbConnection.QuerySingleAsync<string>(command);
    }

    public async Task<string> PhpLineDelete(int phpLineId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "PhpLine_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@PhpLineId", phpLineId);
        parameters.Add("@ModifiedBy", modifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        // PhpLine_Delete returns PhpId or error message
        var result = await _dbConnection.QuerySingleAsync<string>(command);
        
        if (result.StartsWith("-1:::"))
        {
            var errorMessage = result.Substring(5);
            throw new RepoException(errorMessage);
        }
        
        return result;
    }
}
