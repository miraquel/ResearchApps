using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IItemGroup01Service
{
    Task<ServiceResponse<IEnumerable<ItemGroup01Vm>>> CboAsync(CboRequestVm requestVm, CancellationToken cancellationToken);
    Task<ServiceResponse<ItemGroup01Vm>> SelectByIdAsync(int itemGroup01Id, CancellationToken cancellationToken);
    Task<ServiceResponse<PagedListVm<ItemGroup01Vm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    Task<ServiceResponse<ItemGroup01Vm>> InsertAsync(ItemGroup01Vm itemGroup01Vm, CancellationToken cancellationToken);
    Task<ServiceResponse<ItemGroup01Vm>> UpdateAsync(ItemGroup01Vm itemGroup01Vm, CancellationToken cancellationToken);
    Task<ServiceResponse> DeleteAsync(int itemGroup01Id, string modifiedBy, CancellationToken cancellationToken);
}
