using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IItemTypeService
{
    public Task<ServiceResponse<PagedListVm<ItemTypeVm>>> ItemTypeSelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    public Task<ServiceResponse<ItemTypeVm>> ItemTypeSelectByIdAsync(int itemTypeId, CancellationToken cancellationToken);
    public Task<ServiceResponse<ItemTypeVm>> ItemTypeInsertAsync(ItemTypeVm itemType, CancellationToken cancellationToken);
    public Task<ServiceResponse<ItemTypeVm>> ItemTypeUpdateAsync(ItemTypeVm itemType, CancellationToken cancellationToken);
    public Task<ServiceResponse> ItemTypeDeleteAsync(int itemTypeId, CancellationToken cancellationToken);
    public Task<ServiceResponse<IEnumerable<ItemTypeVm>>> ItemTypeCbo(CboRequestVm pagedCboRequestVm, CancellationToken cancellationToken);
}