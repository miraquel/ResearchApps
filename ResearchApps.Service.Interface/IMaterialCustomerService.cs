using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IMaterialCustomerService
{
    // Mc_Select
    Task<ServiceResponse<PagedListVm<MaterialCustomerHeaderVm>>> McSelect(PagedListRequestVm request, CancellationToken cancellationToken);
    
    // Mc_SelectById
    Task<ServiceResponse<MaterialCustomerHeaderVm>> McSelectById(int id, CancellationToken cancellationToken);
    
    // Mc_Insert
    Task<ServiceResponse<(int RecId, string McId)>> McInsert(MaterialCustomerVm materialCustomer, CancellationToken cancellationToken);
    
    // Mc_Update
    Task<ServiceResponse> McUpdate(MaterialCustomerHeaderVm materialCustomerHeader, CancellationToken cancellationToken);
    
    // Mc_Delete
    Task<ServiceResponse> McDelete(int recId, CancellationToken cancellationToken);
    
    // Lines
    Task<ServiceResponse<IEnumerable<MaterialCustomerLineVm>>> McLineSelectByMc(int mcRecId, CancellationToken cancellationToken);
    Task<ServiceResponse<MaterialCustomerLineVm>> McLineSelectById(int mcLineId, CancellationToken cancellationToken);
    Task<ServiceResponse<string>> McLineInsert(MaterialCustomerLineVm materialCustomerLine, CancellationToken cancellationToken);
    Task<ServiceResponse> McLineDelete(int mcLineId, CancellationToken cancellationToken);

    // Composite ViewModel
    Task<ServiceResponse<MaterialCustomerVm>> GetMaterialCustomer(int recId, CancellationToken cancellationToken);
}
