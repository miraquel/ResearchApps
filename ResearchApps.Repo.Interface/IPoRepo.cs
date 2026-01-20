using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IPoRepo
{
    // CRUD Operations
    Task<PagedList<Po>> PoSelect(PagedListRequest request, CancellationToken ct);
    Task<Po?> PoSelectById(int recId, CancellationToken ct);
    Task<Po> PoInsert(Po po, CancellationToken ct);
    Task<Po> PoUpdate(Po po, CancellationToken ct);
    Task PoDelete(int recId, CancellationToken ct);

    // Workflow Operations
    Task<Po> PoSubmitById(int recId, string modifiedBy, CancellationToken ct);
    Task PoRecallById(int recId, string modifiedBy, CancellationToken ct);
    Task PoApproveById(int recId, string? notes, string modifiedBy, CancellationToken ct);
    Task PoRejectById(int recId, string? notes, string modifiedBy, CancellationToken ct);
    Task PoCloseById(int recId, string modifiedBy, CancellationToken ct);

    // Outstanding Operations
    Task<IEnumerable<PoHeaderOutstanding>> PoOsSelect(int supplierId, CancellationToken ct);
    Task<IEnumerable<PoLineOutstanding>> PoOsSelectById(int recId, CancellationToken ct);
}
