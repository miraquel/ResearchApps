using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class PoLineService : IPoLineService
{
    private readonly IPoLineRepo _poLineRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<PoLineService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public PoLineService(
        IPoLineRepo poLineRepo,
        IDbTransaction dbTransaction,
        UserClaimDto userClaimDto,
        ILogger<PoLineService> logger)
    {
        _poLineRepo = poLineRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PoLineVm>> PoLineSelectById(int poLineId, CancellationToken ct)
    {
        var entity = await _poLineRepo.PoLineSelectById(poLineId, ct);
        if (entity == null)
            return ServiceResponse<PoLineVm>.Failure("PO line not found", StatusCodes.Status404NotFound);

        return ServiceResponse<PoLineVm>.Success(_mapper.MapToVm(entity));
    }

    public async Task<ServiceResponse<IEnumerable<PoLineVm>>> PoLineSelectByPo(int recId, CancellationToken ct)
    {
        var entities = await _poLineRepo.PoLineSelectByPo(recId, ct);
        return ServiceResponse<IEnumerable<PoLineVm>>.Success(_mapper.MapToVm(entities));
    }

    public async Task<ServiceResponse<string>> PoLineInsert(PoLineVm poLineVm, CancellationToken ct)
    {
        LogCreatingPoLine(poLineVm.PoId, poLineVm.ItemId, _userClaimDto.Username);

        var entity = _mapper.MapToEntity(poLineVm);
        entity.CreatedBy = _userClaimDto.Username;
        entity.CreatedDate = DateTime.UtcNow;
        entity.ModifiedBy = _userClaimDto.Username;
        entity.ModifiedDate = DateTime.UtcNow;

        var poId = await _poLineRepo.PoLineInsert(entity, ct);
        _dbTransaction.Commit();

        LogPoLineCreated(poId, poLineVm.ItemId);

        return ServiceResponse<string>.Success(
            poId,
            "PO line created successfully.",
            StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<PoLineVm>> PoLineUpdate(PoLineVm poLineVm, CancellationToken ct)
    {
        LogUpdatingPoLine(poLineVm.PoLineId, poLineVm.ItemId, _userClaimDto.Username);

        var entity = _mapper.MapToEntity(poLineVm);
        entity.ModifiedBy = _userClaimDto.Username;
        entity.ModifiedDate = DateTime.UtcNow;

        await _poLineRepo.PoLineUpdate(entity, ct);
        _dbTransaction.Commit();

        // Fetch updated line with recalculated amounts
        var updatedLine = await _poLineRepo.PoLineSelectById(poLineVm.PoLineId, ct);
        var result = _mapper.MapToVm(updatedLine!);

        LogPoLineUpdated(poLineVm.PoLineId, poLineVm.ItemId);

        return ServiceResponse<PoLineVm>.Success(result, "PO line updated successfully.");
    }

    public async Task<ServiceResponse<string>> PoLineDelete(int poLineId, CancellationToken ct)
    {
        LogDeletingPoLine(poLineId, _userClaimDto.Username);

        // Get PoId before delete for return value
        var line = await _poLineRepo.PoLineSelectById(poLineId, ct);
        var poId = line?.PoId ?? string.Empty;

        await _poLineRepo.PoLineDelete(poLineId, ct);
        _dbTransaction.Commit();

        LogPoLineDeleted(poLineId);

        return ServiceResponse<string>.Success(poId, "PO line deleted successfully.");
    }

    #region Logging Methods

    [LoggerMessage(LogLevel.Information, "Creating PO line for PO {PoId}, Item {ItemId} by {Username}")]
    partial void LogCreatingPoLine(string poId, int itemId, string username);

    [LoggerMessage(LogLevel.Information, "PO line created for PO {PoId}, Item {ItemId}")]
    partial void LogPoLineCreated(string poId, int itemId);

    [LoggerMessage(LogLevel.Information, "Updating PO line {PoLineId}, Item {ItemId} by {Username}")]
    partial void LogUpdatingPoLine(int poLineId, int itemId, string username);

    [LoggerMessage(LogLevel.Information, "PO line {PoLineId}, Item {ItemId} updated")]
    partial void LogPoLineUpdated(int poLineId, int itemId);

    [LoggerMessage(LogLevel.Information, "Deleting PO line {PoLineId} by {Username}")]
    partial void LogDeletingPoLine(int poLineId, string username);

    [LoggerMessage(LogLevel.Information, "PO line {PoLineId} deleted")]
    partial void LogPoLineDeleted(int poLineId);

    #endregion
}
