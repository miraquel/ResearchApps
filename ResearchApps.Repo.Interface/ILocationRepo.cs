using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface ILocationRepo
{
    // LocationCbo
    Task<IEnumerable<Location>> LocationCboAsync(CboRequest cboRequest, CancellationToken cancellationToken);
    // LocationDelete
    Task LocationDeleteAsync(int locationId, CancellationToken cancellationToken);
    // LocationInsert
    Task<Location> LocationInsertAsync(Location location, CancellationToken cancellationToken);
    // LocationSelect
    Task<PagedList<Location>> LocationSelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken);
    // LocationSelectById
    Task<Location> LocationSelectByIdAsync(int locationId, CancellationToken cancellationToken);
    // LocationUpdate
    Task<Location> LocationUpdateAsync(Location location, CancellationToken cancellationToken);
}
