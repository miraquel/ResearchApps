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
    // Pr_SubmitById - Submit PR and start workflow
    Task<Pr> PrSubmitById(int id, string modifiedBy, CancellationToken cancellationToken);
    // Pr_ApproveById - Approve PR
    Task PrApproveById(int id, string notes, string modifiedBy, CancellationToken cancellationToken);
    // Pr_RejectById - Reject PR
    Task PrRejectById(int id, string notes, string modifiedBy, CancellationToken cancellationToken);
    // Pr_RecallById - Recall PR
    Task PrRecallById(int id, string modifiedBy, CancellationToken cancellationToken);
}