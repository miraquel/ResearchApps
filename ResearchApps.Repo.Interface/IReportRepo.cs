using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IReportRepo
{
    Task<IEnumerable<Report>> CboAsync();
    Task DeleteAsync(int reportId, string modifiedBy, CancellationToken cancellationToken);
    Task<Report> UpdateAsync(Report report, CancellationToken cancellationToken);
    Task<Report> InsertAsync(Report report, CancellationToken cancellationToken);
    Task<PagedList<Report>> SelectAsync(PagedListRequest request, CancellationToken cancellationToken);
    Task<Report?> SelectByIdAsync(int reportId, CancellationToken cancellationToken);
}