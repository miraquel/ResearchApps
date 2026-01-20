using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Domain;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class ReportService : IReportService
{
    private readonly IReportRepo _reportRepo;
    private readonly IReportParameterRepo _reportParameterRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<ReportService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public ReportService(
        IReportRepo reportRepo, 
        IReportParameterRepo reportParameterRepo,
        IDbTransaction dbTransaction, 
        UserClaimDto userClaimDto,
        ILogger<ReportService> logger)
    {
        _reportRepo = reportRepo;
        _reportParameterRepo = reportParameterRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<ReportVm>>> CboAsync()
    {
        var reports = await _reportRepo.CboAsync();
        return ServiceResponse<IEnumerable<ReportVm>>.Success(
            _mapper.MapToVm(reports), 
            "Reports for combo box retrieved successfully.");
    }

    public async Task<ServiceResponse> DeleteAsync(int reportId, string modifiedBy, CancellationToken cancellationToken)
    {
        LogDeletingReport(reportId, modifiedBy);
        // First delete all parameters associated with this report
        await _reportParameterRepo.DeleteByReportIdAsync(reportId, cancellationToken);
        
        // Then delete the report
        await _reportRepo.DeleteAsync(reportId, modifiedBy, cancellationToken);
        _dbTransaction.Commit();
        LogReportDeleted(reportId);
        return ServiceResponse.Success("Report deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<ReportParameterVm>>> GetParametersAsync(int reportId, CancellationToken cancellationToken)
    {
        var parameters = await _reportParameterRepo.SelectByReportIdAsync(reportId, cancellationToken);
        return ServiceResponse<IEnumerable<ReportParameterVm>>.Success(
            _mapper.MapToVm(parameters),
            "Report parameters retrieved successfully.");
    }

    public async Task<ServiceResponse<ReportVm>> InsertAsync(ReportVm reportVm, CancellationToken cancellationToken)
    {
        LogCreatingReport(reportVm.ReportName, _userClaimDto.Username);
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
        LogReportCreated(insertedReport.ReportId);
        return ServiceResponse<ReportVm>.Success(
            _mapper.MapToVm(insertedReport), 
            "Report inserted successfully.", 
            StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<PagedListVm<ReportVm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var reports = await _reportRepo.SelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<ReportVm>>.Success( _mapper.MapToVm(reports), "Reports retrieved successfully.");
    }

    public async Task<ServiceResponse<ReportVm>> SelectByIdAsync(int reportId, CancellationToken cancellationToken)
    {
        var report = await _reportRepo.SelectByIdAsync(reportId, cancellationToken);
        if (report == null)
        {
            return ServiceResponse<ReportVm>.Failure("Report not found.", StatusCodes.Status404NotFound);
        }
        
        var reportVm = _mapper.MapToVm(report);
        
        // Also fetch parameters
        var parameters = await _reportParameterRepo.SelectByReportIdAsync(reportId, cancellationToken);
        reportVm.Parameters = _mapper.MapToVm(parameters).ToList();
        
        return ServiceResponse<ReportVm>.Success(reportVm, "Report retrieved successfully.");
    }

    public async Task<ServiceResponse<ReportVm>> UpdateAsync(ReportVm reportVm, CancellationToken cancellationToken)
    {
        LogUpdatingReport(reportVm.ReportId, _userClaimDto.Username);
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
        LogReportUpdated(reportVm.ReportId);
        return ServiceResponse<ReportVm>.Success(
            _mapper.MapToVm(updatedReport), 
            "Report updated successfully.");
    }

    public async Task<ServiceResponse<ReportGenerateVm>> GenerateReportAsync(ReportGenerateVm generateVm, CancellationToken cancellationToken)
    {
        LogGeneratingReport(generateVm.ReportId, _userClaimDto.Username);
        var report = await _reportRepo.SelectByIdAsync(generateVm.ReportId, cancellationToken);
        if (report == null)
        {
            return ServiceResponse<ReportGenerateVm>.Failure("Report not found.", StatusCodes.Status404NotFound);
        }

        // Get parameters for validation
        var parameters = await _reportParameterRepo.SelectByReportIdAsync(generateVm.ReportId, cancellationToken);
        
        // Validate required parameters
        var reportParameterDto = parameters as ReportParameter[] ?? parameters.ToArray();
        foreach (var param in reportParameterDto.Where(p => p.IsRequired))
        {
            if (!generateVm.ParameterValues.TryGetValue(param.ParameterName, out var value) || string.IsNullOrWhiteSpace(value))
            {
                return ServiceResponse<ReportGenerateVm>.Failure($"Parameter '{param.DisplayLabel}' is required.");
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

        LogReportGenerated(generateVm.ReportId);
        return ServiceResponse<ReportGenerateVm>.Success(result, "Report generation prepared successfully.");
    }

    [LoggerMessage(LogLevel.Information, "Creating new Report: {reportName} by user: {username}")]
    partial void LogCreatingReport(string reportName, string username);

    [LoggerMessage(LogLevel.Information, "Report created successfully with Id: {reportId}")]
    partial void LogReportCreated(int reportId);

    [LoggerMessage(LogLevel.Information, "Updating Report {reportId} by user: {username}")]
    partial void LogUpdatingReport(int reportId, string username);

    [LoggerMessage(LogLevel.Information, "Report {reportId} updated successfully")]
    partial void LogReportUpdated(int reportId);

    [LoggerMessage(LogLevel.Information, "Deleting Report {reportId} by user: {username}")]
    partial void LogDeletingReport(int reportId, string username);

    [LoggerMessage(LogLevel.Information, "Report {reportId} deleted successfully")]
    partial void LogReportDeleted(int reportId);

    [LoggerMessage(LogLevel.Information, "Generating Report {reportId} by user: {username}")]
    partial void LogGeneratingReport(int reportId, string username);

    [LoggerMessage(LogLevel.Information, "Report {reportId} generation prepared successfully")]
    partial void LogReportGenerated(int reportId);
}