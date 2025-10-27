using System.Data;
using Microsoft.AspNetCore.Http;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public class ItemService : IItemService
{
    private readonly IItemRepo _itemRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly MapperlyMapper _mapper = new();

    public ItemService(IItemRepo itemRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto)
    {
        _itemRepo = itemRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
    }

    public async Task<ServiceResponse> DeleteAsync(int itemId, CancellationToken cancellationToken)
    {
        await _itemRepo.DeleteAsync(itemId, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse.Success("Item deleted successfully.");
    }

    public async Task<ServiceResponse> InsertAsync(ItemVm itemVm, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(itemVm);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedItem = await _itemRepo.InsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse<ItemVm>.Success(_mapper.MapToVm(insertedItem), "Item inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var itemTypes = await _itemRepo.SelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<ItemVm>>.Success(_mapper.MapToVm(itemTypes), "Items retrieved successfully.");
    }

    public async Task<ServiceResponse> SelectByIdAsync(int itemId, CancellationToken cancellationToken)
    {
        var item = await _itemRepo.SelectByIdAsync(itemId, cancellationToken);
        return ServiceResponse<ItemVm>.Success(_mapper.MapToVm(item), "Item retrieved successfully.");
    }

    public async Task<ServiceResponse> UpdateAsync(ItemVm itemVm, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(itemVm);
        entity.ModifiedBy = _userClaimDto.Username;
        var updatedItem = await _itemRepo.UpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse<ItemVm>.Success(_mapper.MapToVm(updatedItem), "Item updated successfully.");
    }
}