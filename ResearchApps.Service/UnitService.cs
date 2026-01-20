using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class UnitService : IUnitService
{
    private readonly IUnitRepo _unitRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<UnitService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public UnitService(IUnitRepo unitRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto, ILogger<UnitService> logger)
    {
        _unitRepo = unitRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<UnitVm>>> CboAsync(CboRequestVm listRequest,
        CancellationToken cancellationToken)
    {
        var units = await _unitRepo.UnitCboAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<IEnumerable<UnitVm>>.Success(_mapper.MapToVm(units));
    }

    public async Task<ServiceResponse> DeleteAsync(int unitId, CancellationToken cancellationToken)
    {
        LogDeletingUnit(unitId, _userClaimDto.Username);
        await _unitRepo.UnitDeleteAsync(unitId, cancellationToken);
        _dbTransaction.Commit();
        LogUnitDeleted(unitId);
        return ServiceResponse.Success("Unit deleted successfully.");
    }

    public async Task<ServiceResponse<UnitVm>> InsertAsync(UnitVm unitVm, CancellationToken cancellationToken)
    {
        LogCreatingUnit(unitVm.UnitName, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(unitVm);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedUnit = await _unitRepo.UnitInsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogUnitCreated(insertedUnit.UnitId);
        return ServiceResponse<UnitVm>.Success(_mapper.MapToVm(insertedUnit), "Unit inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<PagedListVm<UnitVm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var units = await _unitRepo.UnitSelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<UnitVm>>.Success(_mapper.MapToVm(units), "Units retrieved successfully.");
    }

    public async Task<ServiceResponse<UnitVm>> SelectByIdAsync(int unitId, CancellationToken cancellationToken)
    {
        var unit = await _unitRepo.UnitSelectByIdAsync(unitId, cancellationToken);
        return ServiceResponse<UnitVm>.Success(_mapper.MapToVm(unit), "Unit retrieved successfully.");
    }

    public async Task<ServiceResponse<UnitVm>> UpdateAsync(UnitVm unitVm, CancellationToken cancellationToken)
    {
        LogUpdatingUnit(unitVm.UnitId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(unitVm);
        entity.ModifiedBy = _userClaimDto.Username;
        var updatedUnit = await _unitRepo.UnitUpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogUnitUpdated(unitVm.UnitId);
        return ServiceResponse<UnitVm>.Success(_mapper.MapToVm(updatedUnit), "Unit updated successfully.");
    }

    [LoggerMessage(LogLevel.Information, "Creating new Unit: {unitName} by user: {username}")]
    partial void LogCreatingUnit(string unitName, string username);

    [LoggerMessage(LogLevel.Information, "Unit created successfully with Id: {unitId}")]
    partial void LogUnitCreated(int unitId);

    [LoggerMessage(LogLevel.Information, "Updating Unit {unitId} by user: {username}")]
    partial void LogUpdatingUnit(int unitId, string username);

    [LoggerMessage(LogLevel.Information, "Unit {unitId} updated successfully")]
    partial void LogUnitUpdated(int unitId);

    [LoggerMessage(LogLevel.Information, "Deleting Unit {unitId} by user: {username}")]
    partial void LogDeletingUnit(int unitId, string username);

    [LoggerMessage(LogLevel.Information, "Unit {unitId} deleted successfully")]
    partial void LogUnitDeleted(int unitId);
}