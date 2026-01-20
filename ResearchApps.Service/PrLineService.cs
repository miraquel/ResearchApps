using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class PrLineService : IPrLineService
{
    private readonly IPrLineRepo _prLineRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<PrLineService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public PrLineService(IPrLineRepo prLineRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto, ILogger<PrLineService> logger)
    {
        _prLineRepo = prLineRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<string>> PrLineDelete(int id, CancellationToken cancellationToken)
    {
        LogDeletingPrLine(id, _userClaimDto.Username);
        var result = await _prLineRepo.PrLineDelete(id, cancellationToken);
        _dbTransaction.Commit();
        LogPrLineDeleted(id);
        return ServiceResponse<string>.Success(result, "PrLine deleted successfully.");
    }

    public async Task<ServiceResponse<string>> PrLineInsert(PrLineVm prLine, CancellationToken cancellationToken)
    {
        LogCreatingPrLine(prLine.PrId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(prLine);
        entity.CreatedBy = _userClaimDto.Username;
        var result =  await  _prLineRepo.PrLineInsert( entity, cancellationToken);
        _dbTransaction.Commit();
        LogPrLineCreated(prLine.PrId);
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
    public async Task<ServiceResponse<IEnumerable<PrLineVm>>> PrLineSelectForPo(
        int poRecId, 
        int pageNumber, 
        int pageSize, 
        string? prId, 
        string? itemName, 
        DateTime? dateFrom, 
        CancellationToken cancellationToken)
    {
        var prLines = await _prLineRepo.PrLineSelectForPo(poRecId, pageNumber, pageSize, prId, itemName, dateFrom, cancellationToken);
        var prLineVm = _mapper.MapToVm(prLines);
        return ServiceResponse<IEnumerable<PrLineVm>>.Success(prLineVm, "Available PR Lines fetched successfully.");
    }
    public async Task<ServiceResponse<string>> PrLineUpdate(PrLineVm prLine, CancellationToken cancellationToken)
    {
        LogUpdatingPrLine(prLine.PrLineId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(prLine);
        entity.ModifiedBy = _userClaimDto.Username;
        var result = await _prLineRepo.PrLineUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogPrLineUpdated(prLine.PrLineId);
        return ServiceResponse<string>.Success(result, "PrLine updated successfully.");
    }

    [LoggerMessage(LogLevel.Information, "Creating new PrLine for PR: {prId} by user: {username}")]
    partial void LogCreatingPrLine(string prId, string username);

    [LoggerMessage(LogLevel.Information, "PrLine created successfully for PR: {prId}")]
    partial void LogPrLineCreated(string prId);

    [LoggerMessage(LogLevel.Information, "Updating PrLine {lineId} by user: {username}")]
    partial void LogUpdatingPrLine(int lineId, string username);

    [LoggerMessage(LogLevel.Information, "PrLine {lineId} updated successfully")]
    partial void LogPrLineUpdated(int lineId);

    [LoggerMessage(LogLevel.Information, "Deleting PrLine {lineId} by user: {username}")]
    partial void LogDeletingPrLine(int lineId, string username);

    [LoggerMessage(LogLevel.Information, "PrLine {lineId} deleted successfully")]
    partial void LogPrLineDeleted(int lineId);
}