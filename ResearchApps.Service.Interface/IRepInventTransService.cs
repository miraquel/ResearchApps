using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IRepInventTransService
{
    Task<ServiceResponse<IEnumerable<RepInventTransByItemVm>>> RepInventTransByItem(int itemId, DateTime? startDate, DateTime? endDate, CancellationToken ct);
}
