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
    private readonly ILogger<BudgetService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public BudgetService(IBudgetRepo budgetRepo, ILogger<BudgetService> logger)
    {
        _budgetRepo = budgetRepo;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<BudgetVm>>> BudgetCboAsync(CboRequestVm cboRequest, CancellationToken cancellationToken)
    {
        LogRetrievingBudgets();
        var budgets = await _budgetRepo.BudgetCboAsync(_mapper.MapToEntity(cboRequest), cancellationToken);
        LogBudgetsRetrieved(budgets.Count());
        return ServiceResponse<IEnumerable<BudgetVm>>.Success(_mapper.MapToVm(budgets),"Budgets for combo box retrieved successfully.");
    }

    [LoggerMessage(LogLevel.Debug, "Retrieving budgets for combo box")]
    partial void LogRetrievingBudgets();

    [LoggerMessage(LogLevel.Debug, "Retrieved {count} budgets")]
    partial void LogBudgetsRetrieved(int count);
}