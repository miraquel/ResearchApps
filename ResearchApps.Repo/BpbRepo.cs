using System.Data;
using Dapper;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class BpbRepo : IBpbRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public BpbRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<BpbHeader>> BpbSelect(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Bpb_Select";
        var parameters = BuildSelectParameters(request, request.PageNumber, request.PageSize, "BpbId");
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            transaction: _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<BpbHeader>();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<BpbHeader>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<IEnumerable<BpbHeader>> BpbSelectForExport(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Bpb_Select";
        var parameters = BuildSelectParameters(request, 1, int.MaxValue, "BpbId");
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            transaction: _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<BpbHeader>();
        return items;
    }

    public async Task<BpbHeader?> BpbSelectById(int id, CancellationToken cancellationToken)
    {
        const string query = "Bpb_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", id);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<BpbHeader>(command);
    }

    public async Task<IEnumerable<BpbHeader>> BpbSelectByProd(string prodId, CancellationToken cancellationToken)
    {
        const string query = "Bpb_Select";
        var parameters = new DynamicParameters();
        parameters.Add("@RefIdExact", prodId);
        parameters.Add("@PageNumber", 1);
        parameters.Add("@PageSize", int.MaxValue);
        parameters.Add("@SortOrder", "DESC");
        parameters.Add("@SortColumn", "BpbId");
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            transaction: _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<BpbHeader>();
        return items;
    }

    public async Task<int> BpbInsert(BpbHeader bpb, CancellationToken cancellationToken)
    {
        const string query = "Bpb_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@BpbDate", bpb.BpbDate);
        parameters.Add("@Descr", bpb.Descr ?? string.Empty);
        parameters.Add("@RefId", bpb.RefId ?? string.Empty);
        parameters.Add("@Notes", bpb.Notes ?? string.Empty);
        parameters.Add("@BpbStatusId", bpb.BpbStatusId);
        parameters.Add("@CreatedBy", bpb.CreatedBy);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.ExecuteScalarAsync<int>(command);
    }

    public async Task BpbUpdate(BpbHeader bpb, CancellationToken cancellationToken)
    {
        const string query = "Bpb_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", bpb.RecId);
        parameters.Add("@Notes", bpb.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", bpb.ModifiedBy);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task BpbDelete(int recId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Bpb_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", recId);
        parameters.Add("@ModifiedBy", modifiedBy);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task<IEnumerable<BpbLine>> BpbLineSelectByBpb(int bpbRecId, CancellationToken cancellationToken)
    {
        const string query = "BpbLine_SelectByBpb";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", bpbRecId);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryAsync<BpbLine>(command);
    }

    public async Task<BpbLine?> BpbLineSelectById(int bpbLineId, CancellationToken cancellationToken)
    {
        const string query = "BpbLine_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@BpbLineId", bpbLineId);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<BpbLine>(command);
    }

    public async Task<int> BpbLineInsert(BpbLine bpbLine, CancellationToken cancellationToken)
    {
        const string query = "BpbLine_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@RecId", bpbLine.BpbRecId); // Header RecId
        parameters.Add("@ItemId", bpbLine.ItemId);
        parameters.Add("@WhId", bpbLine.WhId);
        parameters.Add("@Qty", bpbLine.Qty);
        parameters.Add("@ProdId", bpbLine.ProdId ?? string.Empty);
        parameters.Add("@Notes", bpbLine.Notes ?? string.Empty);
        parameters.Add("@CreatedBy", bpbLine.CreatedBy);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.ExecuteScalarAsync<int>(command);
    }

    public async Task BpbLineUpdate(BpbLine bpbLine, CancellationToken cancellationToken)
    {
        const string query = "BpbLine_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@BpbLineId", bpbLine.BpbLineId);
        parameters.Add("@ItemId", bpbLine.ItemId);
        parameters.Add("@WhId", bpbLine.WhId);
        parameters.Add("@Qty", bpbLine.Qty);
        parameters.Add("@Notes", bpbLine.Notes ?? string.Empty);
        parameters.Add("@ModifiedBy", bpbLine.ModifiedBy);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task BpbLineDelete(int bpbLineId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "BpbLine_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@BpbLineId", bpbLineId);
        parameters.Add("@ModifiedBy", modifiedBy);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        await _dbConnection.ExecuteAsync(command);
    }

    public async Task<(decimal OnHand, decimal BufferStock)> GetStockInfo(int itemId, int whId, CancellationToken cancellationToken)
    {
        const string query = """
                             SELECT ISNULL(a.Qty, 0) as OnHand, ISNULL(i.BufferStock, 0) as BufferStock
                             FROM Item i
                             LEFT JOIN InventSum a ON a.ItemId = i.ItemId AND a.WhId = @WhId
                             WHERE i.ItemId = @ItemId
                             """;

        var parameters = new { ItemId = itemId, WhId = whId };

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken);

        var result = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(command);
        
        return result == null ? (0, 0) : ((decimal)result.OnHand, (decimal)result.BufferStock);
    }

    private static DynamicParameters BuildSelectParameters(PagedListRequest request, int pageNumber, int pageSize, string defaultSortColumn)
    {
        var parameters = new DynamicParameters();

        foreach (var filter in request.Filters)
        {
            if (string.IsNullOrWhiteSpace(filter.Value))
            {
                continue;
            }

            switch (filter.Key)
            {
                case "BpbDate" or "BpbDateFrom" or "BpbDateTo" or "CreatedDate" or "ModifiedDate":
                {
                    if (DateTime.TryParse(filter.Value, out var dateValue))
                    {
                        parameters.Add($"@{filter.Key}", dateValue);
                    }

                    break;
                }
                case "Amount":
                {
                    if (decimal.TryParse(filter.Value, out var decimalValue))
                    {
                        parameters.Add($"@{filter.Key}", decimalValue);
                    }

                    break;
                }
                case "AmountOperator":
                    parameters.Add("@AmountOperator", filter.Value);
                    break;
                case "BpbStatusId" or "RecId":
                {
                    if (int.TryParse(filter.Value, out var intValue))
                    {
                        parameters.Add($"@{filter.Key}", intValue);
                    }

                    break;
                }
                default:
                    parameters.Add($"@{filter.Key}", filter.Value);
                    break;
            }
        }

        parameters.Add("@PageNumber", pageNumber);
        parameters.Add("@PageSize", pageSize);
        parameters.Add("@SortOrder", request.IsSortAscending ? "ASC" : "DESC");
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? defaultSortColumn : request.SortBy);

        return parameters;
    }
}
