using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IWfTransService
{
    Task<ServiceResponse<IEnumerable<WfTransHistoryVm>>> WfTransSelectByRefIdAsync(string refId, int wfFormId, CancellationToken cancellationToken);
}
