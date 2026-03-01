using ResearchApps.Domain;

namespace ResearchApps.Repo.Interface;

public interface IRepStockRepo
{
    /// <summary>
    /// Calls [Rep_StockCard_Monthly] stored procedure
    /// </summary>
    Task<IEnumerable<RepStockCardMonthly>> RepStockCardMonthly(int itemId, int year, int month, CancellationToken ct);
}
