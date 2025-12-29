using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IReportService
{
    Task<ServiceResponse> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    Task<ServiceResponse> SelectByIdAsync(int reportId, CancellationToken cancellationToken);
    Task<ServiceResponse> InsertAsync(ReportVm reportVm, CancellationToken cancellationToken);
    Task<ServiceResponse> UpdateAsync(ReportVm reportVm, CancellationToken cancellationToken);
    Task<ServiceResponse> DeleteAsync(int reportId, string modifiedBy, CancellationToken cancellationToken);
    Task<ServiceResponse> CboAsync();
    Task<ServiceResponse> GetParametersAsync(int reportId, CancellationToken cancellationToken);
    Task<ServiceResponse> GenerateReportAsync(ReportGenerateVm generateVm, CancellationToken cancellationToken);
}

