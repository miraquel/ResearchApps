using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface ISalesPriceRepo
{
    Task<PagedList<SalesPrice>> SelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken);
    Task<SalesPrice> SelectByIdAsync(int recId, CancellationToken cancellationToken);
    Task<SalesPrice> InsertAsync(SalesPrice salesPrice, CancellationToken cancellationToken);
    Task<SalesPrice> UpdateAsync(SalesPrice salesPrice, CancellationToken cancellationToken);
    Task DeleteAsync(int recId, string modifiedBy, CancellationToken cancellationToken);
}
