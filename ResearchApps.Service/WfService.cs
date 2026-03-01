using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class WfService : IWfService
{
    private readonly IWfRepo _wfRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<WfService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public WfService(
        IWfRepo wfRepo,
        IDbTransaction dbTransaction,
        UserClaimDto userClaimDto,
        ILogger<WfService> logger)
    {
        _wfRepo = wfRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<WfVm>>> WfSelectByWfFormIdAsync(
        int wfFormId, CancellationToken cancellationToken)
    {
        var steps = await _wfRepo.WfSelectByWfFormIdAsync(wfFormId, cancellationToken);
        return ServiceResponse<IEnumerable<WfVm>>.Success(
            _mapper.MapToVm(steps), "Approval steps retrieved successfully.");
    }

    public async Task<ServiceResponse<WfVm>> WfSelectByIdAsync(int wfId, CancellationToken cancellationToken)
    {
        LogRetrievingWfById(wfId);
        var wf = await _wfRepo.WfSelectByIdAsync(wfId, cancellationToken);
        return ServiceResponse<WfVm>.Success(_mapper.MapToVm(wf), "Approval step retrieved successfully.");
    }

    public async Task<ServiceResponse<WfVm>> WfInsertAsync(WfVm wfVm, CancellationToken cancellationToken)
    {
        LogCreatingWf(wfVm.WfFormId, wfVm.UserId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(wfVm);
        var inserted = await _wfRepo.WfInsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogWfCreated(inserted.WfId);
        return ServiceResponse<WfVm>.Success(
            _mapper.MapToVm(inserted), "Approval step created successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<WfVm>> WfUpdateAsync(WfVm wfVm, CancellationToken cancellationToken)
    {
        LogUpdatingWf(wfVm.WfId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(wfVm);
        var updated = await _wfRepo.WfUpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogWfUpdated(updated.WfId);
        return ServiceResponse<WfVm>.Success(_mapper.MapToVm(updated), "Approval step updated successfully.");
    }

    public async Task<ServiceResponse> WfDeleteAsync(int wfId, CancellationToken cancellationToken)
    {
        LogDeletingWf(wfId, _userClaimDto.Username);
        await _wfRepo.WfDeleteAsync(wfId, cancellationToken);
        _dbTransaction.Commit();
        LogWfDeleted(wfId);
        return ServiceResponse.Success("Approval step deleted successfully.");
    }

    [LoggerMessage(LogLevel.Information, "Retrieving Wf by Id: {wfId}")]
    partial void LogRetrievingWfById(int wfId);

    [LoggerMessage(LogLevel.Information, "Creating approval step for form {wfFormId}, user {userId} by {username}")]
    partial void LogCreatingWf(int wfFormId, string userId, string username);

    [LoggerMessage(LogLevel.Information, "Approval step {wfId} created successfully")]
    partial void LogWfCreated(int wfId);

    [LoggerMessage(LogLevel.Information, "Updating approval step {wfId} by user: {username}")]
    partial void LogUpdatingWf(int wfId, string username);

    [LoggerMessage(LogLevel.Information, "Approval step {wfId} updated successfully")]
    partial void LogWfUpdated(int wfId);

    [LoggerMessage(LogLevel.Information, "Deleting approval step {wfId} by user: {username}")]
    partial void LogDeletingWf(int wfId, string username);

    [LoggerMessage(LogLevel.Information, "Approval step {wfId} deleted successfully")]
    partial void LogWfDeleted(int wfId);
}
