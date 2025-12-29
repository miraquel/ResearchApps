using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepo _budgetRepo;
    private readonly MapperlyMapper _mapper = new();

    public BudgetService(IBudgetRepo budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }

    public async Task<ServiceResponse> BudgetCboAsync(CboRequestVm cboRequest, CancellationToken cancellationToken)
    {
        var budgets = await _budgetRepo.BudgetCboAsync(_mapper.MapToEntity(cboRequest), cancellationToken);
        return ServiceResponse<IEnumerable<BudgetVm>>.Success(_mapper.MapToVm(budgets),"Budgets for combo box retrieved successfully.");
    }
}