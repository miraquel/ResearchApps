using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IReportService
{
    Task<ServiceResponse<PagedListVm<ReportVm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken);
    Task<ServiceResponse<ReportVm>> SelectByIdAsync(int reportId, CancellationToken cancellationToken);
    Task<ServiceResponse<ReportVm>> InsertAsync(ReportVm reportVm, CancellationToken cancellationToken);
    Task<ServiceResponse<ReportVm>> UpdateAsync(ReportVm reportVm, CancellationToken cancellationToken);
    Task<ServiceResponse> DeleteAsync(int reportId, string modifiedBy, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<ReportVm>>> CboAsync();
    Task<ServiceResponse<IEnumerable<ReportParameterVm>>> GetParametersAsync(int reportId, CancellationToken cancellationToken);
    Task<ServiceResponse<ReportGenerateVm>> GenerateReportAsync(ReportGenerateVm generateVm, CancellationToken cancellationToken);
}

