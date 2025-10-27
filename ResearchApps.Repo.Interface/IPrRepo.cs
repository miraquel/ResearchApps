using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IPrRepo
{
    // Pr_Delete
    Task PrDelete(int id, CancellationToken cancellationToken);
    // Pr_Insert
    Task<int> PrInsert(Pr pr, CancellationToken cancellationToken);
    // Pr_Select
    Task<PagedList<Pr>> PrSelect(PagedListRequest request, CancellationToken cancellationToken);
    // Pr_SelectById
    Task<Pr> PrSelectById(int id, CancellationToken cancellationToken);
    // Pr_Update
    Task PrUpdate(Pr pr, CancellationToken cancellationToken);
}