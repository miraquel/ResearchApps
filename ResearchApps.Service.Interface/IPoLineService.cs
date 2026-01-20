using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IPoLineService
{
    Task<ServiceResponse<PoLineVm>> PoLineSelectById(int poLineId, CancellationToken ct);
    Task<ServiceResponse<IEnumerable<PoLineVm>>> PoLineSelectByPo(int recId, CancellationToken ct);
    Task<ServiceResponse<string>> PoLineInsert(PoLineVm poLineVm, CancellationToken ct);
    Task<ServiceResponse<PoLineVm>> PoLineUpdate(PoLineVm poLineVm, CancellationToken ct);
    Task<ServiceResponse<string>> PoLineDelete(int poLineId, CancellationToken ct);
}
