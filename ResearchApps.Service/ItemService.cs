using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Domain;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class ItemService : IItemService
{
    private readonly IItemRepo _itemRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<ItemService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public ItemService(IItemRepo itemRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto, ILogger<ItemService> logger)
    {
        _itemRepo = itemRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse> CboAsync(CboRequestVm cboRequestVm, CancellationToken cancellationToken)
    {
        if (cboRequestVm.Term != null) LogRetrievingItemsForComboBoxWithTermTerm(cboRequestVm.Term);
        var items = await _itemRepo.CboAsync(_mapper.MapToEntity(cboRequestVm), cancellationToken);
        var enumerable = items as Item[] ?? items.ToArray();
        LogRetrievedCountItemsForComboBox(enumerable.Length);
        return ServiceResponse<IEnumerable<ItemVm>>.Success(_mapper.MapToVm(enumerable), "Items for combo box retrieved successfully.");
    }

    public async Task<ServiceResponse> DeleteAsync(int itemId, CancellationToken cancellationToken)
    {
        LogDeletingItemWithIDItemidByUserUsername(itemId, _userClaimDto.Username);
        await _itemRepo.DeleteAsync(itemId, cancellationToken);
        _dbTransaction.Commit();
        LogItemItemidDeletedSuccessfully(itemId);
        return ServiceResponse.Success("Item deleted successfully.");
    }

    public async Task<ServiceResponse> InsertAsync(ItemVm itemVm, CancellationToken cancellationToken)
    {
        LogCreatingNewItemItemNameByUserUsername(itemVm.ItemName, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(itemVm);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedItem = await _itemRepo.InsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogItemCreatedSuccessfullyWithIDItemid(insertedItem.ItemId);
        return ServiceResponse<ItemVm>.Success(_mapper.MapToVm(insertedItem), "Item inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        LogRetrievingItemsListPagePagePageSizePageSize(listRequest.PageNumber, listRequest.PageSize);
        var itemTypes = await _itemRepo.SelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        LogRetrievedCountItems(itemTypes.Items.Count());
        return ServiceResponse<PagedListVm<ItemVm>>.Success(_mapper.MapToVm(itemTypes), "Items retrieved successfully.");
    }

    public async Task<ServiceResponse> SelectByIdAsync(int itemId, CancellationToken cancellationToken)
    {
        LogRetrievingItemByIDItemid(itemId);
        var item = await _itemRepo.SelectByIdAsync(itemId, cancellationToken);
        return ServiceResponse<ItemVm>.Success(_mapper.MapToVm(item), "Item retrieved successfully.");
    }

    public async Task<ServiceResponse> UpdateAsync(ItemVm itemVm, CancellationToken cancellationToken)
    {
        LogUpdatingItemItemidItemNameByUserUsername(itemVm.ItemId, itemVm.ItemName, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(itemVm);
        entity.ModifiedBy = _userClaimDto.Username;
        var updatedItem = await _itemRepo.UpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogItemItemidUpdatedSuccessfully(itemVm.ItemId);
        return ServiceResponse<ItemVm>.Success(_mapper.MapToVm(updatedItem), "Item updated successfully.");
    }

    [LoggerMessage(LogLevel.Debug, "Retrieving items for combo box with term: {term}")]
    partial void LogRetrievingItemsForComboBoxWithTermTerm(string term);

    [LoggerMessage(LogLevel.Debug, "Retrieved {count} items for combo box")]
    partial void LogRetrievedCountItemsForComboBox(int count);

    [LoggerMessage(LogLevel.Information, "Deleting item with ID: {itemId} by user: {username}")]
    partial void LogDeletingItemWithIDItemidByUserUsername(int itemId, string username);

    [LoggerMessage(LogLevel.Information, "Item {itemId} deleted successfully")]
    partial void LogItemItemidDeletedSuccessfully(int itemId);

    [LoggerMessage(LogLevel.Information, "Creating new item: {itemName} by user: {username}")]
    partial void LogCreatingNewItemItemNameByUserUsername(string itemName, string username);

    [LoggerMessage(LogLevel.Information, "Item created successfully with ID: {itemId}")]
    partial void LogItemCreatedSuccessfullyWithIDItemid(int itemId);

    [LoggerMessage(LogLevel.Debug, "Retrieving items list - Page: {page}, PageSize: {pageSize}")]
    partial void LogRetrievingItemsListPagePagePageSizePageSize(int page, int pageSize);

    [LoggerMessage(LogLevel.Debug, "Retrieved {count} items")]
    partial void LogRetrievedCountItems(int count);

    [LoggerMessage(LogLevel.Debug, "Retrieving item by ID: {itemId}")]
    partial void LogRetrievingItemByIDItemid(int itemId);

    [LoggerMessage(LogLevel.Information, "Updating item {itemId}: {itemName} by user: {username}")]
    partial void LogUpdatingItemItemidItemNameByUserUsername(int itemId, string itemName, string username);

    [LoggerMessage(LogLevel.Information, "Item {itemId} updated successfully")]
    partial void LogItemItemidUpdatedSuccessfully(int itemId);
}