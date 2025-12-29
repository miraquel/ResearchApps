using System.Data;
using Microsoft.AspNetCore.Http;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepo _warehouseRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly MapperlyMapper _mapper = new();

    public WarehouseService(IWarehouseRepo warehouseRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto)
    {
        _warehouseRepo = warehouseRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
    }

    public async Task<ServiceResponse> CboAsync()
    {
        var itemTypes = await _warehouseRepo.CboAsync();
        return ServiceResponse<IEnumerable<WarehouseVm>>.Success(_mapper.MapToVm(itemTypes),"ItemTypes for combo box retrieved successfully.");
    }

    public async Task<ServiceResponse> DeleteAsync(int whId, string modifiedBy, CancellationToken cancellationToken)
    {
        await _warehouseRepo.DeleteAsync(whId, modifiedBy, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse.Success("Warehouse deleted successfully.");
    }

    public async Task<ServiceResponse> InsertAsync(WarehouseVm warehouseVm, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(warehouseVm);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedWarehouse = await _warehouseRepo.InsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse<WarehouseVm>.Success(_mapper.MapToVm(insertedWarehouse),"Warehouse inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var warehouses = await _warehouseRepo.SelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<WarehouseVm>>.Success(_mapper.MapToVm(warehouses),"Warehouses retrieved successfully.");
    }

    public async Task<ServiceResponse> SelectByIdAsync(int whId, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepo.SelectByIdAsync(whId, cancellationToken);
        return ServiceResponse<WarehouseVm>.Success(_mapper.MapToVm(warehouse),"Warehouse retrieved successfully.");
    }

    public async Task<ServiceResponse> UpdateAsync(WarehouseVm warehouseVm, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(warehouseVm);
        entity.ModifiedBy = _userClaimDto.Username;
        var updatedWarehouse = await _warehouseRepo.UpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse<WarehouseVm>.Success(_mapper.MapToVm(updatedWarehouse),"Warehouse updated successfully.");
    }
}