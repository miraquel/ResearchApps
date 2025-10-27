using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IWarehouseRepo
{
    // WhCbo
    Task<IEnumerable<Warehouse>> CboAsync();
    // WhDelete
    Task DeleteAsync(int whId, string modifiedBy, CancellationToken cancellationToken);
    // WhInsert
    Task<Warehouse> InsertAsync(Warehouse warehouse, CancellationToken cancellationToken);
    // WhSelect
    Task<PagedList<Warehouse>> SelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken);
    // WhSelectById
    Task<Warehouse> SelectByIdAsync(int whId, CancellationToken cancellationToken);
    // WhUpdate
    Task<Warehouse> UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken);
}