using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IItemGroup01Repo
{
    Task<IEnumerable<ItemGroup01>> CboAsync(CboRequest cboRequest, CancellationToken cancellationToken);
    Task<ItemGroup01> SelectByIdAsync(int itemGroup01Id, CancellationToken cancellationToken);
    Task<PagedList<ItemGroup01>> SelectAsync(PagedListRequest request, CancellationToken cancellationToken);
    Task<ItemGroup01> InsertAsync(ItemGroup01 itemGroup01, CancellationToken cancellationToken);
    Task<ItemGroup01> UpdateAsync(ItemGroup01 itemGroup01, CancellationToken cancellationToken);
    Task DeleteAsync(int itemGroup01Id, string modifiedBy, CancellationToken cancellationToken);
}
