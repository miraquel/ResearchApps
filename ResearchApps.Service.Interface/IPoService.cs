using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IPoService
{
    // CRUD Operations
    Task<ServiceResponse<PagedListVm<PoHeaderVm>>> PoSelect(PagedListRequestVm request, CancellationToken ct);
    Task<ServiceResponse<PoVm>> PoSelectById(int recId, CancellationToken ct);
    Task<ServiceResponse<PoVm>> PoInsert(PoHeaderVm poHeaderVm, CancellationToken ct);
    Task<ServiceResponse<PoVm>> PoUpdate(PoHeaderVm poHeaderVm, CancellationToken ct);
    Task<ServiceResponse> PoDelete(int recId, CancellationToken ct);

    // Workflow Operations
    Task<ServiceResponse<PoVm>> PoSubmitById(int recId, CancellationToken ct);
    Task<ServiceResponse> PoRecallById(int recId, CancellationToken ct);
    Task<ServiceResponse> PoApproveById(PoWorkflowActionVm action, CancellationToken ct);
    Task<ServiceResponse> PoRejectById(PoWorkflowActionVm action, CancellationToken ct);
    Task<ServiceResponse> PoCloseById(int recId, CancellationToken ct);

    // Outstanding Operations
    Task<ServiceResponse<IEnumerable<PoHeaderOutstandingVm>>> PoOsSelect(int supplierId, CancellationToken ct);
    Task<ServiceResponse<IEnumerable<PoLineOutstandingVm>>> PoOsSelectById(int recId, CancellationToken ct);
}
