using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Domain;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class TopService : ITopService
{
    private readonly ITopRepo _topRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<TopService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public TopService(
        ITopRepo topRepo,
        IDbTransaction dbTransaction,
        UserClaimDto userClaimDto,
        ILogger<TopService> logger)
    {
        _topRepo = topRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PagedListVm<TopVm>>> SelectAsync(
        PagedListRequestVm listRequest,
        CancellationToken cancellationToken)
    {
        LogRetrievingTopList(listRequest.PageNumber, listRequest.PageSize);
        var tops = await _topRepo.SelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        LogRetrievedTops(tops.Items.Count());
        return ServiceResponse<PagedListVm<TopVm>>.Success(
            _mapper.MapToVm(tops),
            "TOPs retrieved successfully.");
    }

    public async Task<ServiceResponse<TopVm>> SelectByIdAsync(int topId, CancellationToken cancellationToken)
    {
        LogRetrievingTopById(topId);
        var top = await _topRepo.SelectByIdAsync(topId, cancellationToken);
        return ServiceResponse<TopVm>.Success(
            _mapper.MapToVm(top),
            "TOP retrieved successfully.");
    }

    public async Task<ServiceResponse<TopVm>> InsertAsync(TopVm topVm, CancellationToken cancellationToken)
    {
        LogCreatingTop(topVm.TopName, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(topVm);
        entity.CreatedBy = _userClaimDto.Username;
        var inserted = await _topRepo.InsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogTopCreated(inserted.TopId);
        return ServiceResponse<TopVm>.Success(
            _mapper.MapToVm(inserted),
            "TOP created successfully.",
            StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<TopVm>> UpdateAsync(TopVm topVm, CancellationToken cancellationToken)
    {
        LogUpdatingTop(topVm.TopId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(topVm);
        entity.ModifiedBy = _userClaimDto.Username;
        var updated = await _topRepo.UpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogTopUpdated(topVm.TopId);
        return ServiceResponse<TopVm>.Success(
            _mapper.MapToVm(updated),
            "TOP updated successfully.");
    }

    public async Task<ServiceResponse> DeleteAsync(int topId, CancellationToken cancellationToken)
    {
        LogDeletingTop(topId, _userClaimDto.Username);
        await _topRepo.DeleteAsync(topId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogTopDeleted(topId);
        return ServiceResponse.Success("TOP deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<TopVm>>> CboAsync(
        CboRequestVm cboRequestVm,
        CancellationToken cancellationToken)
    {
        if (cboRequestVm.Term != null) LogRetrievingTopsForCbo(cboRequestVm.Term);
        var tops = await _topRepo.CboAsync(_mapper.MapToEntity(cboRequestVm), cancellationToken);
        var enumerable = tops as Top[] ?? tops.ToArray();
        LogRetrievedTopsForCbo(enumerable.Length);
        return ServiceResponse<IEnumerable<TopVm>>.Success(
            _mapper.MapToVm(enumerable),
            "TOPs for combo box retrieved successfully.");
    }

    [LoggerMessage(LogLevel.Debug, "Retrieving TOP list - Page: {page}, PageSize: {pageSize}")]
    partial void LogRetrievingTopList(int page, int pageSize);

    [LoggerMessage(LogLevel.Debug, "Retrieved {count} TOPs")]
    partial void LogRetrievedTops(int count);

    [LoggerMessage(LogLevel.Debug, "Retrieving TOP by ID: {topId}")]
    partial void LogRetrievingTopById(int topId);

    [LoggerMessage(LogLevel.Information, "Creating new TOP: {topName} by user: {username}")]
    partial void LogCreatingTop(string topName, string username);

    [LoggerMessage(LogLevel.Information, "TOP {topId} created successfully")]
    partial void LogTopCreated(int topId);

    [LoggerMessage(LogLevel.Information, "Updating TOP {topId} by user: {username}")]
    partial void LogUpdatingTop(int topId, string username);

    [LoggerMessage(LogLevel.Information, "TOP {topId} updated successfully")]
    partial void LogTopUpdated(int topId);

    [LoggerMessage(LogLevel.Information, "Deleting TOP {topId} by user: {username}")]
    partial void LogDeletingTop(int topId, string username);

    [LoggerMessage(LogLevel.Information, "TOP {topId} deleted successfully")]
    partial void LogTopDeleted(int topId);

    [LoggerMessage(LogLevel.Debug, "Retrieving TOPs for combo box with term: {term}")]
    partial void LogRetrievingTopsForCbo(string term);

    [LoggerMessage(LogLevel.Debug, "Retrieved {count} TOPs for combo box")]
    partial void LogRetrievedTopsForCbo(int count);
}
