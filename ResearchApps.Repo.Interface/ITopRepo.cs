using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface ITopRepo
{
    Task<PagedList<Top>> SelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken);
    Task<Top> SelectByIdAsync(int topId, CancellationToken cancellationToken);
    Task<Top> InsertAsync(Top top, CancellationToken cancellationToken);
    Task<Top> UpdateAsync(Top top, CancellationToken cancellationToken);
    Task DeleteAsync(int topId, string modifiedBy, CancellationToken cancellationToken);
    Task<IEnumerable<Top>> CboAsync(CboRequest cboRequest, CancellationToken cancellationToken);
}
