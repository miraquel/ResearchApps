using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class BudgetService : IBudgetService
{
    private readonly IBudgetRepo _budgetRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<BudgetService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public BudgetService(IBudgetRepo budgetRepo, IDbTransaction dbTransaction,
        UserClaimDto userClaimDto, ILogger<BudgetService> logger)
    {
        _budgetRepo = budgetRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PagedListVm<BudgetVm>>> BudgetSelectAsync(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogRetrievingBudgetListPagePageSize(request.PageNumber, request.PageSize);
        var budgets = await _budgetRepo.BudgetSelectAsync(_mapper.MapToEntity(request), cancellationToken);
        LogRetrievedCountBudgets(budgets.TotalCount);
        return ServiceResponse<PagedListVm<BudgetVm>>.Success(_mapper.MapToVm(budgets), "Budgets retrieved successfully.");
    }

    public async Task<ServiceResponse<BudgetVm>> BudgetSelectByIdAsync(int budgetId, CancellationToken cancellationToken)
    {
        LogRetrievingBudgetById(budgetId);
        var budget = await _budgetRepo.BudgetSelectByIdAsync(budgetId, cancellationToken);

        if (budget == null)
        {
            LogBudgetNotFound(budgetId);
            return ServiceResponse<BudgetVm>.Failure("Budget not found.", StatusCodes.Status404NotFound);
        }

        return ServiceResponse<BudgetVm>.Success(_mapper.MapToVm(budget), "Budget retrieved successfully.");
    }

    public async Task<ServiceResponse<BudgetVm>> BudgetInsertAsync(BudgetVm budgetVm, CancellationToken cancellationToken)
    {
        LogCreatingNewBudgetByUser(budgetVm.BudgetName, _userClaimDto.Username);

        var entity = _mapper.MapToEntity(budgetVm);
        entity.Year = budgetVm.StartDate.Year;
        entity.StatusId = 1;
        entity.CreatedBy = _userClaimDto.Username;

        var result = await _budgetRepo.BudgetInsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();

        LogBudgetCreatedSuccessfully(result.BudgetId);
        return ServiceResponse<BudgetVm>.Success(_mapper.MapToVm(result), "Budget created successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<BudgetVm>> BudgetUpdateAsync(BudgetVm budgetVm, CancellationToken cancellationToken)
    {
        LogUpdatingBudgetByUser(budgetVm.BudgetId, budgetVm.BudgetName, _userClaimDto.Username);

        var entity = _mapper.MapToEntity(budgetVm);
        entity.Year = budgetVm.StartDate.Year;
        entity.ModifiedBy = _userClaimDto.Username;

        var result = await _budgetRepo.BudgetUpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();

        LogBudgetUpdatedSuccessfully(budgetVm.BudgetId);
        return ServiceResponse<BudgetVm>.Success(_mapper.MapToVm(result), "Budget updated successfully.");
    }

    public async Task<ServiceResponse> BudgetDeleteAsync(int budgetId, CancellationToken cancellationToken)
    {
        LogDeletingBudgetByUser(budgetId, _userClaimDto.Username);
        await _budgetRepo.BudgetDeleteAsync(budgetId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogBudgetDeletedSuccessfully(budgetId);
        return ServiceResponse.Success("Budget deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<BudgetVm>>> BudgetCboAsync(CboRequestVm cboRequest, CancellationToken cancellationToken)
    {
        LogRetrievingBudgets();
        var budgets = await _budgetRepo.BudgetCboAsync(_mapper.MapToEntity(cboRequest), cancellationToken);
        LogBudgetsRetrieved(budgets.Count());
        return ServiceResponse<IEnumerable<BudgetVm>>.Success(_mapper.MapToVm(budgets), "Budgets for combo box retrieved successfully.");
    }

    [LoggerMessage(LogLevel.Debug, "Retrieving budgets for combo box")]
    partial void LogRetrievingBudgets();

    [LoggerMessage(LogLevel.Debug, "Retrieved {count} budgets")]
    partial void LogBudgetsRetrieved(int count);

    [LoggerMessage(LogLevel.Information, "Retrieving budget list - Page: {PageNumber}, PageSize: {PageSize}")]
    partial void LogRetrievingBudgetListPagePageSize(int pageNumber, int pageSize);

    [LoggerMessage(LogLevel.Information, "Retrieved {Count} budgets")]
    partial void LogRetrievedCountBudgets(int count);

    [LoggerMessage(LogLevel.Information, "Retrieving budget by ID: {BudgetId}")]
    partial void LogRetrievingBudgetById(int budgetId);

    [LoggerMessage(LogLevel.Warning, "Budget not found: {BudgetId}")]
    partial void LogBudgetNotFound(int budgetId);

    [LoggerMessage(LogLevel.Information, "Creating new budget '{BudgetName}' by user '{Username}'")]
    partial void LogCreatingNewBudgetByUser(string budgetName, string username);

    [LoggerMessage(LogLevel.Information, "Budget created successfully with ID: {BudgetId}")]
    partial void LogBudgetCreatedSuccessfully(int budgetId);

    [LoggerMessage(LogLevel.Information, "Updating budget ID {BudgetId} ('{BudgetName}') by user '{Username}'")]
    partial void LogUpdatingBudgetByUser(int budgetId, string budgetName, string username);

    [LoggerMessage(LogLevel.Information, "Budget updated successfully: {BudgetId}")]
    partial void LogBudgetUpdatedSuccessfully(int budgetId);

    [LoggerMessage(LogLevel.Information, "Deleting budget ID {BudgetId} by user '{Username}'")]
    partial void LogDeletingBudgetByUser(int budgetId, string username);

    [LoggerMessage(LogLevel.Information, "Budget deleted successfully: {BudgetId}")]
    partial void LogBudgetDeletedSuccessfully(int budgetId);
}