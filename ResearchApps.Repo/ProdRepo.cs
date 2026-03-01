using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class ProdRepo : IProdRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public ProdRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<Prod>> SelectAsync(PagedListRequest request, CancellationToken ct)
    {
        const string query = "Prod_Select";
        var parameters = new DynamicParameters();
        parameters.Add("@PageNumber", request.PageNumber);
        parameters.Add("@PageSize", request.PageSize);
        
        // Add filters if provided
        foreach (var filter in request.Filters)
        {
            if (filter.Key.Equals("ProdStatusId", StringComparison.OrdinalIgnoreCase) && 
                int.TryParse(filter.Value, out var statusId))
            {
                parameters.Add($"@{filter.Key}", statusId);
            }
            else if (!string.IsNullOrEmpty(filter.Value))
            {
                parameters.Add($"@{filter.Key}", $"%{filter.Value}%");
            }
        }

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        using var multi = await _dbConnection.QueryMultipleAsync(command);
        var items = (await multi.ReadAsync<Prod>()).ToList();
        var totalCount = await multi.ReadSingleAsync<int>();

        return new PagedList<Prod>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<Prod> SelectByIdAsync(int recId, CancellationToken ct)
    {
        const string query = "Prod_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Prod>(command)
               ?? throw new RepoException($"Production with RecId {recId} not found.");
    }

    public async Task<Prod> SelectByProdIdAsync(string prodId, CancellationToken ct)
    {
        const string query = "Prod_SelectByProdId";
        var parameters = new DynamicParameters();
        parameters.Add("@ProdId", prodId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Prod>(command)
               ?? throw new RepoException($"Production with ProdId {prodId} not found.");
    }

    public async Task<int> InsertAsync(Prod prod, CancellationToken ct)
    {
        const string query = "Prod_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@ProdDate", prod.ProdDate);
        parameters.Add("@CustomerId", prod.CustomerId);
        parameters.Add("@ItemId", prod.ItemId);
        parameters.Add("@PlanQty", prod.PlanQty);
        parameters.Add("@Notes", prod.Notes ?? string.Empty);
        parameters.Add("@ProdStatusId", prod.ProdStatusId);
        parameters.Add("@CreatedBy", prod.CreatedBy);
        parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
        
        var recId = parameters.Get<int>("@ReturnValue");
        
        if (recId == 0)
            throw new RepoException<Prod>("Failed to insert production record", prod);

        return recId;
    }

    public async Task UpdateAsync(Prod prod, CancellationToken ct)
    {
        const string query = "Prod_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", prod.RecId);
        parameters.Add("@ProdDate", prod.ProdDate);
        parameters.Add("@CustomerId", prod.CustomerId);
        parameters.Add("@ItemId", prod.ItemId);
        parameters.Add("@PlanQty", prod.PlanQty);
        parameters.Add("@Notes", prod.Notes ?? string.Empty);
        parameters.Add("@ProdStatusId", prod.ProdStatusId);
        parameters.Add("@ModifiedBy", prod.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task DeleteAsync(int recId, CancellationToken ct)
    {
        const string query = "Prod_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: ct,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task<IEnumerable<ProdStatus>> ProdStatusCboAsync(CancellationToken ct)
    {
        const string query = "SELECT ProdStatusId, ProdStatusName FROM ProdStatus ORDER BY ProdStatusId";

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            transaction: _dbTransaction,
            cancellationToken: ct);

        return await _dbConnection.QueryAsync<ProdStatus>(command);
    }
}
