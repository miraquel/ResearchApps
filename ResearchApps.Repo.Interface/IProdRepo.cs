using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IProdRepo
{
    Task<PagedList<Prod>> SelectAsync(PagedListRequest request, CancellationToken ct);
    Task<Prod> SelectByIdAsync(int recId, CancellationToken ct);
    Task<Prod> SelectByProdIdAsync(string prodId, CancellationToken ct);
    Task<int> InsertAsync(Prod prod, CancellationToken ct);
    Task UpdateAsync(Prod prod, CancellationToken ct);
    Task DeleteAsync(int recId, CancellationToken ct);
    Task<IEnumerable<ProdStatus>> ProdStatusCboAsync(CancellationToken ct);
}
