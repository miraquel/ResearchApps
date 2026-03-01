using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IProdService
{
    Task<ServiceResponse<PagedListVm<ProdVm>>> SelectAsync(PagedListRequestVm request, CancellationToken ct);
    Task<ServiceResponse<ProdVm>> SelectByIdAsync(int recId, CancellationToken ct);
    Task<ServiceResponse<ProdVm>> SelectByProdIdAsync(string prodId, CancellationToken ct);
    Task<ServiceResponse<ProdVm>> InsertAsync(ProdVm prodVm, CancellationToken ct);
    Task<ServiceResponse<ProdVm>> UpdateAsync(ProdVm prodVm, CancellationToken ct);
    Task<ServiceResponse> DeleteAsync(int recId, CancellationToken ct);
    Task<ServiceResponse<IEnumerable<ProdStatusVm>>> ProdStatusCboAsync(CancellationToken ct);
}
