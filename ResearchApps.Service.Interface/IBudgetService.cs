using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IBudgetService
{
    Task<ServiceResponse> BudgetCboAsync(CboRequestVm cboRequest, CancellationToken cancellationToken);
}