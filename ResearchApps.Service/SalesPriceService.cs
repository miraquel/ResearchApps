using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class SalesPriceService : ISalesPriceService
{
    private readonly ISalesPriceRepo _salesPriceRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<SalesPriceService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public SalesPriceService(
        ISalesPriceRepo salesPriceRepo,
        IDbTransaction dbTransaction,
        UserClaimDto userClaimDto,
        ILogger<SalesPriceService> logger)
    {
        _salesPriceRepo = salesPriceRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PagedListVm<SalesPriceVm>>> SelectAsync(
        PagedListRequestVm listRequest,
        CancellationToken cancellationToken)
    {
        LogRetrievingSalesPriceList(listRequest.PageNumber, listRequest.PageSize);
        var salesPrices = await _salesPriceRepo.SelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        LogRetrievedSalesPrices(salesPrices.Items.Count());
        return ServiceResponse<PagedListVm<SalesPriceVm>>.Success(
            _mapper.MapToVm(salesPrices),
            "Sales Prices retrieved successfully.");
    }

    public async Task<ServiceResponse<SalesPriceVm>> SelectByIdAsync(
        int recId,
        CancellationToken cancellationToken)
    {
        LogRetrievingSalesPriceById(recId);
        var salesPrice = await _salesPriceRepo.SelectByIdAsync(recId, cancellationToken);
        return ServiceResponse<SalesPriceVm>.Success(
            _mapper.MapToVm(salesPrice),
            "Sales Price retrieved successfully.");
    }

    public async Task<ServiceResponse<SalesPriceVm>> InsertAsync(
        SalesPriceVm salesPriceVm,
        CancellationToken cancellationToken)
    {
        LogCreatingSalesPrice(_userClaimDto.Username);
        var entity = _mapper.MapToEntity(salesPriceVm);
        entity.CreatedBy = _userClaimDto.Username;
        var inserted = await _salesPriceRepo.InsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogSalesPriceCreated(inserted.RecId);
        return ServiceResponse<SalesPriceVm>.Success(
            _mapper.MapToVm(inserted),
            "Sales Price created successfully.",
            StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<SalesPriceVm>> UpdateAsync(
        SalesPriceVm salesPriceVm,
        CancellationToken cancellationToken)
    {
        LogUpdatingSalesPrice(salesPriceVm.RecId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(salesPriceVm);
        entity.ModifiedBy = _userClaimDto.Username;
        var updated = await _salesPriceRepo.UpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogSalesPriceUpdated(salesPriceVm.RecId);
        return ServiceResponse<SalesPriceVm>.Success(
            _mapper.MapToVm(updated),
            "Sales Price updated successfully.");
    }

    public async Task<ServiceResponse> DeleteAsync(int recId, CancellationToken cancellationToken)
    {
        LogDeletingSalesPrice(recId, _userClaimDto.Username);
        await _salesPriceRepo.DeleteAsync(recId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogSalesPriceDeleted(recId);
        return ServiceResponse.Success("Sales Price deleted successfully.");
    }

    [LoggerMessage(LogLevel.Debug, "Retrieving Sales Prices list - Page: {page}, PageSize: {pageSize}")]
    partial void LogRetrievingSalesPriceList(int page, int pageSize);

    [LoggerMessage(LogLevel.Debug, "Retrieved {count} Sales Prices")]
    partial void LogRetrievedSalesPrices(int count);

    [LoggerMessage(LogLevel.Debug, "Retrieving Sales Price by ID: {recId}")]
    partial void LogRetrievingSalesPriceById(int recId);

    [LoggerMessage(LogLevel.Information, "Creating new Sales Price by user: {username}")]
    partial void LogCreatingSalesPrice(string username);

    [LoggerMessage(LogLevel.Information, "Sales Price {recId} created successfully")]
    partial void LogSalesPriceCreated(int recId);

    [LoggerMessage(LogLevel.Information, "Updating Sales Price {recId} by user: {username}")]
    partial void LogUpdatingSalesPrice(int recId, string username);

    [LoggerMessage(LogLevel.Information, "Sales Price {recId} updated successfully")]
    partial void LogSalesPriceUpdated(int recId);

    [LoggerMessage(LogLevel.Information, "Deleting Sales Price {recId} by user: {username}")]
    partial void LogDeletingSalesPrice(int recId, string username);

    [LoggerMessage(LogLevel.Information, "Sales Price {recId} deleted successfully")]
    partial void LogSalesPriceDeleted(int recId);
}
