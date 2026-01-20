using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class ItemTypeService : IItemTypeService
{
    private readonly IItemTypeRepo _itemTypeRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<ItemTypeService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public ItemTypeService(IItemTypeRepo itemTypeRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto, ILogger<ItemTypeService> logger)
    {
        _itemTypeRepo = itemTypeRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PagedListVm<ItemTypeVm>>> ItemTypeSelectAsync(PagedListRequestVm listRequest,
        CancellationToken cancellationToken)
    {
        var itemTypes = await _itemTypeRepo.ItemTypeSelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<ItemTypeVm>>.Success(_mapper.MapToVm(itemTypes), "ItemTypes retrieved successfully.");
    }

    public async Task<ServiceResponse<ItemTypeVm>> ItemTypeSelectByIdAsync(int itemTypeId, CancellationToken cancellationToken)
    {
        var itemType = await _itemTypeRepo.ItemTypeSelectByIdAsync(itemTypeId, cancellationToken);
        return ServiceResponse<ItemTypeVm>.Success(_mapper.MapToVm(itemType), "ItemType retrieved successfully.");
    }

    public async Task<ServiceResponse<ItemTypeVm>> ItemTypeInsertAsync(ItemTypeVm itemType, CancellationToken cancellationToken)
    {
        LogCreatingItemType(itemType.ItemTypeName, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(itemType);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedItemType = await _itemTypeRepo.ItemTypeInsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogItemTypeCreated(insertedItemType.ItemTypeId);
        return ServiceResponse<ItemTypeVm>.Success(_mapper.MapToVm(insertedItemType), "ItemType inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<ItemTypeVm>> ItemTypeUpdateAsync(ItemTypeVm itemType, CancellationToken cancellationToken)
    {
        LogUpdatingItemType(itemType.ItemTypeId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(itemType);
        entity.ModifiedBy = _userClaimDto.Username;
        var updatedItemType = await _itemTypeRepo.ItemTypeUpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogItemTypeUpdated(itemType.ItemTypeId);
        return ServiceResponse<ItemTypeVm>.Success(_mapper.MapToVm(updatedItemType), "ItemType updated successfully.");
    }

    public async Task<ServiceResponse> ItemTypeDeleteAsync(int itemTypeId, CancellationToken cancellationToken)
    {
        LogDeletingItemType(itemTypeId, _userClaimDto.Username);
        await _itemTypeRepo.ItemTypeDeleteAsync(itemTypeId, cancellationToken);
        _dbTransaction.Commit();
        LogItemTypeDeleted(itemTypeId);
        return ServiceResponse.Success("ItemType deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<ItemTypeVm>>> ItemTypeCbo(CboRequestVm pagedCboRequestVm, CancellationToken cancellationToken)
    {
        var itemTypes = await _itemTypeRepo.ItemTypeCbo(_mapper.MapToEntity(pagedCboRequestVm), cancellationToken);
        return ServiceResponse<IEnumerable<ItemTypeVm>>.Success(_mapper.MapToVm(itemTypes), "ItemTypes for combo box retrieved successfully.");
    }

    [LoggerMessage(LogLevel.Information, "Creating new ItemType: {itemTypeName} by user: {username}")]
    partial void LogCreatingItemType(string itemTypeName, string username);

    [LoggerMessage(LogLevel.Information, "ItemType created successfully with Id: {itemTypeId}")]
    partial void LogItemTypeCreated(int itemTypeId);

    [LoggerMessage(LogLevel.Information, "Updating ItemType {itemTypeId} by user: {username}")]
    partial void LogUpdatingItemType(int itemTypeId, string username);

    [LoggerMessage(LogLevel.Information, "ItemType {itemTypeId} updated successfully")]
    partial void LogItemTypeUpdated(int itemTypeId);

    [LoggerMessage(LogLevel.Information, "Deleting ItemType {itemTypeId} by user: {username}")]
    partial void LogDeletingItemType(int itemTypeId, string username);

    [LoggerMessage(LogLevel.Information, "ItemType {itemTypeId} deleted successfully")]
    partial void LogItemTypeDeleted(int itemTypeId);
}