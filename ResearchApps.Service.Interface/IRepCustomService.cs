using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IRepCustomService
{
    Task<ServiceResponse<IEnumerable<RepToolsVm>>> RepTools(int year, int month, CancellationToken ct);
    Task<ServiceResponse<IEnumerable<RepToolsAnalysisVm>>> RepToolsAnalysis(int year, int month, CancellationToken ct);
}
