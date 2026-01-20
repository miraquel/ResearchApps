using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface ICustomerService
{
    // Customer_Select
    Task<ServiceResponse<PagedListVm<CustomerVm>>> CustomerSelect(PagedListRequestVm request, CancellationToken cancellationToken);
    
    // Customer_SelectById
    Task<ServiceResponse<CustomerVm>> CustomerSelectById(int id, CancellationToken cancellationToken);
    
    // Customer_Insert
    Task<ServiceResponse<int>> CustomerInsert(CustomerVm customer, CancellationToken cancellationToken);
    
    // Customer_Update
    Task<ServiceResponse> CustomerUpdate(CustomerVm customer, CancellationToken cancellationToken);
    
    // Customer_Delete
    Task<ServiceResponse> CustomerDelete(int id, CancellationToken cancellationToken);
    
    // Customer_Cbo - For dropdown
    Task<ServiceResponse<IEnumerable<CustomerVm>>> CustomerCbo(CboRequestVm request, CancellationToken cancellationToken);
}
