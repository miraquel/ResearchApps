using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class PrLineRepo : IPrLineRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public PrLineRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<string> PrLineDelete(int id, CancellationToken cancellationToken)
    {
        const string query = "PrLine_Delete";
        var parameters = new DynamicParameters();

        parameters.Add("@PrLineId", id);
        parameters.Add("@ModifiedBy", "system");

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.ExecuteScalarAsync<string>(command);
        
        return string.IsNullOrEmpty(result) ? throw new RepoException($"PrLine with Id {id} could not be deleted.") : result;
    }

    public async Task<string> PrLineInsert(PrLine prLine, CancellationToken cancellationToken)
    {
        const string query = "PrLine_Insert";
        var parameters = new DynamicParameters();

        parameters.Add("@RecId", prLine.PrRecId);
        parameters.Add("@ItemId",  prLine.ItemId);
        parameters.Add("@RequestDate", prLine.RequestDate);
        parameters.Add("@Qty", prLine.Qty);
        parameters.Add("@Price", prLine.Price);
        parameters.Add("@Notes", prLine.Notes ?? string.Empty);
        parameters.Add("@CreatedBy", prLine.CreatedBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.ExecuteScalarAsync<string>(command);
        
        return string.IsNullOrEmpty(result) ? throw new RepoException("Failed to insert PrLine") : result;
    }

    public async Task<PrLine> PrLineSelectById(int id, CancellationToken cancellationToken)
    {
        const string query = "PrLine_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@PrLineId", id);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var prLine = await _dbConnection.QuerySingleOrDefaultAsync<PrLine>(command);
        
        return prLine ?? throw new RepoException($"PrLine with Id {id} not found.");
    }

    public async Task<IEnumerable<PrLine>> PrLineSelectByPr(string prId, CancellationToken cancellationToken)
    {
        const string query = "PrLine_SelectByPr";
        var parameters = new DynamicParameters();
        parameters.Add("@PrId", prId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var prLines = await _dbConnection.QueryAsync<PrLine>(command);
        
        return prLines;
    }

    public async Task<string> PrLineUpdate(PrLine prLine, CancellationToken cancellationToken)
    {
        const string query = "PrLine_Update";
        var parameters = new DynamicParameters();

        parameters.Add("@PrLineId", prLine.PrLineId);
        parameters.Add("@ItemId",  prLine.ItemId);
        parameters.Add("@RequestDate", prLine.RequestDate);
        parameters.Add("@Qty", prLine.Qty);
        parameters.Add("@Price", prLine.Price);
        parameters.Add("@Notes", prLine.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", prLine.ModifiedBy);
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.ExecuteScalarAsync<string>(command);
        
        return string.IsNullOrEmpty(result) ? throw new RepoException($"Failed to update PrLine with ID {prLine.PrId}") : result;
    }

    public async Task<IEnumerable<PrLine>> PrLineSelectForPo(
        int poRecId, 
        int pageNumber, 
        int pageSize, 
        string? prId, 
        string? itemName, 
        DateTime? dateFrom, 
        CancellationToken cancellationToken)
    {
        const string query = "PrLine_SelectForPo";
        var parameters = new DynamicParameters();
        parameters.Add("@PoRecId", poRecId);
        parameters.Add("@PageNumber", pageNumber);
        parameters.Add("@PageSize", pageSize);
        parameters.Add("@PrId", prId);
        parameters.Add("@ItemName", itemName);
        parameters.Add("@DateFrom", dateFrom, DbType.Date);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var prLines = await _dbConnection.QueryAsync<PrLine>(command);
        
        return prLines;
    }
}