using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IItemDeptService
{
    Task<ServiceResponse<IEnumerable<ItemDeptVm>>> CboAsync(CboRequestVm requestVm, CancellationToken cancellationToken);
    Task<ServiceResponse<ItemDeptVm>> SelectByIdAsync(int itemDeptId, CancellationToken cancellationToken);
    Task<ServiceResponse<PagedListVm<ItemDeptVm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    Task<ServiceResponse<ItemDeptVm>> InsertAsync(ItemDeptVm itemDeptVm, CancellationToken cancellationToken);
    Task<ServiceResponse<ItemDeptVm>> UpdateAsync(ItemDeptVm itemDeptVm, CancellationToken cancellationToken);
    Task<ServiceResponse> DeleteAsync(int itemDeptId, string modifiedBy, CancellationToken cancellationToken);
}
