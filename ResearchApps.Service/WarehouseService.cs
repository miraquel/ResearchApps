using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepo _warehouseRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<WarehouseService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public WarehouseService(IWarehouseRepo warehouseRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto, ILogger<WarehouseService> logger)
    {
        _warehouseRepo = warehouseRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<WarehouseVm>>> CboAsync()
    {
        var itemTypes = await _warehouseRepo.CboAsync();
        return ServiceResponse<IEnumerable<WarehouseVm>>.Success(_mapper.MapToVm(itemTypes),"ItemTypes for combo box retrieved successfully.");
    }

    public async Task<ServiceResponse> DeleteAsync(int whId, string modifiedBy, CancellationToken cancellationToken)
    {
        LogDeletingWarehouse(whId, modifiedBy);
        await _warehouseRepo.DeleteAsync(whId, modifiedBy, cancellationToken);
        _dbTransaction.Commit();
        LogWarehouseDeleted(whId);
        return ServiceResponse.Success("Warehouse deleted successfully.");
    }

    public async Task<ServiceResponse<WarehouseVm>> InsertAsync(WarehouseVm warehouseVm, CancellationToken cancellationToken)
    {
        LogCreatingWarehouse(warehouseVm.WhName, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(warehouseVm);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedWarehouse = await _warehouseRepo.InsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogWarehouseCreated(insertedWarehouse.WhId);
        return ServiceResponse<WarehouseVm>.Success(_mapper.MapToVm(insertedWarehouse),"Warehouse inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<PagedListVm<WarehouseVm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var warehouses = await _warehouseRepo.SelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<WarehouseVm>>.Success(_mapper.MapToVm(warehouses),"Warehouses retrieved successfully.");
    }

    public async Task<ServiceResponse<WarehouseVm>> SelectByIdAsync(int whId, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepo.SelectByIdAsync(whId, cancellationToken);
        return ServiceResponse<WarehouseVm>.Success(_mapper.MapToVm(warehouse),"Warehouse retrieved successfully.");
    }

    public async Task<ServiceResponse<WarehouseVm>> UpdateAsync(WarehouseVm warehouseVm, CancellationToken cancellationToken)
    {
        LogUpdatingWarehouse(warehouseVm.WhId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(warehouseVm);
        entity.ModifiedBy = _userClaimDto.Username;
        var updatedWarehouse = await _warehouseRepo.UpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogWarehouseUpdated(warehouseVm.WhId);
        return ServiceResponse<WarehouseVm>.Success(_mapper.MapToVm(updatedWarehouse),"Warehouse updated successfully.");
    }

    [LoggerMessage(LogLevel.Information, "Creating new Warehouse: {warehouseName} by user: {username}")]
    partial void LogCreatingWarehouse(string warehouseName, string username);

    [LoggerMessage(LogLevel.Information, "Warehouse created successfully with Id: {warehouseId}")]
    partial void LogWarehouseCreated(int warehouseId);

    [LoggerMessage(LogLevel.Information, "Updating Warehouse {warehouseId} by user: {username}")]
    partial void LogUpdatingWarehouse(int warehouseId, string username);

    [LoggerMessage(LogLevel.Information, "Warehouse {warehouseId} updated successfully")]
    partial void LogWarehouseUpdated(int warehouseId);

    [LoggerMessage(LogLevel.Information, "Deleting Warehouse {warehouseId} by user: {username}")]
    partial void LogDeletingWarehouse(int warehouseId, string username);

    [LoggerMessage(LogLevel.Information, "Warehouse {warehouseId} deleted successfully")]
    partial void LogWarehouseDeleted(int warehouseId);
}