using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IBudgetService
{
    Task<ServiceResponse<PagedListVm<BudgetVm>>> BudgetSelectAsync(PagedListRequestVm request, CancellationToken cancellationToken);
    Task<ServiceResponse<BudgetVm>> BudgetSelectByIdAsync(int budgetId, CancellationToken cancellationToken);
    Task<ServiceResponse<BudgetVm>> BudgetInsertAsync(BudgetVm budgetVm, CancellationToken cancellationToken);
    Task<ServiceResponse<BudgetVm>> BudgetUpdateAsync(BudgetVm budgetVm, CancellationToken cancellationToken);
    Task<ServiceResponse> BudgetDeleteAsync(int budgetId, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<BudgetVm>>> BudgetCboAsync(CboRequestVm cboRequest, CancellationToken cancellationToken);
}