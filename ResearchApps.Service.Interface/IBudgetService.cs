using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IBudgetService
{
    Task<ServiceResponse<IEnumerable<BudgetVm>>> BudgetCboAsync(CboRequestVm cboRequest, CancellationToken cancellationToken);
}