using System.Data;
using Microsoft.AspNetCore.Http;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public class ItemDeptService : IItemDeptService
{
    private readonly IItemDeptRepo _itemDeptRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly MapperlyMapper _mapper = new();

    public ItemDeptService(IItemDeptRepo itemRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto)
    {
        _itemDeptRepo = itemRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
    }

    public async Task<ServiceResponse> CboAsync(CboRequestVm requestVm, CancellationToken cancellationToken)
    {
        var itemTypes = await _itemDeptRepo.CboAsync(_mapper.MapToEntity(requestVm), cancellationToken);
        return ServiceResponse<IEnumerable<ItemDeptVm>>.Success(_mapper.MapToVm(itemTypes),"ItemDepts for combo box retrieved successfully.", StatusCodes.Status200OK);
    }

    public async Task<ServiceResponse> SelectByIdAsync(int itemDeptId, CancellationToken cancellationToken)
    {
        var warehouse = await _itemDeptRepo.SelectByIdAsync(itemDeptId, cancellationToken);
        return ServiceResponse<ItemDeptVm>.Success(_mapper.MapToVm(warehouse),"Warehouse retrieved successfully.", StatusCodes.Status200OK);
    }

    public async Task<ServiceResponse> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var itemTypes = await _itemDeptRepo.SelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<ItemDeptVm>>.Success(_mapper.MapToVm(itemTypes),"ItemDepts retrieved successfully.", StatusCodes.Status200OK);
    }

    public async Task<ServiceResponse> InsertAsync(ItemDeptVm itemDeptVm, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(itemDeptVm);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedItemDept = await _itemDeptRepo.InsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse<ItemDeptVm>.Success(_mapper.MapToVm(insertedItemDept),"ItemDept inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> UpdateAsync(ItemDeptVm itemDeptVm, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(itemDeptVm);
        entity.ModifiedBy = _userClaimDto.Username;
        var updatedItemDept = await _itemDeptRepo.UpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse<ItemDeptVm>.Success(_mapper.MapToVm(updatedItemDept),"ItemDept updated successfully.", StatusCodes.Status200OK);
    }

    public async Task<ServiceResponse> DeleteAsync(int itemDeptId, string modifiedBy, CancellationToken cancellationToken)
    {
        await _itemDeptRepo.DeleteAsync(itemDeptId, modifiedBy, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse.Success("ItemDept deleted successfully.");
    }
}