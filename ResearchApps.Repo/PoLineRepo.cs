using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class PoLineRepo : IPoLineRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public PoLineRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PoLine?> PoLineSelectById(int poLineId, CancellationToken ct)
    {
        const string query = "PoLine_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@PoLineId", poLineId);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<PoLine>(command);
    }

    public async Task<IEnumerable<PoLine>> PoLineSelectByPo(int recId, CancellationToken ct)
    {
        const string query = "PoLine_SelectByPo";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<PoLine>(command);
    }

    public async Task<string> PoLineInsert(PoLine poLine, CancellationToken ct)
    {
        const string query = "PoLine_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", poLine.RecId);
        parameters.Add("@PrLineId", poLine.PrLineId);
        parameters.Add("@ItemId", poLine.ItemId);
        parameters.Add("@DeliveryDate", poLine.DeliveryDate, DbType.Date);
        parameters.Add("@Qty", poLine.Qty);
        parameters.Add("@Price", poLine.Price);
        parameters.Add("@Notes", poLine.Notes);
        parameters.Add("@CreatedBy", poLine.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryFirstOrDefaultAsync<PoLine>(command);
        return result?.PoId ?? throw new RepoException<PoLine>("Failed to insert PO line", poLine);
    }

    public async Task PoLineUpdate(PoLine poLine, CancellationToken ct)
    {
        const string query = "PoLine_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@PoLineId", poLine.PoLineId);
        parameters.Add("@ItemId", poLine.ItemId);
        parameters.Add("@DeliveryDate", poLine.DeliveryDate, DbType.Date);
        parameters.Add("@Qty", poLine.Qty);
        parameters.Add("@Price", poLine.Price);
        parameters.Add("@Notes", poLine.Notes);
        parameters.Add("@ModifiedBy", poLine.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task PoLineDelete(int poLineId, CancellationToken ct)
    {
        const string query = "PoLine_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@PoLineId", poLineId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query, parameters, _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }
}
