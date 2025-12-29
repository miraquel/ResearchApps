using System.Data;
using Microsoft.AspNetCore.Http;
using ResearchApps.Domain;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public class ReportService : IReportService
{
    private readonly IReportRepo _reportRepo;
    private readonly IReportParameterRepo _reportParameterRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly MapperlyMapper _mapper = new();

    public ReportService(
        IReportRepo reportRepo, 
        IReportParameterRepo reportParameterRepo,
        IDbTransaction dbTransaction, 
        UserClaimDto userClaimDto)
    {
        _reportRepo = reportRepo;
        _reportParameterRepo = reportParameterRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
    }

    public async Task<ServiceResponse> CboAsync()
    {
        var reports = await _reportRepo.CboAsync();
        return ServiceResponse<IEnumerable<ReportVm>>.Success(
            _mapper.MapToVm(reports), 
            "Reports for combo box retrieved successfully.");
    }

    public async Task<ServiceResponse> DeleteAsync(int reportId, string modifiedBy, CancellationToken cancellationToken)
    {
        // First delete all parameters associated with this report
        await _reportParameterRepo.DeleteByReportIdAsync(reportId, cancellationToken);
        
        // Then delete the report
        await _reportRepo.DeleteAsync(reportId, modifiedBy, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse.Success("Report deleted successfully.");
    }

    public async Task<ServiceResponse> GetParametersAsync(int reportId, CancellationToken cancellationToken)
    {
        var parameters = await _reportParameterRepo.SelectByReportIdAsync(reportId, cancellationToken);
        return ServiceResponse<IEnumerable<ReportParameterVm>>.Success(
            _mapper.MapToVm(parameters),
            "Report parameters retrieved successfully.");
    }

    public async Task<ServiceResponse> InsertAsync(ReportVm reportVm, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(reportVm);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedReport = await _reportRepo.InsertAsync(entity, cancellationToken);
        
        // Insert parameters if any
        if (reportVm.Parameters.Count > 0)
        {
            foreach (var param in reportVm.Parameters)
            {
                var paramEntity = _mapper.MapToEntity(param);
                paramEntity.ReportId = insertedReport.ReportId;
                await _reportParameterRepo.InsertAsync(paramEntity, cancellationToken);
            }
        }
        
        _dbTransaction.Commit();
        return ServiceResponse<ReportVm>.Success(
            _mapper.MapToVm(insertedReport), 
            "Report inserted successfully.", 
            StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var reports = await _reportRepo.SelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<ReportVm>>.Success( _mapper.MapToVm(reports), "Reports retrieved successfully.");
    }

    public async Task<ServiceResponse> SelectByIdAsync(int reportId, CancellationToken cancellationToken)
    {
        var report = await _reportRepo.SelectByIdAsync(reportId, cancellationToken);
        if (report == null)
        {
            return ServiceResponse.Failure("Report not found.", StatusCodes.Status404NotFound);
        }
        
        var reportVm = _mapper.MapToVm(report);
        
        // Also fetch parameters
        var parameters = await _reportParameterRepo.SelectByReportIdAsync(reportId, cancellationToken);
        reportVm.Parameters = _mapper.MapToVm(parameters).ToList();
        
        return ServiceResponse<ReportVm>.Success(reportVm, "Report retrieved successfully.");
    }

    public async Task<ServiceResponse> UpdateAsync(ReportVm reportVm, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(reportVm);
        entity.ModifiedBy = _userClaimDto.Username;
        var updatedReport = await _reportRepo.UpdateAsync(entity, cancellationToken);
        
        // Delete existing parameters and re-insert
        await _reportParameterRepo.DeleteByReportIdAsync(reportVm.ReportId, cancellationToken);
        
        if (reportVm.Parameters.Count > 0)
        {
            foreach (var paramEntity in reportVm.Parameters.Select(param => _mapper.MapToEntity(param)))
            {
                paramEntity.ReportId = updatedReport.ReportId;
                await _reportParameterRepo.InsertAsync(paramEntity, cancellationToken);
            }
        }
        
        _dbTransaction.Commit();
        return ServiceResponse<ReportVm>.Success(
            _mapper.MapToVm(updatedReport), 
            "Report updated successfully.");
    }

    public async Task<ServiceResponse> GenerateReportAsync(ReportGenerateVm generateVm, CancellationToken cancellationToken)
    {
        var report = await _reportRepo.SelectByIdAsync(generateVm.ReportId, cancellationToken);
        if (report == null)
        {
            return ServiceResponse.Failure("Report not found.", StatusCodes.Status404NotFound);
        }

        // Get parameters for validation
        var parameters = await _reportParameterRepo.SelectByReportIdAsync(generateVm.ReportId, cancellationToken);
        
        // Validate required parameters
        var reportParameterDto = parameters as ReportParameter[] ?? parameters.ToArray();
        foreach (var param in reportParameterDto.Where(p => p.IsRequired))
        {
            if (!generateVm.ParameterValues.TryGetValue(param.ParameterName, out var value) || string.IsNullOrWhiteSpace(value))
            {
                return ServiceResponse.Failure($"Parameter '{param.DisplayLabel}' is required.");
            }
        }

        // The actual report generation will be handled by a dedicated report generator service
        // This method returns the report metadata and validated parameters
        var result = new ReportGenerateVm
        {
            ReportId = report.ReportId,
            ReportName = report.ReportName,
            ReportType = (int)report.ReportType,
            Parameters = _mapper.MapToVm(reportParameterDto).ToList(),
            ParameterValues = generateVm.ParameterValues,
            OutputFormat = generateVm.OutputFormat
        };

        return ServiceResponse<ReportGenerateVm>.Success(result, "Report generation prepared successfully.");
    }
}

