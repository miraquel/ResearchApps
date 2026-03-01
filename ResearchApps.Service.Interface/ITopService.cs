using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface ITopService
{
    Task<ServiceResponse<PagedListVm<TopVm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    Task<ServiceResponse<TopVm>> SelectByIdAsync(int topId, CancellationToken cancellationToken);
    Task<ServiceResponse<TopVm>> InsertAsync(TopVm topVm, CancellationToken cancellationToken);
    Task<ServiceResponse<TopVm>> UpdateAsync(TopVm topVm, CancellationToken cancellationToken);
    Task<ServiceResponse> DeleteAsync(int topId, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<TopVm>>> CboAsync(CboRequestVm cboRequestVm, CancellationToken cancellationToken);
}
