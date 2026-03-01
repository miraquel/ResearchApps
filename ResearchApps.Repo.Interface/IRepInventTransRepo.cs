using ResearchApps.Domain;

namespace ResearchApps.Repo.Interface;

public interface IRepInventTransRepo
{
    /// <summary>
    /// Calls [Rep_InventTrans_ByItem] stored procedure
    /// </summary>
    Task<IEnumerable<RepInventTransByItem>> RepInventTransByItem(int itemId, DateTime? startDate, DateTime? endDate, CancellationToken ct);
}
