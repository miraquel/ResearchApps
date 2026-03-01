using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IItemGroup02Repo
{
    Task<IEnumerable<ItemGroup02>> CboAsync(CboRequest cboRequest, CancellationToken cancellationToken);
    Task<ItemGroup02> SelectByIdAsync(int itemGroup02Id, CancellationToken cancellationToken);
    Task<PagedList<ItemGroup02>> SelectAsync(PagedListRequest request, CancellationToken cancellationToken);
    Task<ItemGroup02> InsertAsync(ItemGroup02 itemGroup02, CancellationToken cancellationToken);
    Task<ItemGroup02> UpdateAsync(ItemGroup02 itemGroup02, CancellationToken cancellationToken);
    Task DeleteAsync(int itemGroup02Id, string modifiedBy, CancellationToken cancellationToken);
}
