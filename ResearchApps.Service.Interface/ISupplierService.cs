using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface ISupplierService
{
    Task<ServiceResponse<IEnumerable<SupplierVm>>> SupplierSelect(CancellationToken cancellationToken);
    Task<ServiceResponse<PagedListVm<SupplierVm>>> SupplierSelect(PagedListRequestVm request, CancellationToken cancellationToken);
    Task<ServiceResponse<SupplierVm>> SupplierSelectById(int supplierId, CancellationToken cancellationToken);
    Task<ServiceResponse<SupplierVm>> SupplierInsert(SupplierVm supplierVm, CancellationToken cancellationToken);
    Task<ServiceResponse<SupplierVm>> SupplierUpdate(SupplierVm supplierVm, CancellationToken cancellationToken);
    Task<ServiceResponse> SupplierDelete(int supplierId, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<SupplierVm>>> SupplierCbo(CancellationToken cancellationToken);
}
