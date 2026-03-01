using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IWfFormRepo
{
    Task<PagedList<WfForm>> WfFormSelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken);
    Task<WfForm> WfFormSelectByIdAsync(int wfFormId, CancellationToken cancellationToken);
    Task<WfForm> WfFormInsertAsync(WfForm wfForm, CancellationToken cancellationToken);
    Task<WfForm> WfFormUpdateAsync(WfForm wfForm, CancellationToken cancellationToken);
    Task WfFormDeleteAsync(int wfFormId, CancellationToken cancellationToken);
    Task<IEnumerable<WfForm>> WfFormCboAsync(CboRequest cboRequest, CancellationToken cancellationToken);
}
