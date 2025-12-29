using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IPrService
{
    // Pr_Delete
    Task<ServiceResponse> PrDelete(int id, CancellationToken cancellationToken);
    // Pr_Insert
    Task<ServiceResponse<int>> PrInsert(PrVm pr, CancellationToken cancellationToken);
    // Pr_Select
    Task<ServiceResponse<PagedListVm<PrVm>>> PrSelect(PagedListRequestVm request, CancellationToken cancellationToken);
    // Pr_SelectById
    Task<ServiceResponse<PrVm>> PrSelectById(int id, CancellationToken cancellationToken);
    // Pr_Update
    Task<ServiceResponse> PrUpdate(PrVm pr, CancellationToken cancellationToken);
    // Pr_SubmitById - Submit PR and start workflow
    Task<ServiceResponse<PrVm>> PrSubmitById(int id, CancellationToken cancellationToken);
    // Pr_ApproveById - Approve PR
    Task<ServiceResponse> PrApproveById(PrWorkflowActionVm action, CancellationToken cancellationToken);
    // Pr_RejectById - Reject PR
    Task<ServiceResponse> PrRejectById(PrWorkflowActionVm action, CancellationToken cancellationToken);
    // Pr_RecallById - Recall PR
    Task<ServiceResponse> PrRecallById(int id, CancellationToken cancellationToken);
}