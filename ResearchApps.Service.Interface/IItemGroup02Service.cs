using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IItemGroup02Service
{
    Task<ServiceResponse<IEnumerable<ItemGroup02Vm>>> CboAsync(CboRequestVm requestVm, CancellationToken cancellationToken);
    Task<ServiceResponse<ItemGroup02Vm>> SelectByIdAsync(int itemGroup02Id, CancellationToken cancellationToken);
    Task<ServiceResponse<PagedListVm<ItemGroup02Vm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    Task<ServiceResponse<ItemGroup02Vm>> InsertAsync(ItemGroup02Vm itemGroup02Vm, CancellationToken cancellationToken);
    Task<ServiceResponse<ItemGroup02Vm>> UpdateAsync(ItemGroup02Vm itemGroup02Vm, CancellationToken cancellationToken);
    Task<ServiceResponse> DeleteAsync(int itemGroup02Id, string modifiedBy, CancellationToken cancellationToken);
}
