using System.Data;
using Microsoft.AspNetCore.Http;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public class UnitService : IUnitService
{
    private readonly IUnitRepo _unitRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly MapperlyMapper _mapper = new();

    public UnitService(IUnitRepo unitRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto)
    {
        _unitRepo = unitRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
    }

    public async Task<ServiceResponse> CboAsync(CboRequestVm listRequest,
        CancellationToken cancellationToken)
    {
        var units = await _unitRepo.UnitCboAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<IEnumerable<UnitVm>>.Success(_mapper.MapToVm(units));
    }

    public async Task<ServiceResponse> DeleteAsync(int unitId, CancellationToken cancellationToken)
    {
        await _unitRepo.UnitDeleteAsync(unitId, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse.Success("Unit deleted successfully.");
    }

    public async Task<ServiceResponse> InsertAsync(UnitVm unitVm, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(unitVm);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedUnit = await _unitRepo.UnitInsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse<UnitVm>.Success(_mapper.MapToVm(insertedUnit), "Unit inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var units = await _unitRepo.UnitSelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<UnitVm>>.Success(_mapper.MapToVm(units), "Units retrieved successfully.");
    }

    public async Task<ServiceResponse> SelectByIdAsync(int unitId, CancellationToken cancellationToken)
    {
        var unit = await _unitRepo.UnitSelectByIdAsync(unitId, cancellationToken);
        return ServiceResponse<UnitVm>.Success(_mapper.MapToVm(unit), "Unit retrieved successfully.");
    }

    public async Task<ServiceResponse> UpdateAsync(UnitVm unitVm, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(unitVm);
        entity.ModifiedBy = _userClaimDto.Username;
        var updatedUnit = await _unitRepo.UnitUpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse<UnitVm>.Success(_mapper.MapToVm(updatedUnit), "Unit updated successfully.");
    }
}