using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IItemService
{
    // ItemCbo
    Task<ServiceResponse<IEnumerable<ItemVm>>> CboAsync(CboRequestVm cboRequestVm, CancellationToken cancellationToken);
    // ItemDelete
    Task<ServiceResponse> DeleteAsync(int itemId, CancellationToken cancellationToken);
    // ItemInsert
    Task<ServiceResponse<ItemVm>> InsertAsync(ItemVm itemVm, CancellationToken cancellationToken);
    // ItemSelect
    Task<ServiceResponse<PagedListVm<ItemVm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    // ItemSelectById
    Task<ServiceResponse<ItemVm>> SelectByIdAsync(int itemId, CancellationToken cancellationToken);
    // ItemUpdate
    Task<ServiceResponse<ItemVm>> UpdateAsync(ItemVm itemVm, CancellationToken cancellationToken);
}