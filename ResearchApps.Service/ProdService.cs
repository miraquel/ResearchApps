using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class ProdService : IProdService
{
    private readonly IProdRepo _prodRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<ProdService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public ProdService(
        IProdRepo prodRepo,
        IDbTransaction dbTransaction,
        UserClaimDto userClaimDto,
        ILogger<ProdService> logger)
    {
        _prodRepo = prodRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PagedListVm<ProdVm>>> SelectAsync(
        PagedListRequestVm request,
        CancellationToken ct)
    {
        LogRetrievingProdList(request.PageNumber, request.PageSize);
        var pagedList = await _prodRepo.SelectAsync(_mapper.MapToEntity(request), ct);
        LogRetrievedProdCount(pagedList.TotalCount);
        return ServiceResponse<PagedListVm<ProdVm>>.Success(_mapper.MapToVm(pagedList), "Production records retrieved successfully.");
    }

    public async Task<ServiceResponse<ProdVm>> SelectByIdAsync(int recId, CancellationToken ct)
    {
        LogRetrievingProdById(recId);
        var entity = await _prodRepo.SelectByIdAsync(recId, ct);
        return ServiceResponse<ProdVm>.Success(_mapper.MapToVm(entity), "Production record retrieved successfully.");
    }

    public async Task<ServiceResponse<ProdVm>> SelectByProdIdAsync(string prodId, CancellationToken ct)
    {
        LogRetrievingProdByProdId(prodId);
        var entity = await _prodRepo.SelectByProdIdAsync(prodId, ct);
        return ServiceResponse<ProdVm>.Success(_mapper.MapToVm(entity), "Production record retrieved successfully.");
    }

    public async Task<ServiceResponse<ProdVm>> InsertAsync(ProdVm prodVm, CancellationToken ct)
    {
        LogCreatingProd(prodVm.ItemName ?? "N/A", _userClaimDto.Username);

        var entity = _mapper.MapToEntity(prodVm);
        entity.CreatedBy = _userClaimDto.Username;
        entity.CreatedDate = DateTime.UtcNow;
        entity.ModifiedBy = _userClaimDto.Username;
        entity.ModifiedDate = DateTime.UtcNow;

        var recId = await _prodRepo.InsertAsync(entity, ct);
        _dbTransaction.Commit();

        LogProdCreated(recId);

        // Retrieve inserted record to get auto-generated ProdId
        var inserted = await _prodRepo.SelectByIdAsync(recId, ct);

        return ServiceResponse<ProdVm>.Success(
            _mapper.MapToVm(inserted),
            "Production record created successfully.",
            StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<ProdVm>> UpdateAsync(ProdVm prodVm, CancellationToken ct)
    {
        LogUpdatingProd(prodVm.RecId, _userClaimDto.Username);

        var entity = _mapper.MapToEntity(prodVm);
        entity.ModifiedBy = _userClaimDto.Username;
        entity.ModifiedDate = DateTime.UtcNow;

        await _prodRepo.UpdateAsync(entity, ct);
        _dbTransaction.Commit();

        LogProdUpdated(prodVm.RecId);

        // Retrieve updated record
        var updated = await _prodRepo.SelectByIdAsync(prodVm.RecId, ct);

        return ServiceResponse<ProdVm>.Success(
            _mapper.MapToVm(updated),
            "Production record updated successfully.");
    }

    public async Task<ServiceResponse> DeleteAsync(int recId, CancellationToken ct)
    {
        LogDeletingProd(recId, _userClaimDto.Username);

        await _prodRepo.DeleteAsync(recId, ct);
        _dbTransaction.Commit();

        LogProdDeleted(recId);

        return ServiceResponse.Success("Production record deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<ProdStatusVm>>> ProdStatusCboAsync(CancellationToken ct)
    {
        LogRetrievingProdStatuses();
        var statuses = await _prodRepo.ProdStatusCboAsync(ct);
        return ServiceResponse<IEnumerable<ProdStatusVm>>.Success(
            _mapper.MapToVm(statuses),
            "Production statuses retrieved successfully.");
    }

    // Logging methods using source generators
    [LoggerMessage(LogLevel.Debug, "Retrieving production list - Page: {PageNumber}, Size: {PageSize}")]
    partial void LogRetrievingProdList(int pageNumber, int pageSize);

    [LoggerMessage(LogLevel.Debug, "Retrieved {Count} production records")]
    partial void LogRetrievedProdCount(int count);

    [LoggerMessage(LogLevel.Debug, "Retrieving production by RecId: {RecId}")]
    partial void LogRetrievingProdById(int recId);

    [LoggerMessage(LogLevel.Debug, "Retrieving production by ProdId: {ProdId}")]
    partial void LogRetrievingProdByProdId(string prodId);

    [LoggerMessage(LogLevel.Information, "Creating production for item {ItemName} by {Username}")]
    partial void LogCreatingProd(string itemName, string username);

    [LoggerMessage(LogLevel.Information, "Production RecId {RecId} created")]
    partial void LogProdCreated(int recId);

    [LoggerMessage(LogLevel.Information, "Updating production RecId {RecId} by {Username}")]
    partial void LogUpdatingProd(int recId, string username);

    [LoggerMessage(LogLevel.Information, "Production RecId {RecId} updated")]
    partial void LogProdUpdated(int recId);

    [LoggerMessage(LogLevel.Information, "Deleting production RecId {RecId} by {Username}")]
    partial void LogDeletingProd(int recId, string username);

    [LoggerMessage(LogLevel.Information, "Production RecId {RecId} deleted")]
    partial void LogProdDeleted(int recId);

    [LoggerMessage(LogLevel.Debug, "Retrieving production statuses for combo box")]
    partial void LogRetrievingProdStatuses();
}
