using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IPrStatusRepo
{
    Task<IEnumerable<PrStatus>> PrStatusCboAsync(CboRequest cboRequest, CancellationToken cancellationToken);
}