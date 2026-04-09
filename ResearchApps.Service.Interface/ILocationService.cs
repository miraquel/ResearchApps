using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface ILocationService
{
    // LocationCbo
    Task<ServiceResponse<IEnumerable<LocationVm>>> CboAsync(CboRequestVm request, CancellationToken cancellationToken);
    // LocationDelete
    Task<ServiceResponse> DeleteAsync(int locationId, CancellationToken cancellationToken);
    // LocationInsert
    Task<ServiceResponse<LocationVm>> InsertAsync(LocationVm locationVm, CancellationToken cancellationToken);
    // LocationSelect
    Task<ServiceResponse<PagedListVm<LocationVm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    // LocationSelectById
    Task<ServiceResponse<LocationVm>> SelectByIdAsync(int locationId, CancellationToken cancellationToken);
    // LocationUpdate
    Task<ServiceResponse<LocationVm>> UpdateAsync(LocationVm locationVm, CancellationToken cancellationToken);
}
