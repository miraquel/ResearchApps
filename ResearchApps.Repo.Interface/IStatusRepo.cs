using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IStatusRepo
{
    Task<IEnumerable<Status>> StatusCboAsync(CboRequest cboRequest, CancellationToken cancellationToken);
}