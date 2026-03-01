using ResearchApps.Domain;

namespace ResearchApps.Repo.Interface;

public interface IWfTransRepo
{
    Task<IEnumerable<WfTransHistory>> WfTransSelectByRefIdAsync(string refId, int wfFormId, CancellationToken cancellationToken);
}
