using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class WfFormService : IWfFormService
{
    private readonly IWfFormRepo _wfFormRepo;
    private readonly IWfRepo _wfRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<WfFormService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public WfFormService(
        IWfFormRepo wfFormRepo,
        IWfRepo wfRepo,
        IDbTransaction dbTransaction,
        UserClaimDto userClaimDto,
        ILogger<WfFormService> logger)
    {
        _wfFormRepo = wfFormRepo;
        _wfRepo = wfRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PagedListVm<WfFormVm>>> WfFormSelectAsync(
        PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var wfForms = await _wfFormRepo.WfFormSelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<WfFormVm>>.Success(
            _mapper.MapToVm(wfForms), "Workflow forms retrieved successfully.");
    }

    public async Task<ServiceResponse<WfFormVm>> WfFormSelectByIdAsync(
        int wfFormId, CancellationToken cancellationToken)
    {
        LogRetrievingWfFormById(wfFormId);
        var wfForm = await _wfFormRepo.WfFormSelectByIdAsync(wfFormId, cancellationToken);
        return ServiceResponse<WfFormVm>.Success(
            _mapper.MapToVm(wfForm), "Workflow form retrieved successfully.");
    }

    public async Task<ServiceResponse<WfFormVm>> WfFormInsertAsync(
        WfFormVm wfFormVm, CancellationToken cancellationToken)
    {
        LogCreatingWfForm(wfFormVm.FormName, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(wfFormVm);
        var inserted = await _wfFormRepo.WfFormInsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogWfFormCreated(inserted.WfFormId);
        return ServiceResponse<WfFormVm>.Success(
            _mapper.MapToVm(inserted), "Workflow form created successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<WfFormVm>> WfFormUpdateAsync(
        WfFormVm wfFormVm, CancellationToken cancellationToken)
    {
        LogUpdatingWfForm(wfFormVm.WfFormId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(wfFormVm);
        var updated = await _wfFormRepo.WfFormUpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogWfFormUpdated(updated.WfFormId);
        return ServiceResponse<WfFormVm>.Success(
            _mapper.MapToVm(updated), "Workflow form updated successfully.");
    }

    public async Task<ServiceResponse> WfFormDeleteAsync(int wfFormId, CancellationToken cancellationToken)
    {
        LogDeletingWfForm(wfFormId, _userClaimDto.Username);
        await _wfFormRepo.WfFormDeleteAsync(wfFormId, cancellationToken);
        _dbTransaction.Commit();
        LogWfFormDeleted(wfFormId);
        return ServiceResponse.Success("Workflow form deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<WfFormVm>>> WfFormCboAsync(
        CboRequestVm cboRequest, CancellationToken cancellationToken)
    {
        var wfForms = await _wfFormRepo.WfFormCboAsync(_mapper.MapToEntity(cboRequest), cancellationToken);
        return ServiceResponse<IEnumerable<WfFormVm>>.Success(
            _mapper.MapToVm(wfForms), "Workflow forms for combo box retrieved successfully.");
    }

    public async Task<ServiceResponse<WorkflowConfigVm>> GetWorkflowConfigAsync(
        int wfFormId, CancellationToken cancellationToken)
    {
        LogRetrievingWfFormById(wfFormId);
        var form = await _wfFormRepo.WfFormSelectByIdAsync(wfFormId, cancellationToken);
        var steps = await _wfRepo.WfSelectByWfFormIdAsync(wfFormId, cancellationToken);

        var configVm = new WorkflowConfigVm
        {
            Form = _mapper.MapToVm(form),
            Steps = _mapper.MapToVm(steps).ToList()
        };

        return ServiceResponse<WorkflowConfigVm>.Success(configVm, "Workflow configuration retrieved successfully.");
    }

    [LoggerMessage(LogLevel.Information, "Retrieving WfForm by Id: {wfFormId}")]
    partial void LogRetrievingWfFormById(int wfFormId);

    [LoggerMessage(LogLevel.Information, "Creating new WfForm: {formName} by user: {username}")]
    partial void LogCreatingWfForm(string formName, string username);

    [LoggerMessage(LogLevel.Information, "WfForm created successfully with Id: {wfFormId}")]
    partial void LogWfFormCreated(int wfFormId);

    [LoggerMessage(LogLevel.Information, "Updating WfForm {wfFormId} by user: {username}")]
    partial void LogUpdatingWfForm(int wfFormId, string username);

    [LoggerMessage(LogLevel.Information, "WfForm {wfFormId} updated successfully")]
    partial void LogWfFormUpdated(int wfFormId);

    [LoggerMessage(LogLevel.Information, "Deleting WfForm {wfFormId} by user: {username}")]
    partial void LogDeletingWfForm(int wfFormId, string username);

    [LoggerMessage(LogLevel.Information, "WfForm {wfFormId} deleted successfully")]
    partial void LogWfFormDeleted(int wfFormId);
}
