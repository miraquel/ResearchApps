using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IWfService
{
    Task<ServiceResponse<IEnumerable<WfVm>>> WfSelectByWfFormIdAsync(int wfFormId, CancellationToken cancellationToken);
    Task<ServiceResponse<WfVm>> WfSelectByIdAsync(int wfId, CancellationToken cancellationToken);
    Task<ServiceResponse<WfVm>> WfInsertAsync(WfVm wf, CancellationToken cancellationToken);
    Task<ServiceResponse<WfVm>> WfUpdateAsync(WfVm wf, CancellationToken cancellationToken);
    Task<ServiceResponse> WfDeleteAsync(int wfId, CancellationToken cancellationToken);
}
