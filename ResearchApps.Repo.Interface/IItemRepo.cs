using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IItemRepo
{
    // ItemDelete
    Task DeleteAsync(int itemId, CancellationToken cancellationToken);
    // ItemInsert
    Task<Item> InsertAsync(Item item, CancellationToken cancellationToken);
    // ItemSelect
    Task<PagedList<Item>> SelectAsync(PagedListRequest listRequest, CancellationToken cancellationToken);
    // ItemSelectById
    Task<Item> SelectByIdAsync(int itemId, CancellationToken cancellationToken);
    // ItemUpdate
    Task<Item> UpdateAsync(Item item, CancellationToken cancellationToken);
}