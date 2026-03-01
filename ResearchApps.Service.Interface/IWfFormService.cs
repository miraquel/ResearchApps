using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IWfFormService
{
    Task<ServiceResponse<PagedListVm<WfFormVm>>> WfFormSelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    Task<ServiceResponse<WfFormVm>> WfFormSelectByIdAsync(int wfFormId, CancellationToken cancellationToken);
    Task<ServiceResponse<WfFormVm>> WfFormInsertAsync(WfFormVm wfForm, CancellationToken cancellationToken);
    Task<ServiceResponse<WfFormVm>> WfFormUpdateAsync(WfFormVm wfForm, CancellationToken cancellationToken);
    Task<ServiceResponse> WfFormDeleteAsync(int wfFormId, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<WfFormVm>>> WfFormCboAsync(CboRequestVm cboRequest, CancellationToken cancellationToken);
    Task<ServiceResponse<WorkflowConfigVm>> GetWorkflowConfigAsync(int wfFormId, CancellationToken cancellationToken);
}
