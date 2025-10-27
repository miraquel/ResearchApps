using System.Data;
using Microsoft.AspNetCore.Http;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public class PrLineService : IPrLineService
{
    private readonly IPrLineRepo _prLineRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly MapperlyMapper _mapper = new();

    public PrLineService(IPrLineRepo prLineRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto)
    {
        _prLineRepo = prLineRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
    }

    public async Task<ServiceResponse<string>> PrLineDelete(int id, CancellationToken cancellationToken)
    {
        var result = await _prLineRepo.PrLineDelete(id, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse<string>.Success(result, "PrLine deleted successfully.");
    }

    public async Task<ServiceResponse<string>> PrLineInsert(PrLineVm prLine, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(prLine);
        entity.CreatedBy = _userClaimDto.Username;
        var result =  await  _prLineRepo.PrLineInsert( entity, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse<string>.Success(result, "PrLine inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<PrLineVm>> PrLineSelectById(int id, CancellationToken cancellationToken)
    {
        var prLine = await _prLineRepo.PrLineSelectById(id, cancellationToken);
        return ServiceResponse<PrLineVm>.Success(_mapper.MapToVm(prLine), "PrLine retrieved successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<PrLineVm>>> PrLineSelectByPr(string prId,
        CancellationToken cancellationToken)
    {
        var prLines = await _prLineRepo.PrLineSelectByPr(prId, cancellationToken);
        return ServiceResponse<IEnumerable<PrLineVm>>.Success(_mapper.MapToVm(prLines), "PrLines retrieved successfully.");
    }

    public async Task<ServiceResponse<string>> PrLineUpdate(PrLineVm prLine, CancellationToken cancellationToken)
    {
        var entity = _mapper.MapToEntity(prLine);
        entity.ModifiedBy = _userClaimDto.Username;
        var result = await _prLineRepo.PrLineUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        return ServiceResponse<string>.Success(result, "PrLine updated successfully.");
    }
}