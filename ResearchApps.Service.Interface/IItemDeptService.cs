using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IItemDeptService
{
    Task<ServiceResponse> CboAsync(CboRequestVm requestVm, CancellationToken cancellationToken);
    Task<ServiceResponse> SelectByIdAsync(int itemDeptId, CancellationToken cancellationToken);
    Task<ServiceResponse> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    Task<ServiceResponse> InsertAsync(ItemDeptVm itemDeptVm, CancellationToken cancellationToken);
    Task<ServiceResponse> UpdateAsync(ItemDeptVm itemDeptVm, CancellationToken cancellationToken);
    Task<ServiceResponse> DeleteAsync(int itemDeptId, string modifiedBy, CancellationToken cancellationToken);
}
