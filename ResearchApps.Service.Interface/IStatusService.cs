using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface
{
    public interface IStatusService
    {
        Task<ServiceResponse> StatusCboAsync(CboRequestVm listRequest, CancellationToken cancellationToken);
    }
}
