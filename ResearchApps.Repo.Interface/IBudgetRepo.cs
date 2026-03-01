using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IBudgetRepo
{
    Task<PagedList<Budget>> BudgetSelectAsync(PagedListRequest request, CancellationToken cancellationToken);
    Task<Budget?> BudgetSelectByIdAsync(int budgetId, CancellationToken cancellationToken);
    Task<Budget> BudgetInsertAsync(Budget budget, CancellationToken cancellationToken);
    Task<Budget> BudgetUpdateAsync(Budget budget, CancellationToken cancellationToken);
    Task BudgetDeleteAsync(int budgetId, string modifiedBy, CancellationToken cancellationToken);
    Task<IEnumerable<Budget>> BudgetCboAsync(CboRequest cboRequest, CancellationToken cancellationToken);
}