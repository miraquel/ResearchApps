using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface ISalesPriceService
{
    Task<ServiceResponse<PagedListVm<SalesPriceVm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    Task<ServiceResponse<SalesPriceVm>> SelectByIdAsync(int recId, CancellationToken cancellationToken);
    Task<ServiceResponse<SalesPriceVm>> InsertAsync(SalesPriceVm salesPriceVm, CancellationToken cancellationToken);
    Task<ServiceResponse<SalesPriceVm>> UpdateAsync(SalesPriceVm salesPriceVm, CancellationToken cancellationToken);
    Task<ServiceResponse> DeleteAsync(int recId, CancellationToken cancellationToken);
}
