using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class CustomerOrderService : ICustomerOrderService
{
    private readonly ICustomerOrderRepo _customerOrderRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<CustomerOrderService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public CustomerOrderService(ICustomerOrderRepo customerOrderRepo, IDbTransaction dbTransaction, 
        UserClaimDto userClaimDto, ILogger<CustomerOrderService> logger)
    {
        _customerOrderRepo = customerOrderRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PagedListVm<CustomerOrderHeaderVm>>> CoSelect(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogRetrievingCoListPagePageSize(request.PageNumber, request.PageSize);
        var cos = await _customerOrderRepo.CoSelect(_mapper.MapToEntity(request), cancellationToken);
        LogRetrievedCountCos(cos.TotalCount);
        return ServiceResponse<PagedListVm<CustomerOrderHeaderVm>>.Success(_mapper.MapToVm(cos), "Customer Orders retrieved successfully.");
    }

    public async Task<ServiceResponse<CustomerOrderHeaderVm>> CoSelectById(int id, CancellationToken cancellationToken)
    {
        LogRetrievingCoById(id);
        var co = await _customerOrderRepo.CoSelectById(id, cancellationToken);
        return ServiceResponse<CustomerOrderHeaderVm>.Success(_mapper.MapToVm(co), "Customer Order retrieved successfully.");
    }

    public async Task<ServiceResponse<(int RecId, string CoId)>> CoInsert(CustomerOrderVm customerOrderHeader,
        CancellationToken cancellationToken)
    {
        LogCreatingNewCoByUser(_userClaimDto.Username);
        var entity = _mapper.MapToEntity(customerOrderHeader);
        entity.Header.CreatedBy = _userClaimDto.Username;
        var result = await _customerOrderRepo.CoInsert(entity.Header, cancellationToken);

        foreach (var line in entity.Lines)
        {
            line.CoRecId = result.RecId;
            line.CreatedBy = _userClaimDto.Username;
            await _customerOrderRepo.CoLineInsert(line, cancellationToken);
        }
        
        _dbTransaction.Commit();
        LogCoCreatedSuccessfully(result.RecId, result.CoId);
        return ServiceResponse<(int RecId, string CoId)>.Success(result, "Customer Order created successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> CoUpdate(CustomerOrderHeaderVm customerOrderHeader, CancellationToken cancellationToken)
    {
        LogUpdatingCoByUser(customerOrderHeader.RecId, customerOrderHeader.CoId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(customerOrderHeader);
        entity.ModifiedBy = _userClaimDto.Username;
        await _customerOrderRepo.CoUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogCoUpdatedSuccessfully(customerOrderHeader.RecId);
        return ServiceResponse.Success("Customer Order updated successfully.");
    }

    public async Task<ServiceResponse> CoDelete(int recId, CancellationToken cancellationToken)
    {
        LogDeletingCoByUser(recId, _userClaimDto.Username);
        await _customerOrderRepo.CoDelete(recId, cancellationToken);
        _dbTransaction.Commit();
        LogCoDeletedSuccessfully(recId);
        return ServiceResponse.Success("Customer Order deleted successfully.");
    }

    public async Task<ServiceResponse> CoSubmitById(CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken)
    {
        LogSubmittingCoByUser(action.RecId, _userClaimDto.Username);
        await _customerOrderRepo.CoSubmitById(action.RecId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogCoSubmittedSuccessfully(action.RecId);
        return ServiceResponse.Success("Customer Order submitted successfully.");
    }

    public async Task<ServiceResponse> CoRecallById(CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken)
    {
        LogRecallingCoByUser(action.RecId, _userClaimDto.Username);
        await _customerOrderRepo.CoRecallById(action.RecId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogCoRecalledSuccessfully(action.RecId);
        return ServiceResponse.Success("Customer Order recalled successfully.");
    }

    public async Task<ServiceResponse> CoRejectById(CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken)
    {
        LogRejectingCoByUser(action.RecId, _userClaimDto.Username, action.Notes ?? string.Empty);
        await _customerOrderRepo.CoRejectById(action.RecId, _userClaimDto.Username, action.Notes ?? string.Empty, cancellationToken);
        _dbTransaction.Commit();
        LogCoRejectedSuccessfully(action.RecId);
        return ServiceResponse.Success("Customer Order rejected successfully.");
    }

    public async Task<ServiceResponse> CoCloseByNo(CustomerOrderWorkflowActionVm action,
        CancellationToken cancellationToken)
    {
        LogClosingCoByUser(action.CoId ?? "N/A", _userClaimDto.Username);
        
        if (action.CoId == null)
        {
            return ServiceResponse.Failure("CoId cannot be null for closing a Customer Order.");
        }

        await _customerOrderRepo.CoCloseByNo(action.CoId, _userClaimDto.Username, cancellationToken);

        _dbTransaction.Commit();
        LogCoClosedSuccessfully(action.CoId);
        return ServiceResponse.Success("Customer Order closed successfully.");
    }
    
    public async Task<ServiceResponse> CoApproveById(CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken)
    {
        LogApprovingCoRecIdByUserUsername(action.RecId, _userClaimDto.Username);
        await _customerOrderRepo.CoApproveById(action.RecId, _userClaimDto.Username, action.Notes ?? string.Empty, cancellationToken);
        _dbTransaction.Commit();
        LogCoRecIdApprovedSuccessfully(action.RecId);
        return ServiceResponse.Success("Customer Order approved successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<CustomerOrderHeaderOutstandingVm>>> CoHdOsSelect(int customerId, CancellationToken cancellationToken)
    {
        var headers = await _customerOrderRepo.CoHdOsSelect(customerId, cancellationToken);
        return ServiceResponse<IEnumerable<CustomerOrderHeaderOutstandingVm>>.Success(_mapper.MapToVm(headers), "Outstanding CO headers retrieved successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<CustomerOrderOutstandingVm>>> CoOsSelect(int customerId, CancellationToken cancellationToken)
    {
        var lines = await _customerOrderRepo.CoOsSelect(customerId, cancellationToken);
        return ServiceResponse<IEnumerable<CustomerOrderOutstandingVm>>.Success(_mapper.MapToVm(lines), "Outstanding CO lines retrieved successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<CustomerOrderOutstandingVm>>> CoOsById(int coRecId, CancellationToken cancellationToken)
    {
        var lines = await _customerOrderRepo.CoOsById(coRecId, cancellationToken);
        var customerOrderLineOutstandings = lines as CustomerOrderLineOutstanding[] ?? lines.ToArray();
        return customerOrderLineOutstandings.Length == 0 ?
            ServiceResponse<IEnumerable<CustomerOrderOutstandingVm>>.NotFound("No outstanding CO lines found for the specified Customer Order.") : 
            ServiceResponse<IEnumerable<CustomerOrderOutstandingVm>>.Success(_mapper.MapToVm(customerOrderLineOutstandings), "Outstanding CO lines retrieved successfully.");
    }

    public async Task<ServiceResponse<CustomerOrderOutstandingVm>> CoOsByCoLineId(int coLineId, CancellationToken cancellationToken)
    {
        var line = await _customerOrderRepo.CoOsByCoLineId(coLineId, cancellationToken);
        return line == null ? 
            ServiceResponse<CustomerOrderOutstandingVm>.NotFound("Outstanding CO line not found for the specified CO line ID.") :
            ServiceResponse<CustomerOrderOutstandingVm>.Success(_mapper.MapToVm(line), "Outstanding CO line retrieved successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<CustomerOrderLineVm>>> CoLineSelectByCo(int coRecId, CancellationToken cancellationToken)
    {
        var lines = await _customerOrderRepo.CoLineSelectByCo(coRecId, cancellationToken);
        var customerOrderLines = lines as CustomerOrderLine[] ?? lines.ToArray();
        return customerOrderLines.Length == 0 ? 
            ServiceResponse<IEnumerable<CustomerOrderLineVm>>.NotFound("No CO lines found for the specified Customer Order.") : 
            ServiceResponse<IEnumerable<CustomerOrderLineVm>>.Success(_mapper.MapToVm(customerOrderLines), "CO lines retrieved successfully.");
    }

    public async Task<ServiceResponse<CustomerOrderLineVm>> CoLineSelectById(int coLineId, CancellationToken cancellationToken)
    {
        var line = await _customerOrderRepo.CoLineSelectById(coLineId, cancellationToken);
        return line == null ? 
            ServiceResponse<CustomerOrderLineVm>.NotFound("CO line not found for the specified CO line ID.") :
            ServiceResponse<CustomerOrderLineVm>.Success(_mapper.MapToVm(line), "CO line retrieved successfully.");
    }

    public async Task<ServiceResponse<string>> CoLineInsert(CustomerOrderLineVm customerOrderLine, CancellationToken cancellationToken)
    {
        LogInsertingCoLine(customerOrderLine.CoRecId, customerOrderLine.ItemId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(customerOrderLine);
        entity.CreatedBy = _userClaimDto.Username;
        var coId = await _customerOrderRepo.CoLineInsert(entity, cancellationToken);
        _dbTransaction.Commit();
        LogCoLineInsertedSuccessfully(coId);
        return ServiceResponse<string>.Success(coId, "CO line inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> CoLineUpdate(CustomerOrderLineVm customerOrderLine, CancellationToken cancellationToken)
    {
        LogUpdatingCoLine(customerOrderLine.CoLineId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(customerOrderLine);
        entity.ModifiedBy = _userClaimDto.Username;
        await _customerOrderRepo.CoLineUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogCoLineUpdatedSuccessfully(customerOrderLine.CoLineId);
        return ServiceResponse.NoContent("CO line updated successfully.");
    }

    public async Task<ServiceResponse> CoLineDelete(int coLineId, CancellationToken cancellationToken)
    {
        LogDeletingCoLine(coLineId, _userClaimDto.Username);
        await _customerOrderRepo.CoLineDelete(coLineId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogCoLineDeletedSuccessfully(coLineId);
        return ServiceResponse.Success("CO line deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<CustomerOrderTypeVm>>> CoTypeCbo(CancellationToken cancellationToken)
    {
        var types = await _customerOrderRepo.CoTypeCbo(cancellationToken);
        return ServiceResponse<IEnumerable<CustomerOrderTypeVm>>.Success(_mapper.MapToVm(types), "CO types retrieved successfully.");
    }

    public async Task<ServiceResponse<CustomerOrderVm>> GetCustomerOrder(int recId, CancellationToken cancellationToken)
    {
        var header = await _customerOrderRepo.CoSelectById(recId, cancellationToken);
        var lines = await _customerOrderRepo.CoLineSelectByCo(recId, cancellationToken);
        var outstanding = await _customerOrderRepo.CoOsSelect(header.CustomerId, cancellationToken);

        var viewModel = new CustomerOrderVm
        {
            Header = _mapper.MapToVm(header),
            Lines = _mapper.MapToVm(lines),
            Outstanding = _mapper.MapToVm(outstanding)
        };

        return ServiceResponse<CustomerOrderVm>.Success(viewModel, "Customer Order ViewModel retrieved successfully.");
    }

    // Logger methods
    [LoggerMessage(LogLevel.Debug, "Retrieving CO list - Page: {page}, PageSize: {pageSize}")]
    partial void LogRetrievingCoListPagePageSize(int page, int pageSize);

    [LoggerMessage(LogLevel.Debug, "Retrieved {count} customer orders")]
    partial void LogRetrievedCountCos(int count);

    [LoggerMessage(LogLevel.Debug, "Retrieving CO by RecId: {id}")]
    partial void LogRetrievingCoById(int id);

    [LoggerMessage(LogLevel.Information, "Creating new CO by user: {username}")]
    partial void LogCreatingNewCoByUser(string username);

    [LoggerMessage(LogLevel.Information, "CO created successfully with RecId: {recId}, CoId: {coId}")]
    partial void LogCoCreatedSuccessfully(int recId, string coId);

    [LoggerMessage(LogLevel.Information, "Updating CO {recId}: {coId} by user: {username}")]
    partial void LogUpdatingCoByUser(int recId, string coId, string username);

    [LoggerMessage(LogLevel.Information, "CO {recId} updated successfully")]
    partial void LogCoUpdatedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Deleting CO with RecId: {recId} by user: {username}")]
    partial void LogDeletingCoByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "CO {recId} deleted successfully")]
    partial void LogCoDeletedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Submitting CO {recId} by user: {username}")]
    partial void LogSubmittingCoByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "CO {recId} submitted successfully")]
    partial void LogCoSubmittedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Recalling CO {recId} by user: {username}")]
    partial void LogRecallingCoByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "CO {recId} recalled successfully")]
    partial void LogCoRecalledSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Rejecting CO {recId} by user: {username}. Notes: {notes}")]
    partial void LogRejectingCoByUser(int recId, string username, string notes);

    [LoggerMessage(LogLevel.Information, "CO {recId} rejected successfully")]
    partial void LogCoRejectedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Closing CO {coId} by user: {username}")]
    partial void LogClosingCoByUser(string coId, string username);

    [LoggerMessage(LogLevel.Information, "CO {coId} closed successfully")]
    partial void LogCoClosedSuccessfully(string coId);

    [LoggerMessage(LogLevel.Information, "Inserting CO line for CO {coRecId}, Item {itemId} by user: {username}")]
    partial void LogInsertingCoLine(int coRecId, int itemId, string username);

    [LoggerMessage(LogLevel.Information, "CO line {lineId} inserted successfully")]
    partial void LogCoLineInsertedSuccessfully(string lineId);

    [LoggerMessage(LogLevel.Information, "Updating CO line {lineId} by user: {username}")]
    partial void LogUpdatingCoLine(int lineId, string username);

    [LoggerMessage(LogLevel.Information, "CO line {lineId} updated successfully")]
    partial void LogCoLineUpdatedSuccessfully(int lineId);

    [LoggerMessage(LogLevel.Information, "Deleting CO line {lineId} by user: {username}")]
    partial void LogDeletingCoLine(int lineId, string username);

    [LoggerMessage(LogLevel.Information, "CO line {lineId} deleted successfully")]
    partial void LogCoLineDeletedSuccessfully(int lineId);

    [LoggerMessage(LogLevel.Information, "Approving CO {recId} by user: {username}")]
    partial void LogApprovingCoRecIdByUserUsername(int recId, string username);

    [LoggerMessage(LogLevel.Information, "CO {recId} approved successfully")]
    partial void LogCoRecIdApprovedSuccessfully(int recId);
    
    [LoggerMessage(LogLevel.Information, "Generating CO Summary Report from {startDate} to {endDate} by user: {username}")]
    partial void LogGeneratingCoSummaryReport(DateTime startDate, DateTime endDate, string username);
    
    [LoggerMessage(LogLevel.Information, "CO Summary Report data retrieved successfully with {itemCount} items")]
    partial void LogCoSummaryReportDataRetrieved(int itemCount);

    public async Task<IEnumerable<CoSummaryReportItem>> GetCoSummaryReportData(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        LogGeneratingCoSummaryReport(startDate, endDate, _userClaimDto.Username);
        
        var data = await _customerOrderRepo.CoRptSummary(startDate, endDate, cancellationToken);
        var dataList = data.ToList();
        
        LogCoSummaryReportDataRetrieved(dataList.Count);
        
        return dataList;
    }
    
    [LoggerMessage(LogLevel.Information, "Generating CO Detail Report from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd} by user {username}")]
    partial void LogGeneratingCoDetailReport(DateTime startDate, DateTime endDate, string username);
    
    [LoggerMessage(LogLevel.Information, "CO Detail Report data retrieved successfully with {itemCount} items")]
    partial void LogCoDetailReportDataRetrieved(int itemCount);
    
    public async Task<IEnumerable<CoDetailReportItem>> GetCoDetailReportData(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        LogGeneratingCoDetailReport(startDate, endDate, _userClaimDto.Username);
        
        var data = await _customerOrderRepo.CoRptDetail(startDate, endDate, cancellationToken);
        var dataList = data.ToList();
        
        LogCoDetailReportDataRetrieved(dataList.Count);
        
        return dataList;
    }
    
    [LoggerMessage(LogLevel.Information, "Exporting CO data by user {username}")]
    partial void LogExportingCoData(string username);
    
    [LoggerMessage(LogLevel.Information, "CO export data retrieved successfully with {itemCount} items")]
    partial void LogCoExportDataRetrieved(int itemCount);
    
    public async Task<IEnumerable<CustomerOrderHeader>> GetCoExportData(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogExportingCoData(_userClaimDto.Username);
        
        var data = await _customerOrderRepo.CoSelectForExport(_mapper.MapToEntity(request), cancellationToken);
        var dataList = data.ToList();
        
        LogCoExportDataRetrieved(dataList.Count);
        
        return dataList;
    }
    
    public async Task<ServiceResponse<IEnumerable<WfTransHistoryVm>>> GetWfHistory(string refId, int wfFormId, CancellationToken cancellationToken)
    {
        var data = await _customerOrderRepo.WfTransSelectByRefId(refId, wfFormId, cancellationToken);
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
        
        return ServiceResponse<IEnumerable<WfTransHistoryVm>>.Success(vmList, "Workflow history retrieved successfully.");
    }
}
