using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class PhpService : IPhpService
{
    private readonly IPhpRepo _phpRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<PhpService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public PhpService(IPhpRepo phpRepo, IDbTransaction dbTransaction,
        UserClaimDto userClaimDto, ILogger<PhpService> logger)
    {
        _phpRepo = phpRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PagedListVm<PhpHeaderVm>>> PhpSelect(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogRetrievingPhpList();
        
        // Convert Vm request to Domain request
        var domainRequest = new ResearchApps.Domain.Common.PagedListRequest
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            IsSortAscending = request.IsSortAscending,
            Filters = request.Filters
        };
        
        var pagedResult = await _phpRepo.PhpSelect(domainRequest, cancellationToken);
        LogRetrievedPhpCount(pagedResult.Items.Count());
        
        var pagedVm = new PagedListVm<PhpHeaderVm>
        {
            Items = _mapper.MapToVm(pagedResult.Items),
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize
        };
        
        return ServiceResponse<PagedListVm<PhpHeaderVm>>.Success(pagedVm, "Php records retrieved successfully.");
    }

    public async Task<ServiceResponse<PhpHeaderVm>> PhpSelectById(int recId, CancellationToken cancellationToken)
    {
        LogRetrievingPhpById(recId);
        var php = await _phpRepo.PhpSelectById(recId, cancellationToken);
        return ServiceResponse<PhpHeaderVm>.Success(_mapper.MapToVm(php), "Php retrieved successfully.");
    }

    public async Task<ServiceResponse<(int RecId, string PhpId)>> PhpInsert(PhpHeaderVm phpHeader, CancellationToken cancellationToken)
    {
        LogCreatingNewPhpByUser(_userClaimDto.Username);
        var entity = _mapper.MapToEntity(phpHeader);
        entity.CreatedBy = _userClaimDto.Username;
        var result = await _phpRepo.PhpInsert(entity, cancellationToken);
        _dbTransaction.Commit();
        LogPhpCreatedSuccessfully(result.RecId, result.PhpId);
        return ServiceResponse<(int RecId, string PhpId)>.Success(result, "Php created successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> PhpUpdate(PhpHeaderVm phpHeader, CancellationToken cancellationToken)
    {
        LogUpdatingPhpByUser(phpHeader.RecId, phpHeader.PhpId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(phpHeader);
        entity.ModifiedBy = _userClaimDto.Username;
        await _phpRepo.PhpUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogPhpUpdatedSuccessfully(phpHeader.RecId);
        return ServiceResponse.Success("Php updated successfully.");
    }

    public async Task<ServiceResponse> PhpDelete(int recId, CancellationToken cancellationToken)
    {
        LogDeletingPhpByUser(recId, _userClaimDto.Username);
        await _phpRepo.PhpDelete(recId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogPhpDeletedSuccessfully(recId);
        return ServiceResponse.Success("Php deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<PhpLineVm>>> PhpLineSelectByPhp(int phpRecId, CancellationToken cancellationToken)
    {
        var lines = await _phpRepo.PhpLineSelectByPhp(phpRecId, cancellationToken);
        return ServiceResponse<IEnumerable<PhpLineVm>>.Success(_mapper.MapToVm(lines), "Php lines retrieved successfully.");
    }

    public async Task<ServiceResponse<PhpLineVm>> PhpLineSelectById(int phpLineId, CancellationToken cancellationToken)
    {
        var line = await _phpRepo.PhpLineSelectById(phpLineId, cancellationToken);
        if (line == null)
        {
            return ServiceResponse<PhpLineVm>.Failure("Php line not found.", StatusCodes.Status404NotFound);
        }
        return ServiceResponse<PhpLineVm>.Success(_mapper.MapToVm(line), "Php line retrieved successfully.");
    }

    public async Task<ServiceResponse> PhpLineInsert(PhpLineVm phpLine, CancellationToken cancellationToken)
    {
        LogInsertingPhpLineByUser(phpLine.PhpRecId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(phpLine);
        entity.CreatedBy = _userClaimDto.Username;
        var phpId = await _phpRepo.PhpLineInsert(entity, cancellationToken);
        _dbTransaction.Commit();
        LogPhpLineInsertedSuccessfully(phpId);
        return ServiceResponse.Created("Php line created successfully.");
    }

    public async Task<ServiceResponse> PhpLineUpdate(PhpLineVm phpLine, CancellationToken cancellationToken)
    {
        LogUpdatingPhpLineByUser(phpLine.PhpLineId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(phpLine);
        entity.ModifiedBy = _userClaimDto.Username;
        await _phpRepo.PhpLineUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogPhpLineUpdatedSuccessfully(phpLine.PhpLineId);
        return ServiceResponse.Success("Php line updated successfully.");
    }

    public async Task<ServiceResponse> PhpLineDelete(int phpLineId, CancellationToken cancellationToken)
    {
        LogDeletingPhpLineByUser(phpLineId, _userClaimDto.Username);
        await _phpRepo.PhpLineDelete(phpLineId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogPhpLineDeletedSuccessfully(phpLineId);
        return ServiceResponse.Success("Php line deleted successfully.");
    }

    public async Task<ServiceResponse<PhpVm>> GetPhp(int recId, CancellationToken cancellationToken)
    {
        LogRetrievingPhpById(recId);
        var header = await _phpRepo.PhpSelectById(recId, cancellationToken);
        var lines = await _phpRepo.PhpLineSelectByPhp(recId, cancellationToken);

        var vm = new PhpVm
        {
            Header = _mapper.MapToVm(header),
            Lines = _mapper.MapToVm(lines)
        };

        return ServiceResponse<PhpVm>.Success(vm, "Php retrieved successfully.");
    }

    #region Logging

    [LoggerMessage(LogLevel.Information, "Retrieving Php list")]
    partial void LogRetrievingPhpList();

    [LoggerMessage(LogLevel.Information, "Retrieved {count} Php records")]
    partial void LogRetrievedPhpCount(int count);

    [LoggerMessage(LogLevel.Information, "Retrieving Php by RecId: {recId}")]
    partial void LogRetrievingPhpById(int recId);

    [LoggerMessage(LogLevel.Information, "Creating new Php by user: {username}")]
    partial void LogCreatingNewPhpByUser(string username);

    [LoggerMessage(LogLevel.Information, "Php created successfully - RecId: {recId}, PhpId: {phpId}")]
    partial void LogPhpCreatedSuccessfully(int recId, string phpId);

    [LoggerMessage(LogLevel.Information, "Updating Php {recId} ({phpId}) by user: {username}")]
    partial void LogUpdatingPhpByUser(int recId, string phpId, string username);

    [LoggerMessage(LogLevel.Information, "Php {recId} updated successfully")]
    partial void LogPhpUpdatedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Deleting Php {recId} by user: {username}")]
    partial void LogDeletingPhpByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "Php {recId} deleted successfully")]
    partial void LogPhpDeletedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Inserting Php line for Php {phpRecId} by user: {username}")]
    partial void LogInsertingPhpLineByUser(int phpRecId, string username);

    [LoggerMessage(LogLevel.Information, "Php line inserted successfully - PhpId: {phpId}")]
    partial void LogPhpLineInsertedSuccessfully(string phpId);

    [LoggerMessage(LogLevel.Information, "Updating Php line {phpLineId} by user: {username}")]
    partial void LogUpdatingPhpLineByUser(int phpLineId, string username);

    [LoggerMessage(LogLevel.Information, "Php line {phpLineId} updated successfully")]
    partial void LogPhpLineUpdatedSuccessfully(int phpLineId);

    [LoggerMessage(LogLevel.Information, "Deleting Php line {phpLineId} by user: {username}")]
    partial void LogDeletingPhpLineByUser(int phpLineId, string username);

    [LoggerMessage(LogLevel.Information, "Php line {phpLineId} deleted successfully")]
    partial void LogPhpLineDeletedSuccessfully(int phpLineId);

    #endregion
}
