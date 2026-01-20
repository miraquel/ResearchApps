using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface
{
    public interface IStatusService
    {
        Task<ServiceResponse<IEnumerable<StatusVm>>> StatusCboAsync(CboRequestVm listRequest, CancellationToken cancellationToken);
    }
}
