using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class ItemDeptService : IItemDeptService
{
    private readonly IItemDeptRepo _itemDeptRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<ItemDeptService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public ItemDeptService(IItemDeptRepo itemRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto, ILogger<ItemDeptService> logger)
    {
        _itemDeptRepo = itemRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<ItemDeptVm>>> CboAsync(CboRequestVm requestVm, CancellationToken cancellationToken)
    {
        var itemTypes = await _itemDeptRepo.CboAsync(_mapper.MapToEntity(requestVm), cancellationToken);
        return ServiceResponse<IEnumerable<ItemDeptVm>>.Success(_mapper.MapToVm(itemTypes),"ItemDepts for combo box retrieved successfully.");
    }

    public async Task<ServiceResponse<ItemDeptVm>> SelectByIdAsync(int itemDeptId, CancellationToken cancellationToken)
    {
        var warehouse = await _itemDeptRepo.SelectByIdAsync(itemDeptId, cancellationToken);
        return ServiceResponse<ItemDeptVm>.Success(_mapper.MapToVm(warehouse),"Warehouse retrieved successfully.");
    }

    public async Task<ServiceResponse<PagedListVm<ItemDeptVm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var itemTypes = await _itemDeptRepo.SelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<ItemDeptVm>>.Success(_mapper.MapToVm(itemTypes),"ItemDepts retrieved successfully.");
    }

    public async Task<ServiceResponse<ItemDeptVm>> InsertAsync(ItemDeptVm itemDeptVm, CancellationToken cancellationToken)
    {
        LogCreatingItemDept(itemDeptVm.ItemDeptName, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(itemDeptVm);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedItemDept = await _itemDeptRepo.InsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogItemDeptCreated(insertedItemDept.ItemDeptId);
        return ServiceResponse<ItemDeptVm>.Success(_mapper.MapToVm(insertedItemDept),"ItemDept inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<ItemDeptVm>> UpdateAsync(ItemDeptVm itemDeptVm, CancellationToken cancellationToken)
    {
        LogUpdatingItemDept(itemDeptVm.ItemDeptId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(itemDeptVm);
        entity.ModifiedBy = _userClaimDto.Username;
        var updatedItemDept = await _itemDeptRepo.UpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogItemDeptUpdated(itemDeptVm.ItemDeptId);
        return ServiceResponse<ItemDeptVm>.Success(_mapper.MapToVm(updatedItemDept),"ItemDept updated successfully.");
    }

    public async Task<ServiceResponse> DeleteAsync(int itemDeptId, string modifiedBy, CancellationToken cancellationToken)
    {
        LogDeletingItemDept(itemDeptId, modifiedBy);
        await _itemDeptRepo.DeleteAsync(itemDeptId, modifiedBy, cancellationToken);
        _dbTransaction.Commit();
        LogItemDeptDeleted(itemDeptId);
        return ServiceResponse.Success("ItemDept deleted successfully.");
    }

    [LoggerMessage(LogLevel.Information, "Creating new ItemDept: {itemDeptName} by user: {username}")]
    partial void LogCreatingItemDept(string itemDeptName, string username);

    [LoggerMessage(LogLevel.Information, "ItemDept created successfully with Id: {itemDeptId}")]
    partial void LogItemDeptCreated(int itemDeptId);

    [LoggerMessage(LogLevel.Information, "Updating ItemDept {itemDeptId} by user: {username}")]
    partial void LogUpdatingItemDept(int itemDeptId, string username);

    [LoggerMessage(LogLevel.Information, "ItemDept {itemDeptId} updated successfully")]
    partial void LogItemDeptUpdated(int itemDeptId);

    [LoggerMessage(LogLevel.Information, "Deleting ItemDept {itemDeptId} by user: {username}")]
    partial void LogDeletingItemDept(int itemDeptId, string username);

    [LoggerMessage(LogLevel.Information, "ItemDept {itemDeptId} deleted successfully")]
    partial void LogItemDeptDeleted(int itemDeptId);
}