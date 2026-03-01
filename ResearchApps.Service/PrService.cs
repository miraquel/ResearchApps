using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class PrService : IPrService
{
    private readonly IPrRepo _prRepo;
    private readonly IPrLineRepo _prLineRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<PrService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public PrService(IPrRepo prRepo, IPrLineRepo prLineRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto, ILogger<PrService> logger)
    {
        _prRepo = prRepo;
        _prLineRepo = prLineRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse> PrDelete(int id, CancellationToken cancellationToken)
    {
        LogDeletingPrWithRecIdRecIdByUserUsername(id, _userClaimDto.Username);
        await _prRepo.PrDelete(id, cancellationToken);
        _dbTransaction.Commit();
        LogPrRecIdDeletedSuccessfully(id);
        return ServiceResponse.Success("PR deleted successfully.");
    }

    public async Task<ServiceResponse<int>> PrInsert(PrVm pr, CancellationToken cancellationToken)
    {
        LogCreatingNewPrPrNameByUserUsername(pr.PrName, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(pr);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedPrId = await _prRepo.PrInsert(entity, cancellationToken);
        _dbTransaction.Commit();
        LogPrCreatedSuccessfullyWithRecIdRecId(insertedPrId);
        return ServiceResponse<int>.Success(insertedPrId, "PR inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<PagedListVm<PrVm>>> PrSelect(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogRetrievingPrsListPagePagePagesizePagesize(request.PageNumber, request.PageSize);
        var prs = await _prRepo.PrSelect(_mapper.MapToEntity(request), cancellationToken);
        LogRetrievedCountPrs(prs.TotalCount);
        return ServiceResponse<PagedListVm<PrVm>>.Success(_mapper.MapToVm(prs), "PRs retrieved successfully.");
    }

    public async Task<ServiceResponse<PrVm>> PrSelectById(int id, CancellationToken cancellationToken)
    {
        LogRetrievingPrByRecIdRecId(id);
        var pr = await _prRepo.PrSelectById(id, cancellationToken);
        return ServiceResponse<PrVm>.Success(_mapper.MapToVm(pr), "PR retrieved successfully.");
    }

    public async Task<ServiceResponse<PrCompositeVm>> GetPurchaseRequisition(int recId, CancellationToken cancellationToken)
    {
        LogRetrievingPrCompositeByRecIdRecId(recId);
        
        var pr = await _prRepo.PrSelectById(recId, cancellationToken);
        var lines = await _prLineRepo.PrLineSelectByPr(pr.PrId, cancellationToken);

        var viewModel = new PrCompositeVm
        {
            Header = _mapper.MapToVm(pr),
            Lines = _mapper.MapToVm(lines).ToList()
        };

        LogPrCompositeRetrievedSuccessfullyWithLinesCount(viewModel.Lines.Count);
        return ServiceResponse<PrCompositeVm>.Success(viewModel, "PR Composite ViewModel retrieved successfully.");
    }

    public async Task<ServiceResponse> PrUpdate(PrVm pr, CancellationToken cancellationToken)
    {
        LogUpdatingPrRecIdPrNameByUserUsername(pr.RecId, pr.PrName, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(pr);
        entity.ModifiedBy = _userClaimDto.Username;
        await _prRepo.PrUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogPrRecIdUpdatedSuccessfully(pr.RecId);
        return ServiceResponse.Success("PR updated successfully.");
    }

    public async Task<ServiceResponse> PrSubmitById(int id, CancellationToken cancellationToken)
    {
        LogSubmittingPrRecIdForApprovalByUserUsername(id, _userClaimDto.Username);
        await _prRepo.PrSubmitById(id, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogPrRecIdSubmittedSuccessfullyCurrentApproverApprover(id);
        return ServiceResponse.Success("PR submitted for approval successfully.");
    }

    public async Task<ServiceResponse> PrApproveById(PrWorkflowActionVm action, CancellationToken cancellationToken)
    {
        LogApprovingPrRecIdByUserUsernameNotesNotes(action.RecId, _userClaimDto.Username, action.Notes ?? string.Empty);
        await _prRepo.PrApproveById(action.RecId, action.Notes ?? "", _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogPrRecIdApprovedSuccessfullyByUsername(action.RecId, _userClaimDto.Username);
        return ServiceResponse.Success("PR approved successfully.");
    }

    public async Task<ServiceResponse> PrRejectById(PrWorkflowActionVm action, CancellationToken cancellationToken)
    {
        LogRejectingPrRecIdByUserUsernameReasonNotes(action.RecId, _userClaimDto.Username, action.Notes ?? string.Empty);
        await _prRepo.PrRejectById(action.RecId, action.Notes ?? "", _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogPrRecIdRejectedByUsername(action.RecId, _userClaimDto.Username);
        return ServiceResponse.Success("PR rejected successfully.");
    }

    public async Task<ServiceResponse> PrRecallById(int id, CancellationToken cancellationToken)
    {
        LogRecallingPrRecIdByUserUsername(id, _userClaimDto.Username);
        await _prRepo.PrRecallById(id, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogPrRecIdRecalledSuccessfullyByUsername(id, _userClaimDto.Username);
        return ServiceResponse.Success("PR recalled successfully.");
    }

    [LoggerMessage(LogLevel.Information, "Deleting PR with RecId: {recId} by user: {username}")]
    partial void LogDeletingPrWithRecIdRecIdByUserUsername(int recId, string username);

    [LoggerMessage(LogLevel.Information, "PR {recId} deleted successfully")]
    partial void LogPrRecIdDeletedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Creating new PR: {prName} by user: {username}")]
    partial void LogCreatingNewPrPrNameByUserUsername(string prName, string username);

    [LoggerMessage(LogLevel.Information, "PR created successfully with RecId: {recId}")]
    partial void LogPrCreatedSuccessfullyWithRecIdRecId(int recId);

    [LoggerMessage(LogLevel.Debug, "Retrieving PRs list - Page: {page}, PageSize: {pageSize}")]
    partial void LogRetrievingPrsListPagePagePagesizePagesize(int page, int pageSize);

    [LoggerMessage(LogLevel.Debug, "Retrieved {count} PRs")]
    partial void LogRetrievedCountPrs(int count);

    [LoggerMessage(LogLevel.Debug, "Retrieving PR by RecId: {recId}")]
    partial void LogRetrievingPrByRecIdRecId(int recId);

    [LoggerMessage(LogLevel.Debug, "Retrieving PR Composite by RecId: {recId}")]
    partial void LogRetrievingPrCompositeByRecIdRecId(int recId);

    [LoggerMessage(LogLevel.Debug, "PR Composite retrieved successfully with {count} lines")]
    partial void LogPrCompositeRetrievedSuccessfullyWithLinesCount(int count);

    [LoggerMessage(LogLevel.Information, "Updating PR {recId}: {prName} by user: {username}")]
    partial void LogUpdatingPrRecIdPrNameByUserUsername(int recId, string prName, string username);

    [LoggerMessage(LogLevel.Information, "PR {recId} updated successfully")]
    partial void LogPrRecIdUpdatedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Submitting PR {recId} for approval by user: {username}")]
    partial void LogSubmittingPrRecIdForApprovalByUserUsername(int recId, string username);

    [LoggerMessage(LogLevel.Information, "PR {recId} submitted successfully.")]
    partial void LogPrRecIdSubmittedSuccessfullyCurrentApproverApprover(int recId);

    [LoggerMessage(LogLevel.Information, "Approving PR {recId} by user: {username}. Notes: {notes}")]
    partial void LogApprovingPrRecIdByUserUsernameNotesNotes(int recId, string username, string notes);

    [LoggerMessage(LogLevel.Information, "PR {recId} approved successfully by {username}")]
    partial void LogPrRecIdApprovedSuccessfullyByUsername(int recId, string username);

    [LoggerMessage(LogLevel.Information, "Rejecting PR {recId} by user: {username}. Reason: {notes}")]
    partial void LogRejectingPrRecIdByUserUsernameReasonNotes(int recId, string username, string notes);

    [LoggerMessage(LogLevel.Warning, "PR {recId} rejected by {username}")]
    partial void LogPrRecIdRejectedByUsername(int recId, string username);

    [LoggerMessage(LogLevel.Information, "Recalling PR {recId} by user: {username}")]
    partial void LogRecallingPrRecIdByUserUsername(int recId, string username);

    [LoggerMessage(LogLevel.Information, "PR {recId} recalled successfully by {username}")]
    partial void LogPrRecIdRecalledSuccessfullyByUsername(int recId, string username);

    public async Task<ServiceResponse<IEnumerable<WfTransHistoryVm>>> GetWfHistory(string refId, int wfFormId, CancellationToken cancellationToken)
    {
        LogRetrievingWfHistoryForPrRefIdRefId(refId);
        
        var data = await _prRepo.WfTransSelectByRefId(refId, wfFormId, cancellationToken);
        var vmList = data.Select(item => new WfTransHistoryVm
        {
            WfTransId = item.WfTransId,
            WfId = item.WfId,
            WfFormId = item.WfFormId,
            FormName = item.FormName,
            RefId = item.RefId,
            Index = item.Index,
            UserId = item.UserId,
            WfStatusActionId = item.WfStatusActionId,
            WfStatusActionName = item.WfStatusActionName,
            ActionDate = item.ActionDate,
            ActionDateStr = item.ActionDate.ToString("dd MMM yyyy HH:mm"),
            CreatedDate = item.CreatedDate,
            Notes = item.Notes
        }).ToList();
        
        LogWfHistoryRetrievedSuccessfullyCountCount(vmList.Count);
        return ServiceResponse<IEnumerable<WfTransHistoryVm>>.Success(vmList, "Workflow history retrieved successfully.");
    }

    [LoggerMessage(LogLevel.Debug, "Retrieving workflow history for PR RefId: {refId}")]
    partial void LogRetrievingWfHistoryForPrRefIdRefId(string refId);

    [LoggerMessage(LogLevel.Debug, "Workflow history retrieved successfully. Count: {count}")]
    partial void LogWfHistoryRetrievedSuccessfullyCountCount(int count);
}