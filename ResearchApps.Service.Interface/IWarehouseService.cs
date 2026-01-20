using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IWarehouseService
{
    // WhCbo
    Task<ServiceResponse<IEnumerable<WarehouseVm>>> CboAsync();
    // WhDelete
    Task<ServiceResponse> DeleteAsync(int whId, string modifiedBy, CancellationToken cancellationToken);
    // WhInsert
    Task<ServiceResponse<WarehouseVm>> InsertAsync(WarehouseVm warehouseVm, CancellationToken cancellationToken);
    // WhSelect
    Task<ServiceResponse<PagedListVm<WarehouseVm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    // WhSelectById
    Task<ServiceResponse<WarehouseVm>> SelectByIdAsync(int whId, CancellationToken cancellationToken);
    // WhUpdate
    Task<ServiceResponse<WarehouseVm>> UpdateAsync(WarehouseVm warehouseVm, CancellationToken cancellationToken);
}