using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IRepStockService
{
    Task<ServiceResponse<IEnumerable<RepInventTransByItemVm>>> RepInventTransByItem(int itemId, DateTime? startDate, DateTime? endDate, CancellationToken ct);
    Task<ServiceResponse<IEnumerable<RepStockCardMonthlyVm>>> RepStockCardMonthly(int itemId, int year, int month, CancellationToken ct);
}
