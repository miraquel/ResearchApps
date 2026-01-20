using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class PoService : IPoService
{
    private readonly IPoRepo _poRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<PoService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public PoService(
        IPoRepo poRepo,
        IDbTransaction dbTransaction,
        UserClaimDto userClaimDto,
        ILogger<PoService> logger)
    {
        _poRepo = poRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    #region CRUD Operations

    public async Task<ServiceResponse<PagedListVm<PoHeaderVm>>> PoSelect(PagedListRequestVm request, CancellationToken ct)
    {
        var pagedList = await _poRepo.PoSelect(_mapper.MapToEntity(request), ct);
        return ServiceResponse<PagedListVm<PoHeaderVm>>.Success(_mapper.MapToVm(pagedList));
    }

    public async Task<ServiceResponse<PoVm>> PoSelectById(int recId, CancellationToken ct)
    {
        var entity = await _poRepo.PoSelectById(recId, ct);
        if (entity == null)
            return ServiceResponse<PoVm>.Failure("Purchase order not found", StatusCodes.Status404NotFound);

        // For full details, we need to load lines too - but this is just header for now
        return ServiceResponse<PoVm>.Success(new PoVm { Header = _mapper.MapToVm(entity), Lines = new List<PoLineVm>() });
    }

    public async Task<ServiceResponse<PoVm>> PoInsert(PoHeaderVm poHeaderVm, CancellationToken ct)
    {
        var entity = _mapper.MapToEntity(poHeaderVm);
        entity.CreatedBy = _userClaimDto.Username;
        entity.CreatedDate = DateTime.UtcNow;
        entity.ModifiedBy = _userClaimDto.Username;
        entity.ModifiedDate = DateTime.UtcNow;

        var inserted = await _poRepo.PoInsert(entity, ct);
        _dbTransaction.Commit();

        LogPoCreated(inserted.PoId);

        return ServiceResponse<PoVm>.Success(
            new PoVm { Header = _mapper.MapToVm(inserted), Lines = new List<PoLineVm>() },
            "Purchase order created successfully.",
            StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<PoVm>> PoUpdate(PoHeaderVm poHeaderVm, CancellationToken ct)
    {
        LogUpdatingPo(poHeaderVm.RecId, _userClaimDto.Username);

        var entity = _mapper.MapToEntity(poHeaderVm);
        entity.ModifiedBy = _userClaimDto.Username;
        entity.ModifiedDate = DateTime.UtcNow;

        var updated = await _poRepo.PoUpdate(entity, ct);
        _dbTransaction.Commit();

        LogPoUpdated(updated.RecId);

        return ServiceResponse<PoVm>.Success(
            new PoVm { Header = _mapper.MapToVm(updated), Lines = new List<PoLineVm>() },
            "Purchase order updated successfully.");
    }

    public async Task<ServiceResponse> PoDelete(int recId, CancellationToken ct)
    {
        LogDeletingPo(recId, _userClaimDto.Username);

        await _poRepo.PoDelete(recId, ct);
        _dbTransaction.Commit();

        LogPoDeleted(recId);

        return ServiceResponse.Success("Purchase order deleted successfully.");
    }

    #endregion

    #region Workflow Operations

    public async Task<ServiceResponse<PoVm>> PoSubmitById(int recId, CancellationToken ct)
    {
        LogSubmittingPo(recId, _userClaimDto.Username);

        var result = await _poRepo.PoSubmitById(recId, _userClaimDto.Username, ct);
        _dbTransaction.Commit();

        LogPoSubmitted(recId, result.CurrentApprover ?? "N/A");

        return ServiceResponse<PoVm>.Success(
            new PoVm { Header = _mapper.MapToVm(result), Lines = new List<PoLineVm>() },
            "Purchase order submitted successfully.");
    }

    public async Task<ServiceResponse> PoRecallById(int recId, CancellationToken ct)
    {
        LogRecallingPo(recId, _userClaimDto.Username);

        await _poRepo.PoRecallById(recId, _userClaimDto.Username, ct);
        _dbTransaction.Commit();

        LogPoRecalled(recId);

        return ServiceResponse.Success("Purchase order recalled successfully.");
    }

    public async Task<ServiceResponse> PoApproveById(PoWorkflowActionVm action, CancellationToken ct)
    {
        LogApprovingPo(action.RecId, _userClaimDto.Username);

        await _poRepo.PoApproveById(action.RecId, action.Notes, _userClaimDto.Username, ct);
        _dbTransaction.Commit();

        LogPoApproved(action.RecId);

        return ServiceResponse.Success("Purchase order approved successfully.");
    }

    public async Task<ServiceResponse> PoRejectById(PoWorkflowActionVm action, CancellationToken ct)
    {
        LogRejectingPo(action.RecId, _userClaimDto.Username);

        await _poRepo.PoRejectById(action.RecId, action.Notes, _userClaimDto.Username, ct);
        _dbTransaction.Commit();

        LogPoRejected(action.RecId);

        return ServiceResponse.Success("Purchase order rejected successfully.");
    }

    public async Task<ServiceResponse> PoCloseById(int recId, CancellationToken ct)
    {
        LogClosingPo(recId, _userClaimDto.Username);

        await _poRepo.PoCloseById(recId, _userClaimDto.Username, ct);
        _dbTransaction.Commit();

        LogPoClosed(recId);

        return ServiceResponse.Success("Purchase order closed successfully.");
    }

    #endregion

    #region Outstanding Operations

    public async Task<ServiceResponse<IEnumerable<PoHeaderOutstandingVm>>> PoOsSelect(int supplierId, CancellationToken ct)
    {
        var entities = await _poRepo.PoOsSelect(supplierId, ct);
        return ServiceResponse<IEnumerable<PoHeaderOutstandingVm>>.Success(_mapper.MapToVm(entities));
    }

    public async Task<ServiceResponse<IEnumerable<PoLineOutstandingVm>>> PoOsSelectById(int recId, CancellationToken ct)
    {
        var entities = await _poRepo.PoOsSelectById(recId, ct);
        return ServiceResponse<IEnumerable<PoLineOutstandingVm>>.Success(_mapper.MapToVm(entities));
    }

    #endregion

    #region Logging Methods

    [LoggerMessage(LogLevel.Information, "Purchase order {PoId} created")]
    partial void LogPoCreated(string poId);

    [LoggerMessage(LogLevel.Information, "Updating purchase order {RecId} by {Username}")]
    partial void LogUpdatingPo(int recId, string username);

    [LoggerMessage(LogLevel.Information, "Purchase order {RecId} updated")]
    partial void LogPoUpdated(int recId);

    [LoggerMessage(LogLevel.Information, "Deleting purchase order {RecId} by {Username}")]
    partial void LogDeletingPo(int recId, string username);

    [LoggerMessage(LogLevel.Information, "Purchase order {RecId} deleted")]
    partial void LogPoDeleted(int recId);

    [LoggerMessage(LogLevel.Information, "Submitting purchase order {RecId} by {Username}")]
    partial void LogSubmittingPo(int recId, string username);

    [LoggerMessage(LogLevel.Information, "Purchase order {RecId} submitted, next approver: {Approver}")]
    partial void LogPoSubmitted(int recId, string approver);

    [LoggerMessage(LogLevel.Information, "Recalling purchase order {RecId} by {Username}")]
    partial void LogRecallingPo(int recId, string username);

    [LoggerMessage(LogLevel.Information, "Purchase order {RecId} recalled")]
    partial void LogPoRecalled(int recId);

    [LoggerMessage(LogLevel.Information, "Approving purchase order {RecId} by {Username}")]
    partial void LogApprovingPo(int recId, string username);

    [LoggerMessage(LogLevel.Information, "Purchase order {RecId} approved")]
    partial void LogPoApproved(int recId);

    [LoggerMessage(LogLevel.Information, "Rejecting purchase order {RecId} by {Username}")]
    partial void LogRejectingPo(int recId, string username);

    [LoggerMessage(LogLevel.Information, "Purchase order {RecId} rejected")]
    partial void LogPoRejected(int recId);

    [LoggerMessage(LogLevel.Information, "Closing purchase order {RecId} by {Username}")]
    partial void LogClosingPo(int recId, string username);

    [LoggerMessage(LogLevel.Information, "Purchase order {RecId} closed")]
    partial void LogPoClosed(int recId);

    #endregion
}
