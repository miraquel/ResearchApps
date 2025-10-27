using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IItemTypeRepo
{
    public Task<PagedList<ItemType>> ItemTypeSelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken);
    public Task<ItemType> ItemTypeSelectByIdAsync(int itemTypeId, CancellationToken cancellationToken);
    public Task<ItemType> ItemTypeInsertAsync(ItemType itemType, CancellationToken cancellationToken);
    public Task<ItemType> ItemTypeUpdateAsync(ItemType itemType, CancellationToken cancellationToken);
    public Task ItemTypeDeleteAsync(int itemTypeId, CancellationToken cancellationToken);
    public Task<IEnumerable<ItemType>> ItemTypeCbo(CboRequest pagedCboRequest, CancellationToken cancellationToken);
}