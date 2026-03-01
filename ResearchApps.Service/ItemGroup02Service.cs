using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class ItemGroup02Service : IItemGroup02Service
{
    private readonly IItemGroup02Repo _itemGroup02Repo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<ItemGroup02Service> _logger;
    private readonly MapperlyMapper _mapper = new();

    public ItemGroup02Service(IItemGroup02Repo itemGroup02Repo, IDbTransaction dbTransaction, UserClaimDto userClaimDto, ILogger<ItemGroup02Service> logger)
    {
        _itemGroup02Repo = itemGroup02Repo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<ItemGroup02Vm>>> CboAsync(CboRequestVm requestVm, CancellationToken cancellationToken)
    {
        var items = await _itemGroup02Repo.CboAsync(_mapper.MapToEntity(requestVm), cancellationToken);
        return ServiceResponse<IEnumerable<ItemGroup02Vm>>.Success(_mapper.MapToVm(items), "ItemGroup02s for combo box retrieved successfully.");
    }

    public async Task<ServiceResponse<ItemGroup02Vm>> SelectByIdAsync(int itemGroup02Id, CancellationToken cancellationToken)
    {
        var itemGroup02 = await _itemGroup02Repo.SelectByIdAsync(itemGroup02Id, cancellationToken);
        return ServiceResponse<ItemGroup02Vm>.Success(_mapper.MapToVm(itemGroup02), "ItemGroup02 retrieved successfully.");
    }

    public async Task<ServiceResponse<PagedListVm<ItemGroup02Vm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var items = await _itemGroup02Repo.SelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<ItemGroup02Vm>>.Success(_mapper.MapToVm(items), "ItemGroup02s retrieved successfully.");
    }

    public async Task<ServiceResponse<ItemGroup02Vm>> InsertAsync(ItemGroup02Vm itemGroup02Vm, CancellationToken cancellationToken)
    {
        LogCreatingItemGroup02(itemGroup02Vm.ItemGroup02Name, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(itemGroup02Vm);
        entity.CreatedBy = _userClaimDto.Username;
        var inserted = await _itemGroup02Repo.InsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogItemGroup02Created(inserted.ItemGroup02Id);
        return ServiceResponse<ItemGroup02Vm>.Success(_mapper.MapToVm(inserted), "ItemGroup02 inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<ItemGroup02Vm>> UpdateAsync(ItemGroup02Vm itemGroup02Vm, CancellationToken cancellationToken)
    {
        LogUpdatingItemGroup02(itemGroup02Vm.ItemGroup02Id, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(itemGroup02Vm);
        entity.ModifiedBy = _userClaimDto.Username;
        var updated = await _itemGroup02Repo.UpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogItemGroup02Updated(itemGroup02Vm.ItemGroup02Id);
        return ServiceResponse<ItemGroup02Vm>.Success(_mapper.MapToVm(updated), "ItemGroup02 updated successfully.");
    }

    public async Task<ServiceResponse> DeleteAsync(int itemGroup02Id, string modifiedBy, CancellationToken cancellationToken)
    {
        LogDeletingItemGroup02(itemGroup02Id, modifiedBy);
        await _itemGroup02Repo.DeleteAsync(itemGroup02Id, modifiedBy, cancellationToken);
        _dbTransaction.Commit();
        LogItemGroup02Deleted(itemGroup02Id);
        return ServiceResponse.Success("ItemGroup02 deleted successfully.");
    }

    [LoggerMessage(LogLevel.Information, "Creating new ItemGroup02: {itemGroup02Name} by user: {username}")]
    partial void LogCreatingItemGroup02(string itemGroup02Name, string username);

    [LoggerMessage(LogLevel.Information, "ItemGroup02 created successfully with Id: {itemGroup02Id}")]
    partial void LogItemGroup02Created(int itemGroup02Id);

    [LoggerMessage(LogLevel.Information, "Updating ItemGroup02 {itemGroup02Id} by user: {username}")]
    partial void LogUpdatingItemGroup02(int itemGroup02Id, string username);

    [LoggerMessage(LogLevel.Information, "ItemGroup02 {itemGroup02Id} updated successfully")]
    partial void LogItemGroup02Updated(int itemGroup02Id);

    [LoggerMessage(LogLevel.Information, "Deleting ItemGroup02 {itemGroup02Id} by user: {username}")]
    partial void LogDeletingItemGroup02(int itemGroup02Id, string username);

    [LoggerMessage(LogLevel.Information, "ItemGroup02 {itemGroup02Id} deleted successfully")]
    partial void LogItemGroup02Deleted(int itemGroup02Id);
}
