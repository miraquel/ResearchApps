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

public partial class GoodsReceiptService : IGoodsReceiptService
{
    private readonly IGoodsReceiptRepo _goodsReceiptRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<GoodsReceiptService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public GoodsReceiptService(IGoodsReceiptRepo goodsReceiptRepo, IDbTransaction dbTransaction,
        UserClaimDto userClaimDto, ILogger<GoodsReceiptService> logger)
    {
        _goodsReceiptRepo = goodsReceiptRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PagedListVm<GoodsReceiptHeaderVm>>> GrSelect(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogRetrievingGrListPagePageSize(request.PageNumber, request.PageSize);
        var grs = await _goodsReceiptRepo.GrSelect(_mapper.MapToEntity(request), cancellationToken);
        LogRetrievedCountGrs(grs.TotalCount);
        return ServiceResponse<PagedListVm<GoodsReceiptHeaderVm>>.Success(_mapper.MapToVm(grs), "Goods Receipts retrieved successfully.");
    }

    public async Task<ServiceResponse<GoodsReceiptHeaderVm>> GrSelectById(int id, CancellationToken cancellationToken)
    {
        LogRetrievingGrById(id);
        var gr = await _goodsReceiptRepo.GrSelectById(id, cancellationToken);
        return ServiceResponse<GoodsReceiptHeaderVm>.Success(_mapper.MapToVm(gr), "Goods Receipt retrieved successfully.");
    }

    public async Task<ServiceResponse<int>> GrInsert(GoodsReceiptVm goodsReceipt, CancellationToken cancellationToken)
    {
        LogCreatingNewGrByUser(_userClaimDto.Username);
        var entity = _mapper.MapToEntity(goodsReceipt);
        entity.Header.CreatedBy = _userClaimDto.Username;
        var recId = await _goodsReceiptRepo.GrInsert(entity.Header, cancellationToken);

        foreach (var line in entity.Lines)
        {
            line.GrRecId = recId;
            line.CreatedBy = _userClaimDto.Username;
            await _goodsReceiptRepo.GrLineInsert(line, cancellationToken);
        }

        _dbTransaction.Commit();
        LogGrCreatedSuccessfully(recId);
        return ServiceResponse<int>.Success(recId, "Goods Receipt created successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> GrUpdate(GoodsReceiptHeaderVm goodsReceiptHeader, CancellationToken cancellationToken)
    {
        LogUpdatingGrByUser(goodsReceiptHeader.RecId, goodsReceiptHeader.GrId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(goodsReceiptHeader);
        entity.ModifiedBy = _userClaimDto.Username;
        await _goodsReceiptRepo.GrUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogGrUpdatedSuccessfully(goodsReceiptHeader.RecId);
        return ServiceResponse.Success("Goods Receipt updated successfully.");
    }

    public async Task<ServiceResponse> GrDelete(int recId, CancellationToken cancellationToken)
    {
        LogDeletingGrByUser(recId, _userClaimDto.Username);
        await _goodsReceiptRepo.GrDelete(recId, cancellationToken);
        _dbTransaction.Commit();
        LogGrDeletedSuccessfully(recId);
        return ServiceResponse.Success("Goods Receipt deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<GoodsReceiptLineVm>>> GrLineSelectByGr(int grRecId, CancellationToken cancellationToken)
    {
        var lines = await _goodsReceiptRepo.GrLineSelectByGr(grRecId, cancellationToken);
        return ServiceResponse<IEnumerable<GoodsReceiptLineVm>>.Success(_mapper.MapToVm(lines), "Goods Receipt lines retrieved successfully.");
    }

    public async Task<ServiceResponse<GoodsReceiptLineVm>> GrLineSelectById(int grLineId, CancellationToken cancellationToken)
    {
        var line = await _goodsReceiptRepo.GrLineSelectById(grLineId, cancellationToken);
        if (line == null)
        {
            return ServiceResponse<GoodsReceiptLineVm>.Failure("Goods Receipt line not found.", StatusCodes.Status404NotFound);
        }
        return ServiceResponse<GoodsReceiptLineVm>.Success(_mapper.MapToVm(line), "Goods Receipt line retrieved successfully.");
    }

    public async Task<ServiceResponse> GrLineInsert(GoodsReceiptLineVm goodsReceiptLine, CancellationToken cancellationToken)
    {
        LogInsertingGrLineByUser(goodsReceiptLine.GrRecId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(goodsReceiptLine);
        entity.CreatedBy = _userClaimDto.Username;
        var result = await _goodsReceiptRepo.GrLineInsert(entity, cancellationToken);
        _dbTransaction.Commit();
        LogGrLineInsertedSuccessfully(result);
        return ServiceResponse.Created("Goods Receipt line created successfully.");
    }

    public async Task<ServiceResponse> GrLineUpdate(GoodsReceiptLineVm goodsReceiptLine, CancellationToken cancellationToken)
    {
        LogUpdatingGrLineByUser(goodsReceiptLine.GrLineId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(goodsReceiptLine);
        entity.ModifiedBy = _userClaimDto.Username;
        await _goodsReceiptRepo.GrLineUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogGrLineUpdatedSuccessfully(goodsReceiptLine.GrLineId);
        return ServiceResponse.Success("Goods Receipt line updated successfully.");
    }

    public async Task<ServiceResponse> GrLineDelete(int grLineId, CancellationToken cancellationToken)
    {
        LogDeletingGrLineByUser(grLineId, _userClaimDto.Username);
        await _goodsReceiptRepo.GrLineDelete(grLineId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogGrLineDeletedSuccessfully(grLineId);
        return ServiceResponse.Success("Goods Receipt line deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<PoLineOutstandingVm>>> PoOsSelectBySupplier(int supplierId, CancellationToken cancellationToken)
    {
        var lines = await _goodsReceiptRepo.PoOsSelectBySupplier(supplierId, cancellationToken);
        return ServiceResponse<IEnumerable<PoLineOutstandingVm>>.Success(_mapper.MapToVm(lines), "Outstanding PO lines retrieved successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<PoLineOutstandingVm>>> PoOsSelectById(int poLineId, CancellationToken cancellationToken)
    {
        var line = await _goodsReceiptRepo.PoOsSelectById(poLineId, cancellationToken);
        var result = line != null ? new[] { _mapper.MapToVm(line) } : Array.Empty<PoLineOutstandingVm>();
        return ServiceResponse<IEnumerable<PoLineOutstandingVm>>.Success(result, "Outstanding PO line retrieved successfully.");
    }

    public async Task<ServiceResponse<GoodsReceiptVm>> GetGoodsReceipt(int recId, CancellationToken cancellationToken)
    {
        LogRetrievingGrById(recId);
        var header = await _goodsReceiptRepo.GrSelectById(recId, cancellationToken);
        var lines = await _goodsReceiptRepo.GrLineSelectByGr(recId, cancellationToken);

        var vm = new GoodsReceiptVm
        {
            Header = _mapper.MapToVm(header),
            Lines = _mapper.MapToVm(lines)
        };

        return ServiceResponse<GoodsReceiptVm>.Success(vm, "Goods Receipt retrieved successfully.");
    }

    public async Task<IEnumerable<GrReportItem>> GetGrReportData(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        LogGeneratingGrReport(startDate, endDate, _userClaimDto.Username);
        
        var data = await _goodsReceiptRepo.GrRpt(startDate, endDate, cancellationToken);
        var dataList = data.ToList();
        
        LogGrReportDataRetrieved(dataList.Count);
        
        return dataList;
    }

    public async Task<IEnumerable<GoodsReceiptHeader>> GetGrExportData(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogExportingGrData(_userClaimDto.Username);
        
        var data = await _goodsReceiptRepo.GrSelectForExport(_mapper.MapToEntity(request), cancellationToken);
        var dataList = data.ToList();
        
        LogGrExportDataRetrieved(dataList.Count);
        
        return dataList;
    }

    #region Logging

    [LoggerMessage(LogLevel.Information, "Retrieving GR list - Page: {page}, PageSize: {pageSize}")]
    partial void LogRetrievingGrListPagePageSize(int page, int pageSize);

    [LoggerMessage(LogLevel.Information, "Retrieved {count} GRs")]
    partial void LogRetrievedCountGrs(int count);

    [LoggerMessage(LogLevel.Information, "Retrieving GR by ID: {id}")]
    partial void LogRetrievingGrById(int id);

    [LoggerMessage(LogLevel.Information, "Creating new GR by user: {username}")]
    partial void LogCreatingNewGrByUser(string username);

    [LoggerMessage(LogLevel.Information, "GR created successfully - RecId: {recId}")]
    partial void LogGrCreatedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Updating GR {recId} ({grId}) by user: {username}")]
    partial void LogUpdatingGrByUser(int recId, string grId, string username);

    [LoggerMessage(LogLevel.Information, "GR {recId} updated successfully")]
    partial void LogGrUpdatedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Deleting GR {recId} by user: {username}")]
    partial void LogDeletingGrByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "GR {recId} deleted successfully")]
    partial void LogGrDeletedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Inserting GR line for GR {grRecId} by user: {username}")]
    partial void LogInsertingGrLineByUser(int grRecId, string username);

    [LoggerMessage(LogLevel.Information, "GR line {grLineId} inserted successfully")]
    partial void LogGrLineInsertedSuccessfully(int grLineId);

    [LoggerMessage(LogLevel.Information, "Updating GR line {grLineId} by user: {username}")]
    partial void LogUpdatingGrLineByUser(int grLineId, string username);

    [LoggerMessage(LogLevel.Information, "GR line {grLineId} updated successfully")]
    partial void LogGrLineUpdatedSuccessfully(int grLineId);

    [LoggerMessage(LogLevel.Information, "Deleting GR line {grLineId} by user: {username}")]
    partial void LogDeletingGrLineByUser(int grLineId, string username);

    [LoggerMessage(LogLevel.Information, "GR line {grLineId} deleted successfully")]
    partial void LogGrLineDeletedSuccessfully(int grLineId);

    [LoggerMessage(LogLevel.Information, "Generating GR Report from {startDate} to {endDate} by user: {username}")]
    partial void LogGeneratingGrReport(DateTime startDate, DateTime endDate, string username);

    [LoggerMessage(LogLevel.Information, "GR Report data retrieved successfully with {itemCount} items")]
    partial void LogGrReportDataRetrieved(int itemCount);

    [LoggerMessage(LogLevel.Information, "Exporting GR data by user {username}")]
    partial void LogExportingGrData(string username);

    [LoggerMessage(LogLevel.Information, "GR export data retrieved successfully with {itemCount} items")]
    partial void LogGrExportDataRetrieved(int itemCount);

    #endregion
}
