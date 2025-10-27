using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IItemTypeService
{
    public Task<ServiceResponse> ItemTypeSelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    public Task<ServiceResponse> ItemTypeSelectByIdAsync(int itemTypeId, CancellationToken cancellationToken);
    public Task<ServiceResponse> ItemTypeInsertAsync(ItemTypeVm itemType, CancellationToken cancellationToken);
    public Task<ServiceResponse> ItemTypeUpdateAsync(ItemTypeVm itemType, CancellationToken cancellationToken);
    public Task<ServiceResponse> ItemTypeDeleteAsync(int itemTypeId, CancellationToken cancellationToken);
    public Task<ServiceResponse> ItemTypeCbo(CboRequestVm pagedCboRequestVm, CancellationToken cancellationToken);
}