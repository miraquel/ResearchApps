using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IBudgetRepo
{
    Task<IEnumerable<Budget>> BudgetCboAsync(CboRequest cboRequest, CancellationToken cancellationToken);
}