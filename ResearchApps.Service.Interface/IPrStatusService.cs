using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IPrStatusService
{
    Task<ServiceResponse<IEnumerable<PrStatusVm>>> PrStatusCboAsync(CboRequestVm request, CancellationToken cancellationToken);
}