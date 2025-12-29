using ResearchApps.Domain;

namespace ResearchApps.Repo.Interface;

public interface IReportParameterRepo
{
    Task<ReportParameter?> SelectByIdAsync(int parameterId, CancellationToken cancellationToken);
    Task<IEnumerable<ReportParameter>> SelectByReportIdAsync(int reportId, CancellationToken cancellationToken);
    Task<ReportParameter> InsertAsync(ReportParameter parameter, CancellationToken cancellationToken);
    Task<ReportParameter> UpdateAsync(ReportParameter parameter, CancellationToken cancellationToken);
    Task DeleteAsync(int parameterId, CancellationToken cancellationToken);
    Task DeleteByReportIdAsync(int reportId, CancellationToken cancellationToken);
}

