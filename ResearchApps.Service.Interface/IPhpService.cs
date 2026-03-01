using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IPhpService
{
    // Php_Select - Get list of all Php records with pagination
    Task<ServiceResponse<PagedListVm<PhpHeaderVm>>> PhpSelect(PagedListRequestVm request, CancellationToken cancellationToken);
    
    // Php_SelectById
    Task<ServiceResponse<PhpHeaderVm>> PhpSelectById(int recId, CancellationToken cancellationToken);
    
    // Php_Insert
    Task<ServiceResponse<(int RecId, string PhpId)>> PhpInsert(PhpHeaderVm phpHeader, CancellationToken cancellationToken);
    
    // Php_Update
    Task<ServiceResponse> PhpUpdate(PhpHeaderVm phpHeader, CancellationToken cancellationToken);
    
    // Php_Delete
    Task<ServiceResponse> PhpDelete(int recId, CancellationToken cancellationToken);
    
    // Lines
    Task<ServiceResponse<IEnumerable<PhpLineVm>>> PhpLineSelectByPhp(int phpRecId, CancellationToken cancellationToken);
    Task<ServiceResponse<PhpLineVm>> PhpLineSelectById(int phpLineId, CancellationToken cancellationToken);
    Task<ServiceResponse> PhpLineInsert(PhpLineVm phpLine, CancellationToken cancellationToken);
    Task<ServiceResponse> PhpLineUpdate(PhpLineVm phpLine, CancellationToken cancellationToken);
    Task<ServiceResponse> PhpLineDelete(int phpLineId, CancellationToken cancellationToken);

    // Composite ViewModel
    Task<ServiceResponse<PhpVm>> GetPhp(int recId, CancellationToken cancellationToken);
}
