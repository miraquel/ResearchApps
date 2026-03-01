using ResearchApps.Domain;

namespace ResearchApps.Repo.Interface;

public interface IWfRepo
{
    Task<IEnumerable<Wf>> WfSelectByWfFormIdAsync(int wfFormId, CancellationToken cancellationToken);
    Task<Wf> WfSelectByIdAsync(int wfId, CancellationToken cancellationToken);
    Task<Wf> WfInsertAsync(Wf wf, CancellationToken cancellationToken);
    Task<Wf> WfUpdateAsync(Wf wf, CancellationToken cancellationToken);
    Task WfDeleteAsync(int wfId, CancellationToken cancellationToken);
}
