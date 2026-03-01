using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class ItemGroup01Service : IItemGroup01Service
{
    private readonly IItemGroup01Repo _itemGroup01Repo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<ItemGroup01Service> _logger;
    private readonly MapperlyMapper _mapper = new();

    public ItemGroup01Service(IItemGroup01Repo itemGroup01Repo, IDbTransaction dbTransaction, UserClaimDto userClaimDto, ILogger<ItemGroup01Service> logger)
    {
        _itemGroup01Repo = itemGroup01Repo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<ItemGroup01Vm>>> CboAsync(CboRequestVm requestVm, CancellationToken cancellationToken)
    {
        var items = await _itemGroup01Repo.CboAsync(_mapper.MapToEntity(requestVm), cancellationToken);
        return ServiceResponse<IEnumerable<ItemGroup01Vm>>.Success(_mapper.MapToVm(items), "ItemGroup01s for combo box retrieved successfully.");
    }

    public async Task<ServiceResponse<ItemGroup01Vm>> SelectByIdAsync(int itemGroup01Id, CancellationToken cancellationToken)
    {
        var itemGroup01 = await _itemGroup01Repo.SelectByIdAsync(itemGroup01Id, cancellationToken);
        return ServiceResponse<ItemGroup01Vm>.Success(_mapper.MapToVm(itemGroup01), "ItemGroup01 retrieved successfully.");
    }

    public async Task<ServiceResponse<PagedListVm<ItemGroup01Vm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var items = await _itemGroup01Repo.SelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<ItemGroup01Vm>>.Success(_mapper.MapToVm(items), "ItemGroup01s retrieved successfully.");
    }

    public async Task<ServiceResponse<ItemGroup01Vm>> InsertAsync(ItemGroup01Vm itemGroup01Vm, CancellationToken cancellationToken)
    {
        LogCreatingItemGroup01(itemGroup01Vm.ItemGroup01Name, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(itemGroup01Vm);
        entity.CreatedBy = _userClaimDto.Username;
        var inserted = await _itemGroup01Repo.InsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogItemGroup01Created(inserted.ItemGroup01Id);
        return ServiceResponse<ItemGroup01Vm>.Success(_mapper.MapToVm(inserted), "ItemGroup01 inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<ItemGroup01Vm>> UpdateAsync(ItemGroup01Vm itemGroup01Vm, CancellationToken cancellationToken)
    {
        LogUpdatingItemGroup01(itemGroup01Vm.ItemGroup01Id, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(itemGroup01Vm);
        entity.ModifiedBy = _userClaimDto.Username;
        var updated = await _itemGroup01Repo.UpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogItemGroup01Updated(itemGroup01Vm.ItemGroup01Id);
        return ServiceResponse<ItemGroup01Vm>.Success(_mapper.MapToVm(updated), "ItemGroup01 updated successfully.");
    }

    public async Task<ServiceResponse> DeleteAsync(int itemGroup01Id, string modifiedBy, CancellationToken cancellationToken)
    {
        LogDeletingItemGroup01(itemGroup01Id, modifiedBy);
        await _itemGroup01Repo.DeleteAsync(itemGroup01Id, modifiedBy, cancellationToken);
        _dbTransaction.Commit();
        LogItemGroup01Deleted(itemGroup01Id);
        return ServiceResponse.Success("ItemGroup01 deleted successfully.");
    }

    [LoggerMessage(LogLevel.Information, "Creating new ItemGroup01: {itemGroup01Name} by user: {username}")]
    partial void LogCreatingItemGroup01(string itemGroup01Name, string username);

    [LoggerMessage(LogLevel.Information, "ItemGroup01 created successfully with Id: {itemGroup01Id}")]
    partial void LogItemGroup01Created(int itemGroup01Id);

    [LoggerMessage(LogLevel.Information, "Updating ItemGroup01 {itemGroup01Id} by user: {username}")]
    partial void LogUpdatingItemGroup01(int itemGroup01Id, string username);

    [LoggerMessage(LogLevel.Information, "ItemGroup01 {itemGroup01Id} updated successfully")]
    partial void LogItemGroup01Updated(int itemGroup01Id);

    [LoggerMessage(LogLevel.Information, "Deleting ItemGroup01 {itemGroup01Id} by user: {username}")]
    partial void LogDeletingItemGroup01(int itemGroup01Id, string username);

    [LoggerMessage(LogLevel.Information, "ItemGroup01 {itemGroup01Id} deleted successfully")]
    partial void LogItemGroup01Deleted(int itemGroup01Id);
}
