using System.Data;
using Microsoft.AspNetCore.Http;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public class ItemTypeService : IItemTypeService
{
    private readonly IItemTypeRepo _itemTypeRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly MapperlyMapper _mapper = new();

    public ItemTypeService(IItemTypeRepo itemTypeRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto)
    {
        _itemTypeRepo = itemTypeRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
    }

    public async Task<ServiceResponse> ItemTypeSelectAsync(PagedListRequestVm listRequest,
        CancellationToken cancellationToken)
    {
        var itemTypes = await _itemTypeRepo.ItemTypeSelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<ItemTypeVm>>.Success(_mapper.MapToVm(itemTypes), "ItemTypes retrieved successfully.");
    }

    public async Task<ServiceResponse> ItemTypeSelectByIdAsync(int itemTypeId, CancellationToken cancellationToken)
    {
        var itemType = await _itemTypeRepo.ItemTypeSelectByIdAsync(itemTypeId, cancellationToken);
        return ServiceResponse<ItemTypeVm>.Success(_mapper.MapToVm(itemType), "ItemType retrieved successfully.");
    }

    public async Task<ServiceResponse> ItemTypeInsertAsync(ItemTypeVm itemType, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(itemType);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedItemType = await _itemTypeRepo.ItemTypeInsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse<ItemTypeVm>.Success(_mapper.MapToVm(insertedItemType), "ItemType inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> ItemTypeUpdateAsync(ItemTypeVm itemType, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(itemType);
        entity.ModifiedBy = _userClaimDto.Username;
        var updatedItemType = await _itemTypeRepo.ItemTypeUpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse<ItemTypeVm>.Success(_mapper.MapToVm(updatedItemType), "ItemType updated successfully.");
    }

    public async Task<ServiceResponse> ItemTypeDeleteAsync(int itemTypeId, CancellationToken cancellationToken)
    {
        await _itemTypeRepo.ItemTypeDeleteAsync(itemTypeId, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse.Success("ItemType deleted successfully.");
    }

    public async Task<ServiceResponse> ItemTypeCbo(CboRequestVm pagedCboRequestVm, CancellationToken cancellationToken)
    {
        var itemTypes = await _itemTypeRepo.ItemTypeCbo(_mapper.MapToEntity(pagedCboRequestVm), cancellationToken);
        return ServiceResponse<IEnumerable<ItemTypeVm>>.Success(_mapper.MapToVm(itemTypes), "ItemTypes for combo box retrieved successfully.");
    }
}