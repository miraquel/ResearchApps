using System.Data;
using Microsoft.AspNetCore.Http;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public class PrService : IPrService
{
    private readonly IPrRepo _prRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly MapperlyMapper _mapper = new();

    public PrService(IPrRepo prRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto)
    {
        _prRepo = prRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
    }

    public async Task<ServiceResponse> PrDelete(int id, CancellationToken cancellationToken)
    {
        await _prRepo.PrDelete(id, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse.Success("PR deleted successfully.");
    }

    public async Task<ServiceResponse<int>> PrInsert(PrVm pr, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(pr);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedPrId = await _prRepo.PrInsert(entity, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse<int>.Success(insertedPrId, "PR inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<PagedListVm<PrVm>>> PrSelect(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        var prs = await _prRepo.PrSelect(_mapper.MapToEntity(request), cancellationToken);
        return ServiceResponse<PagedListVm<PrVm>>.Success(_mapper.MapToVm(prs), "PRs retrieved successfully.");
    }

    public async Task<ServiceResponse<PrVm>> PrSelectById(int id, CancellationToken cancellationToken)
    {
        var pr = await _prRepo.PrSelectById(id, cancellationToken);
        return ServiceResponse<PrVm>.Success(_mapper.MapToVm(pr), "PR retrieved successfully.");
    }

    public async Task<ServiceResponse> PrUpdate(PrVm pr, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(pr);
        entity.ModifiedBy = _userClaimDto.Username;
        await _prRepo.PrUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse.Success("PR updated successfully.");
    }
}