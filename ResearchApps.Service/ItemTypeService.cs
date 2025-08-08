using System.Data;
using System.Net;
using Microsoft.AspNetCore.Http;
using ResearchApps.Common.Exceptions;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public class ItemTypeService : IItemTypeService
{
    private readonly IItemTypeRepo _itemTypeRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly MapperlyMapper _mapper = new();

    public ItemTypeService(IItemTypeRepo itemTypeRepo, IDbTransaction dbTransaction)
    {
        _itemTypeRepo = itemTypeRepo;
        _dbTransaction = dbTransaction;
    }

    public async Task<ServiceResponse<IEnumerable<ItemTypeVm>>> ItemTypeSelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var itemTypes = await _itemTypeRepo.ItemTypeSelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);

        return new ServiceResponse<IEnumerable<ItemTypeVm>>
        {
            Data = _mapper.MapToVm(itemTypes),
            Message = "ItemTypes retrieved successfully.",
            StatusCode = StatusCodes.Status200OK
        };
    }

    public async Task<ServiceResponse<ItemTypeVm>> ItemTypeSelectByIdAsync(int itemTypeId, CancellationToken cancellationToken)
    {
        var itemType = await _itemTypeRepo.ItemTypeSelectByIdAsync(itemTypeId, cancellationToken);

        return new ServiceResponse<ItemTypeVm>
        {
            Data = _mapper.MapToVm(itemType),
            Message = "ItemType retrieved successfully.",
            StatusCode = StatusCodes.Status200OK
        };
    }

    public async Task<ServiceResponse<ItemTypeVm>> ItemTypeInsertAsync(ItemTypeVm itemType, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(itemType);
        var insertedItemType = await _itemTypeRepo.ItemTypeInsertAsync(entity, cancellationToken);
        
        _dbTransaction.Commit();

        return new ServiceResponse<ItemTypeVm>
        {
            Data = _mapper.MapToVm(insertedItemType),
            Message = "ItemType inserted successfully.",
            StatusCode = StatusCodes.Status201Created
        };
    }

    public async Task<ServiceResponse<ItemTypeVm>> ItemTypeUpdateAsync(ItemTypeVm itemType, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(itemType);
        var updatedItemType = await _itemTypeRepo.ItemTypeUpdateAsync(entity, cancellationToken);
        
        _dbTransaction.Commit();

        return new ServiceResponse<ItemTypeVm>
        {
            Data = _mapper.MapToVm(updatedItemType),
            Message = "ItemType updated successfully.",
            StatusCode = StatusCodes.Status200OK
        };
    }

    public async Task<ServiceResponse> ItemTypeDeleteAsync(int itemTypeId, CancellationToken cancellationToken)
    {
        await _itemTypeRepo.ItemTypeDeleteAsync(itemTypeId, cancellationToken);
        
        _dbTransaction.Commit();

        return new ServiceResponse
        {
            Message = "ItemType deleted successfully.",
            StatusCode = StatusCodes.Status204NoContent
        };
    }

    public async Task<ServiceResponse<IEnumerable<ItemTypeVm>>> ItemTypeCbo(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var itemTypes = await _itemTypeRepo.ItemTypeCbo(_mapper.MapToEntity(listRequest), cancellationToken);

        return new ServiceResponse<IEnumerable<ItemTypeVm>>
        {
            Data = _mapper.MapToVm(itemTypes),
            Message = "ItemTypes for combo box retrieved successfully.",
            StatusCode = StatusCodes.Status200OK
        };
    }
}