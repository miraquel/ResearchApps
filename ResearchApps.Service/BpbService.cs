using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class BpbService : IBpbService
{
    private readonly IBpbRepo _bpbRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<BpbService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public BpbService(IBpbRepo bpbRepo, IDbTransaction dbTransaction,
        UserClaimDto userClaimDto, ILogger<BpbService> logger)
    {
        _bpbRepo = bpbRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PagedListVm<BpbHeaderVm>>> BpbSelect(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogRetrievingBpbListPagePageSize(request.PageNumber, request.PageSize);
        var bpbs = await _bpbRepo.BpbSelect(_mapper.MapToEntity(request), cancellationToken);
        LogRetrievedCountBpbs(bpbs.TotalCount);
        return ServiceResponse<PagedListVm<BpbHeaderVm>>.Success(_mapper.MapToVm(bpbs), "BPBs retrieved successfully.");
    }

    public async Task<ServiceResponse<BpbHeaderVm>> BpbSelectById(int id, CancellationToken cancellationToken)
    {
        LogRetrievingBpbById(id);
        var bpb = await _bpbRepo.BpbSelectById(id, cancellationToken);
        return bpb == null ? ServiceResponse<BpbHeaderVm>.Failure("BPB not found.", StatusCodes.Status404NotFound) : ServiceResponse<BpbHeaderVm>.Success(_mapper.MapToVm(bpb), "BPB retrieved successfully.");
    }

    public async Task<ServiceResponse<BpbVm>> GetBpb(int recId, CancellationToken cancellationToken)
    {
        LogRetrievingBpbById(recId);
        var header = await _bpbRepo.BpbSelectById(recId, cancellationToken);
        if (header == null)
        {
            return ServiceResponse<BpbVm>.Failure("BPB not found.", StatusCodes.Status404NotFound);
        }
        
        var lines = await _bpbRepo.BpbLineSelectByBpb(recId, cancellationToken);

        var vm = new BpbVm
        {
            Header = _mapper.MapToVm(header),
            Lines = _mapper.MapToVm(lines)
        };

        return ServiceResponse<BpbVm>.Success(vm, "BPB retrieved successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<BpbHeaderVm>>> BpbSelectByProd(string prodId, CancellationToken cancellationToken)
    {
        LogRetrievingBpbByProdId(prodId);
        var bpbs = await _bpbRepo.BpbSelectByProd(prodId, cancellationToken);
        return ServiceResponse<IEnumerable<BpbHeaderVm>>.Success(_mapper.MapToVm(bpbs), "BPBs retrieved successfully.");
    }

    public async Task<ServiceResponse<int>> BpbInsert(BpbHeaderVm bpbHeader, CancellationToken cancellationToken)
    {
        LogCreatingNewBpbByUser(_userClaimDto.Username);

        var entity = _mapper.MapToEntity(bpbHeader);
        entity.CreatedBy = _userClaimDto.Username;
        
        var recId = await _bpbRepo.BpbInsert(entity, cancellationToken);

        _dbTransaction.Commit();
        LogBpbCreatedSuccessfully(recId);
        return ServiceResponse<int>.Success(recId, $"BPB with RecId {recId} created successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> BpbUpdate(BpbHeaderVm bpbHeader, CancellationToken cancellationToken)
    {
        LogUpdatingBpbByUser(bpbHeader.RecId, bpbHeader.BpbId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(bpbHeader);
        entity.ModifiedBy = _userClaimDto.Username;
        await _bpbRepo.BpbUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogBpbUpdatedSuccessfully(bpbHeader.RecId);
        return ServiceResponse.Success("BPB updated successfully.");
    }

    public async Task<ServiceResponse> BpbDelete(int recId, CancellationToken cancellationToken)
    {
        LogDeletingBpbByUser(recId, _userClaimDto.Username);
        await _bpbRepo.BpbDelete(recId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogBpbDeletedSuccessfully(recId);
        return ServiceResponse.Success("BPB deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<BpbLineVm>>> BpbLineSelectByBpb(int bpbRecId, CancellationToken cancellationToken)
    {
        var lines = await _bpbRepo.BpbLineSelectByBpb(bpbRecId, cancellationToken);
        return ServiceResponse<IEnumerable<BpbLineVm>>.Success(_mapper.MapToVm(lines), "BPB lines retrieved successfully.");
    }

    public async Task<ServiceResponse<BpbLineVm>> BpbLineSelectById(int bpbLineId, CancellationToken cancellationToken)
    {
        var line = await _bpbRepo.BpbLineSelectById(bpbLineId, cancellationToken);
        return line == null ? ServiceResponse<BpbLineVm>.Failure("BPB line not found.", StatusCodes.Status404NotFound) : ServiceResponse<BpbLineVm>.Success(_mapper.MapToVm(line), "BPB line retrieved successfully.");
    }

    public async Task<ServiceResponse<int>> BpbLineInsert(BpbLineVm bpbLine, CancellationToken cancellationToken)
    {
        LogInsertingBpbLineByUser(bpbLine.BpbRecId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(bpbLine);
        entity.CreatedBy = _userClaimDto.Username;
        
        var id = await _bpbRepo.BpbLineInsert(entity, cancellationToken);
        
        _dbTransaction.Commit();
        LogBpbLineInsertedSuccessfully(id);
        return ServiceResponse<int>.Success(id, "BPB line inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<int>> BpbLineUpdate(BpbLineVm bpbLine, CancellationToken cancellationToken)
    {
        LogUpdatingBpbLineByUser(bpbLine.BpbLineId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(bpbLine);
        entity.ModifiedBy = _userClaimDto.Username;
        await _bpbRepo.BpbLineUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogBpbLineUpdatedSuccessfully(bpbLine.BpbLineId);
        return ServiceResponse<int>.Success(bpbLine.BpbLineId, "BPB line updated successfully.");
    }

    public async Task<ServiceResponse<int>> BpbLineDelete(int bpbLineId, CancellationToken cancellationToken)
    {
        LogDeletingBpbLineByUser(bpbLineId, _userClaimDto.Username);
        await _bpbRepo.BpbLineDelete(bpbLineId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogBpbLineDeletedSuccessfully(bpbLineId);
        return ServiceResponse<int>.Success(bpbLineId, "BPB line deleted successfully.");
    }

    public async Task<ServiceResponse<StockCheckVm>> CheckStock(int itemId, int whId, decimal requestedQty, CancellationToken cancellationToken)
    {
        var (onHand, bufferStock) = await _bpbRepo.GetStockInfo(itemId, whId, cancellationToken);
        
        var result = new StockCheckVm
        {
            ItemId = itemId,
            WhId = whId,
            OnHand = onHand,
            BufferStock = bufferStock,
            RequestedQty = requestedQty
        };

        if (!result.IsAvailable)
        {
            result.Message = $"Insufficient stock. Available: {onHand:N2}, Requested: {requestedQty:N2}";
            return ServiceResponse<StockCheckVm>.Failure(result.Message);
        }

        if (result.WillBeBelowBuffer)
        {
            result.Message = $"Warning: Stock will be below buffer level. Current: {onHand:N2}, After: {(onHand - requestedQty):N2}, Buffer: {bufferStock:N2}";
        }
        else
        {
            result.Message = $"Stock available. Current: {onHand:N2}, After: {(onHand - requestedQty):N2}";
        }

        return ServiceResponse<StockCheckVm>.Success(result, result.Message);
    }

    // Source-generated logging
    [LoggerMessage(LogLevel.Information, "Retrieving BPB list, page {Page}, pageSize {PageSize}")]
    partial void LogRetrievingBpbListPagePageSize(int page, int pageSize);

    [LoggerMessage(LogLevel.Information, "Retrieved {Count} BPBs")]
    partial void LogRetrievedCountBpbs(int count);

    [LoggerMessage(LogLevel.Information, "Retrieving BPB by ID {Id}")]
    partial void LogRetrievingBpbById(int id);

    [LoggerMessage(LogLevel.Information, "Retrieving BPBs by ProdId {ProdId}")]
    partial void LogRetrievingBpbByProdId(string prodId);

    [LoggerMessage(LogLevel.Information, "Creating new BPB by user {Username}")]
    partial void LogCreatingNewBpbByUser(string username);

    [LoggerMessage(LogLevel.Information, "BPB {RecId} created successfully")]
    partial void LogBpbCreatedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Updating BPB {RecId} ({BpbId}) by user {Username}")]
    partial void LogUpdatingBpbByUser(int recId, string bpbId, string username);

    [LoggerMessage(LogLevel.Information, "BPB {RecId} updated successfully")]
    partial void LogBpbUpdatedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Deleting BPB {RecId} by user {Username}")]
    partial void LogDeletingBpbByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "BPB {RecId} deleted successfully")]
    partial void LogBpbDeletedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Inserting BPB line for BPB RecId {RecId} by user {Username}")]
    partial void LogInsertingBpbLineByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "BPB line inserted successfully: {BpbId}")]
    partial void LogBpbLineInsertedSuccessfully(int bpbId);

    [LoggerMessage(LogLevel.Information, "Updating BPB line {LineId} by user {Username}")]
    partial void LogUpdatingBpbLineByUser(int lineId, string username);

    [LoggerMessage(LogLevel.Information, "BPB line {LineId} updated successfully")]
    partial void LogBpbLineUpdatedSuccessfully(int lineId);

    [LoggerMessage(LogLevel.Information, "Deleting BPB line {LineId} by user {Username}")]
    partial void LogDeletingBpbLineByUser(int lineId, string username);

    [LoggerMessage(LogLevel.Information, "BPB line {LineId} deleted successfully")]
    partial void LogBpbLineDeletedSuccessfully(int lineId);
}
