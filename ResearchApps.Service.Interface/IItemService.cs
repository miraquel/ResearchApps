using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IItemService
{
    // ItemCbo
    Task<ServiceResponse> CboAsync(CboRequestVm cboRequestVm, CancellationToken cancellationToken);
    // ItemDelete
    Task<ServiceResponse> DeleteAsync(int itemId, CancellationToken cancellationToken);
    // ItemInsert
    Task<ServiceResponse> InsertAsync(ItemVm itemVm, CancellationToken cancellationToken);
    // ItemSelect
    Task<ServiceResponse> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    // ItemSelectById
    Task<ServiceResponse> SelectByIdAsync(int itemId, CancellationToken cancellationToken);
    // ItemUpdate
    Task<ServiceResponse> UpdateAsync(ItemVm itemVm, CancellationToken cancellationToken);
}