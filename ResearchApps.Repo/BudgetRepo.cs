using System.Data;
using Dapper;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class BudgetRepo : IBudgetRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public BudgetRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<Budget>> BudgetSelectAsync(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Budget_Select";
        var parameters = new DynamicParameters();

        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
            {
                switch (filter.Key)
                {
                    case "BudgetId":
                        parameters.Add("@BudgetId", strValue);
                        break;
                    case "Year":
                        if (int.TryParse(strValue, out var yearValue))
                            parameters.Add("@Year", yearValue);
                        break;
                    case "BudgetName":
                        parameters.Add("@BudgetName", strValue);
                        break;
                    case "StartDateFrom":
                        if (DateTime.TryParse(strValue, out var startDateFrom))
                            parameters.Add("@StartDateFrom", startDateFrom);
                        break;
                    case "StartDateTo":
                        if (DateTime.TryParse(strValue, out var startDateTo))
                            parameters.Add("@StartDateTo", startDateTo);
                        break;
                    case "EndDateFrom":
                        if (DateTime.TryParse(strValue, out var endDateFrom))
                            parameters.Add("@EndDateFrom", endDateFrom);
                        break;
                    case "EndDateTo":
                        if (DateTime.TryParse(strValue, out var endDateTo))
                            parameters.Add("@EndDateTo", endDateTo);
                        break;
                    case "Amount":
                        if (decimal.TryParse(strValue, out var amountValue))
                            parameters.Add("@Amount", amountValue);
                        break;
                    case "AmountOperator":
                        parameters.Add("@AmountOperator", strValue);
                        break;
                    case "RemAmount":
                        if (decimal.TryParse(strValue, out var remAmountValue))
                            parameters.Add("@RemAmount", remAmountValue);
                        break;
                    case "RemAmountOperator":
                        parameters.Add("@RemAmountOperator", strValue);
                        break;
                    case "StatusId":
                        if (int.TryParse(strValue, out var statusId))
                            parameters.Add("@StatusId", statusId);
                        break;
                    case "CreatedBy":
                        parameters.Add("@CreatedBy", strValue);
                        break;
                    case "ModifiedBy":
                        parameters.Add("@ModifiedBy", strValue);
                        break;
                    default:
                        parameters.Add($"@{filter.Key}", strValue);
                        break;
                }
            }
        }

        parameters.Add("@PageNumber", request.PageNumber);
        parameters.Add("@PageSize", request.PageSize);
        parameters.Add("@SortOrder", request.IsSortAscending ? "ASC" : "DESC");
        parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "BudgetId" : request.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<Budget>();
        var totalCount = result.ReadSingle<int>();

        return new PagedList<Budget>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<Budget?> BudgetSelectByIdAsync(int budgetId, CancellationToken cancellationToken)
    {
        const string query = "Budget_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@BudgetId", budgetId);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstOrDefaultAsync<Budget>(command);
    }

    public async Task<Budget> BudgetInsertAsync(Budget budget, CancellationToken cancellationToken)
    {
        const string query = "Budget_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@Year", budget.Year);
        parameters.Add("@BudgetName", budget.BudgetName);
        parameters.Add("@StartDate", budget.StartDate);
        parameters.Add("@EndDate", budget.EndDate);
        parameters.Add("@Amount", budget.Amount);
        parameters.Add("@StatusId", budget.StatusId);
        parameters.Add("@CreatedBy", budget.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstAsync<Budget>(command);
    }

    public async Task<Budget> BudgetUpdateAsync(Budget budget, CancellationToken cancellationToken)
    {
        const string query = "Budget_Update";
        var parameters = new DynamicParameters();
        parameters.Add("@BudgetId", budget.BudgetId);
        parameters.Add("@Year", budget.Year);
        parameters.Add("@BudgetName", budget.BudgetName);
        parameters.Add("@StartDate", budget.StartDate);
        parameters.Add("@EndDate", budget.EndDate);
        parameters.Add("@Amount", budget.Amount);
        parameters.Add("@ModifiedBy", budget.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);

        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        return await _dbConnection.QueryFirstAsync<Budget>(command);
    }

    public async Task BudgetDeleteAsync(int budgetId, string modifiedBy, CancellationToken cancellationToken)
    {
        const string query = "Budget_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@BudgetId", budgetId);
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

    public async Task<IEnumerable<Budget>> BudgetCboAsync(CboRequest cboRequest, CancellationToken cancellationToken)
    {
        const string query = "BudgetCbo";
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        var parameters = new DynamicParameters();
            
        if (cboRequest.Id > 0)
        {
            parameters.Add("@Id", cboRequest.Id);
        }
            
        if (!string.IsNullOrEmpty(cboRequest.Term))
        {
            parameters.Add("@Term", cboRequest.Term);
        }
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<Budget>(command);
    }
}