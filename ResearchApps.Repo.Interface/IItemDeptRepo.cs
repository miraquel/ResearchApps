using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using System.Threading;

namespace ResearchApps.Repo.Interface;

public interface IItemDeptRepo
{
    Task<IEnumerable<ItemDept>> CboAsync(CboRequest cboRequest, CancellationToken cancellationToken);
    Task<ItemDept> SelectByIdAsync(int itemDeptId, CancellationToken cancellationToken);
    Task<PagedList<ItemDept>> SelectAsync(PagedListRequest request, CancellationToken cancellationToken);
    Task<ItemDept> InsertAsync(ItemDept itemDept, CancellationToken cancellationToken);
    Task<ItemDept> UpdateAsync(ItemDept itemDept, CancellationToken cancellationToken);
    Task DeleteAsync(int itemDeptId, string modifiedBy, CancellationToken cancellationToken);
}

