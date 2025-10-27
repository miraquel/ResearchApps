using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IWarehouseService
{
    // WhCbo
    Task<ServiceResponse> CboAsync();
    // WhDelete
    Task<ServiceResponse> DeleteAsync(int whId, string modifiedBy, CancellationToken cancellationToken);
    // WhInsert
    Task<ServiceResponse> InsertAsync(WarehouseVm warehouseVm, CancellationToken cancellationToken);
    // WhSelect
    Task<ServiceResponse> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    // WhSelectById
    Task<ServiceResponse> SelectByIdAsync(int whId, CancellationToken cancellationToken);
    // WhUpdate
    Task<ServiceResponse> UpdateAsync(WarehouseVm warehouseVm, CancellationToken cancellationToken);
}