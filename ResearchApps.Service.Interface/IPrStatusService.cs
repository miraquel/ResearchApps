using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IPrStatusService
{
    Task<ServiceResponse> PrStatusCboAsync(CboRequestVm request, CancellationToken cancellationToken);
}