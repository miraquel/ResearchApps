using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IUnitRepo
{
    // UnitCbo
    Task<IEnumerable<Unit>> UnitCboAsync(CboRequest cboRequest, CancellationToken cancellationToken);
    // UnitDelete
    Task UnitDeleteAsync(int unitId, CancellationToken cancellationToken);
    // UnitInsert
    Task<Unit> UnitInsertAsync(Unit unit, CancellationToken cancellationToken);
    // UnitSelect
    Task<PagedList<Unit>> UnitSelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken);
    // UnitSelectById
    Task<Unit> UnitSelectByIdAsync(int unitId, CancellationToken cancellationToken);
    // UnitUpdate
    Task<Unit> UnitUpdateAsync(Unit unit, CancellationToken cancellationToken);
}