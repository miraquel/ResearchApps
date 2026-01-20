using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IDashboardService
{
    Task<ServiceResponse<DashboardVm>> GetDashboardData(string userId, CancellationToken cancellationToken);
}
