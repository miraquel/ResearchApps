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

public partial class DeliveryOrderService : IDeliveryOrderService
{
    private readonly IDeliveryOrderRepo _deliveryOrderRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<DeliveryOrderService> _logger;
    private readonly IItemRepo _itemRepo;
    private readonly MapperlyMapper _mapper = new();

    public DeliveryOrderService(IDeliveryOrderRepo deliveryOrderRepo, IDbTransaction dbTransaction, 
        UserClaimDto userClaimDto, ILogger<DeliveryOrderService> logger, IItemRepo itemRepo)
    {
        _deliveryOrderRepo = deliveryOrderRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
        _itemRepo = itemRepo;
    }

    public async Task<ServiceResponse<PagedListVm<DeliveryOrderHeaderVm>>> DoSelect(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogRetrievingDoListPagePagesize(request.PageNumber, request.PageSize);
        var dos = await _deliveryOrderRepo.DoSelect(_mapper.MapToEntity(request), cancellationToken);
        LogRetrievedCountDos(dos.TotalCount);
        return ServiceResponse<PagedListVm<DeliveryOrderHeaderVm>>.Success(_mapper.MapToVm(dos), "Delivery Orders retrieved successfully.");
    }

    public async Task<ServiceResponse<DeliveryOrderHeaderVm>> DoSelectById(int id, CancellationToken cancellationToken)
    {
        LogRetrievingDoById(id);
        var deliveryOrder = await _deliveryOrderRepo.DoSelectById(id, cancellationToken);
        return ServiceResponse<DeliveryOrderHeaderVm>.Success(_mapper.MapToVm(deliveryOrder), "Delivery Order retrieved successfully.");
    }

    public async Task<ServiceResponse<(int RecId, string DoId)>> DoInsert(DeliveryOrderVm deliveryOrderHeader, CancellationToken cancellationToken)
    {
        LogCreatingNewDoByUser(_userClaimDto.Username);
        var entity = _mapper.MapToEntity(deliveryOrderHeader);
        entity.Header.CreatedBy = _userClaimDto.Username;
        var headerResult = await _deliveryOrderRepo.DoInsert(entity.Header, cancellationToken);
        
        // Set required fields for each line
        foreach (var line in entity.Lines)
        {
            line.CreatedBy = _userClaimDto.Username;
            line.DoRecId = headerResult.RecId; // Set the foreign key
            line.DoId = headerResult.DoId; // Set DO ID
            line.CustomerId = entity.Header.CustomerId; // Set customer from header
            line.WhId = 1; // Default warehouse - adjust as needed
            
            // If CoId is not set, try to get it from the header
            if (string.IsNullOrEmpty(line.CoId))
            {
                line.CoId = entity.Header.CoId;
            }
            
            _ = await _deliveryOrderRepo.DoLineInsert(line, cancellationToken);
        }
        var result = (headerResult.RecId, headerResult.DoId);
        _dbTransaction.Commit();
        LogDoCreatedSuccessfully(result.RecId, result.DoId);
        return ServiceResponse<(int RecId, string DoId)>.Success(result, "Delivery Order created successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> DoUpdate(DeliveryOrderHeaderVm deliveryOrderHeader, CancellationToken cancellationToken)
    {
        LogUpdatingDoByUser(deliveryOrderHeader.RecId, deliveryOrderHeader.DoId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(deliveryOrderHeader);
        entity.ModifiedBy = _userClaimDto.Username;
        entity.ModifiedDate = DateTime.Now;
        await _deliveryOrderRepo.DoUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogDoUpdatedSuccessfully(deliveryOrderHeader.RecId);
        return ServiceResponse.Success("Delivery Order updated successfully.");
    }

    public async Task<ServiceResponse> DoDelete(int recId, CancellationToken cancellationToken)
    {
        LogDeletingDoByUser(recId, _userClaimDto.Username);
        await _deliveryOrderRepo.DoDelete(recId, cancellationToken);
        _dbTransaction.Commit();
        LogDoDeletedSuccessfully(recId);
        return ServiceResponse.Success("Delivery Order deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<DeliveryOrderHeaderOutstandingVm>>> DoHdOsSelect(int customerId, CancellationToken cancellationToken)
    {
        var headers = await _deliveryOrderRepo.DoHdOsSelect(customerId, cancellationToken);
        return ServiceResponse<IEnumerable<DeliveryOrderHeaderOutstandingVm>>.Success(_mapper.MapToVm(headers), "Outstanding DO headers retrieved successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<DeliveryOrderOutstandingVm>>> DoOsSelect(int customerId, CancellationToken cancellationToken)
    {
        var lines = await _deliveryOrderRepo.DoOsSelect(customerId, cancellationToken);
        return ServiceResponse<IEnumerable<DeliveryOrderOutstandingVm>>.Success(_mapper.MapToVm(lines), "Outstanding DO lines retrieved successfully.");
    }

    public async Task<ServiceResponse<DeliveryOrderOutstandingVm>> DoOsByDoLineId(int doLineId, CancellationToken cancellationToken)
    {
        var line = await _deliveryOrderRepo.DoOsByDoLineId(doLineId, cancellationToken);
        return ServiceResponse<DeliveryOrderOutstandingVm>.Success(_mapper.MapToVm(line), "Outstanding DO line retrieved successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<DeliveryOrderLineVm>>> DoLineSelectByDo(int doRecId, CancellationToken cancellationToken)
    {
        var lines = await _deliveryOrderRepo.DoLineSelectByDo(doRecId, cancellationToken);
        return ServiceResponse<IEnumerable<DeliveryOrderLineVm>>.Success(_mapper.MapToVm(lines), "DO lines retrieved successfully.");
    }

    public async Task<ServiceResponse<DeliveryOrderLineVm>> DoLineSelectById(int doLineId, CancellationToken cancellationToken)
    {
        var line = await _deliveryOrderRepo.DoLineSelectById(doLineId, cancellationToken);
        return ServiceResponse<DeliveryOrderLineVm>.Success(_mapper.MapToVm(line), "DO line retrieved successfully.");
    }

    public async Task<ServiceResponse<string>> DoLineInsert(DeliveryOrderLineVm deliveryOrderLine, CancellationToken cancellationToken)
    {
        LogInsertingDoLine(deliveryOrderLine.DoRecId, deliveryOrderLine.ItemId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(deliveryOrderLine);
        entity.CreatedBy = _userClaimDto.Username;
        var doId = await _deliveryOrderRepo.DoLineInsert(entity, cancellationToken);
        _dbTransaction.Commit();
        LogDoLineInsertedSuccessfully(doId);
        return ServiceResponse<string>.Success(doId, "DO line inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> DoLineUpdate(DeliveryOrderLineVm deliveryOrderLine, CancellationToken cancellationToken)
    {
        LogUpdatingDoLine(deliveryOrderLine.DoLineId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(deliveryOrderLine);
        entity.ModifiedBy = _userClaimDto.Username;
        await _deliveryOrderRepo.DoLineUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogDoLineUpdatedSuccessfully(deliveryOrderLine.DoLineId);
        return ServiceResponse.Success("DO line updated successfully.");
    }

    public async Task<ServiceResponse> DoLineDelete(int doLineId, CancellationToken cancellationToken)
    {
        LogDeletingDoLine(doLineId, _userClaimDto.Username);
        await _deliveryOrderRepo.DoLineDelete(doLineId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogDoLineDeletedSuccessfully(doLineId);
        return ServiceResponse.Success("DO line deleted successfully.");
    }

    public async Task<ServiceResponse<DeliveryOrderVm>> GetDeliveryOrderViewModel(int recId, CancellationToken cancellationToken)
    {
        // Single round trip to get all data
        var viewModel = _mapper.MapToVm(await _deliveryOrderRepo.DoSelectCompositeById(recId, cancellationToken));
        return ServiceResponse<DeliveryOrderVm>.Success(viewModel, "Delivery Order ViewModel retrieved successfully.");
    }

    // Logger methods
    [LoggerMessage(LogLevel.Debug, "Retrieving DO list - Page: {page}, PageSize: {pageSize}")]
    partial void LogRetrievingDoListPagePagesize(int page, int pageSize);

    [LoggerMessage(LogLevel.Debug, "Retrieved {count} delivery orders")]
    partial void LogRetrievedCountDos(int count);

    [LoggerMessage(LogLevel.Debug, "Retrieving DO by RecId: {id}")]
    partial void LogRetrievingDoById(int id);

    [LoggerMessage(LogLevel.Information, "Creating new DO by user: {username}")]
    partial void LogCreatingNewDoByUser(string username);

    [LoggerMessage(LogLevel.Information, "DO created successfully with RecId: {recId}, DoId: {doId}")]
    partial void LogDoCreatedSuccessfully(int recId, string doId);

    [LoggerMessage(LogLevel.Information, "Updating DO {recId}: {doId} by user: {username}")]
    partial void LogUpdatingDoByUser(int recId, string doId, string username);

    [LoggerMessage(LogLevel.Information, "DO {recId} updated successfully")]
    partial void LogDoUpdatedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Deleting DO with RecId: {recId} by user: {username}")]
    partial void LogDeletingDoByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "DO {recId} deleted successfully")]
    partial void LogDoDeletedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Inserting DO line for DO {doRecId}, Item {itemId} by user: {username}")]
    partial void LogInsertingDoLine(int doRecId, int itemId, string username);

    [LoggerMessage(LogLevel.Information, "DO line {doId} inserted successfully")]
    partial void LogDoLineInsertedSuccessfully(string doId);

    [LoggerMessage(LogLevel.Information, "Updating DO line {lineId} by user: {username}")]
    partial void LogUpdatingDoLine(int lineId, string username);

    [LoggerMessage(LogLevel.Information, "DO line {lineId} updated successfully")]
    partial void LogDoLineUpdatedSuccessfully(int lineId);

    [LoggerMessage(LogLevel.Information, "Deleting DO line {lineId} by user: {username}")]
    partial void LogDeletingDoLine(int lineId, string username);

    [LoggerMessage(LogLevel.Information, "DO line {lineId} deleted successfully")]
    partial void LogDoLineDeletedSuccessfully(int lineId);
    
    [LoggerMessage(LogLevel.Information, "Exporting DO data by user {username}")]
    partial void LogExportingDoData(string username);
    
    [LoggerMessage(LogLevel.Information, "DO export data retrieved successfully with {itemCount} items")]
    partial void LogDoExportDataRetrieved(int itemCount);
    
    public async Task<IEnumerable<DeliveryOrderHeader>> GetDoExportData(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogExportingDoData(_userClaimDto.Username);
        
        var data = await _deliveryOrderRepo.DoSelectForExport(_mapper.MapToEntity(request), cancellationToken);
        var dataList = data.ToList();
        
        LogDoExportDataRetrieved(dataList.Count);
        
        return dataList;
    }
    
    public async Task<ServiceResponse<IEnumerable<WfTransHistoryVm>>> GetWfHistory(string refId, int wfFormId, CancellationToken cancellationToken)
    {
        var data = await _deliveryOrderRepo.WfTransSelectByRefId(refId, wfFormId, cancellationToken);
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
