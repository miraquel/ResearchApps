using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IUnitService
{
    // UnitCbo
    Task<ServiceResponse> CboAsync(CboRequestVm request, CancellationToken cancellationToken);
    // UnitDelete
    Task<ServiceResponse> DeleteAsync(int unitId, CancellationToken cancellationToken);
    // UnitInsert
    Task<ServiceResponse> InsertAsync(UnitVm unitVm, CancellationToken cancellationToken);
    // UnitSelect
    Task<ServiceResponse> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    // UnitSelectById
    Task<ServiceResponse> SelectByIdAsync(int unitId, CancellationToken cancellationToken);
    // UnitUpdate
    Task<ServiceResponse> UpdateAsync(UnitVm unitVm, CancellationToken cancellationToken);
}