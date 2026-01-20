using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IUnitService
{
    // UnitCbo
    Task<ServiceResponse<IEnumerable<UnitVm>>> CboAsync(CboRequestVm request, CancellationToken cancellationToken);
    // UnitDelete
    Task<ServiceResponse> DeleteAsync(int unitId, CancellationToken cancellationToken);
    // UnitInsert
    Task<ServiceResponse<UnitVm>> InsertAsync(UnitVm unitVm, CancellationToken cancellationToken);
    // UnitSelect
    Task<ServiceResponse<PagedListVm<UnitVm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    // UnitSelectById
    Task<ServiceResponse<UnitVm>> SelectByIdAsync(int unitId, CancellationToken cancellationToken);
    // UnitUpdate
    Task<ServiceResponse<UnitVm>> UpdateAsync(UnitVm unitVm, CancellationToken cancellationToken);
}