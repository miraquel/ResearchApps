using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Domain.Common;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class PsService : IPsService
{
    private readonly IPsRepo _psRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<PsService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public PsService(IPsRepo psRepo, IDbTransaction dbTransaction,
        UserClaimDto userClaimDto, ILogger<PsService> logger)
    {
        _psRepo = psRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<PsHeaderVm>>> PsSelect(CancellationToken cancellationToken)
    {
        LogRetrievingPsList();
        var pss = await _psRepo.PsSelect(cancellationToken);
        var result = _mapper.MapToVm(pss);
        var psHeaderVms = result as PsHeaderVm[] ?? result.ToArray();
        LogRetrievedCountPss(psHeaderVms.Count());
        return ServiceResponse<IEnumerable<PsHeaderVm>>.Success(psHeaderVms, "Penyesuaian Stock list retrieved successfully.");
    }

    public async Task<ServiceResponse<PagedListVm<PsHeaderVm>>> PsSelect(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogRetrievingPsList();
        
        // Create domain request from VM request
        var domainRequest = new PagedListRequest
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            IsSortAscending = request.IsSortAscending,
            Filters = request.Filters
        };
        
        // Call repository method with pagination support
        var pagedResult = await _psRepo.PsSelect(domainRequest, cancellationToken);
        
        var result = new PagedListVm<PsHeaderVm>
        {
            Items = _mapper.MapToVm(pagedResult.Items),
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalCount = pagedResult.TotalCount
        };
        
        LogRetrievedCountPss(pagedResult.Items.Count());
        return ServiceResponse<PagedListVm<PsHeaderVm>>.Success(result, "Penyesuaian Stock list retrieved successfully.");
    }

    public async Task<ServiceResponse<PsHeaderVm>> PsSelectById(int id, CancellationToken cancellationToken)
    {
        LogRetrievingPsById(id);
        var ps = await _psRepo.PsSelectById(id, cancellationToken);
        return ServiceResponse<PsHeaderVm>.Success(_mapper.MapToVm(ps), "Penyesuaian Stock retrieved successfully.");
    }

    public async Task<ServiceResponse<(int RecId, string PsId)>> PsInsert(PsVm ps, CancellationToken cancellationToken)
    {
        LogCreatingNewPsByUser(_userClaimDto.Username);
        var entity = _mapper.MapToEntity(ps);
        entity.Header.CreatedBy = _userClaimDto.Username;
        
        var result = await _psRepo.PsInsert(entity.Header, cancellationToken);

        _dbTransaction.Commit();
        LogPsCreatedSuccessfully(result.RecId, result.PsId);
        return ServiceResponse<(int RecId, string PsId)>.Success(result, "Penyesuaian Stock created successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> PsUpdate(PsHeaderVm psHeader, CancellationToken cancellationToken)
    {
        LogUpdatingPsByUser(psHeader.RecId, psHeader.PsId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(psHeader);
        entity.ModifiedBy = _userClaimDto.Username;
        await _psRepo.PsUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogPsUpdatedSuccessfully(psHeader.RecId);
        return ServiceResponse.Success("Penyesuaian Stock updated successfully.");
    }

    public async Task<ServiceResponse> PsDelete(int recId, CancellationToken cancellationToken)
    {
        LogDeletingPsByUser(recId, _userClaimDto.Username);
        await _psRepo.PsDelete(recId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogPsDeletedSuccessfully(recId);
        return ServiceResponse.Success("Penyesuaian Stock deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<PsLineVm>>> PsLineSelectByPs(int psRecId, CancellationToken cancellationToken)
    {
        LogRetrievingPsLines(psRecId);
        var lines = await _psRepo.PsLineSelectByPs(psRecId, cancellationToken);
        return ServiceResponse<IEnumerable<PsLineVm>>.Success(_mapper.MapToVm(lines), "Penyesuaian Stock lines retrieved successfully.");
    }

    public async Task<ServiceResponse<PsLineVm>> PsLineSelectById(int psLineId, CancellationToken cancellationToken)
    {
        LogRetrievingPsLineById(psLineId);
        var line = await _psRepo.PsLineSelectById(psLineId, cancellationToken);
        if (line == null)
        {
            return ServiceResponse<PsLineVm>.Failure($"Penyesuaian Stock line with Id {psLineId} not found.");
        }
        return ServiceResponse<PsLineVm>.Success(_mapper.MapToVm(line), "Penyesuaian Stock line retrieved successfully.");
    }

    public async Task<ServiceResponse<string>> PsLineInsert(PsLineVm psLine, CancellationToken cancellationToken)
    {
        LogInsertingPsLine(psLine.PsRecId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(psLine);
        entity.CreatedBy = _userClaimDto.Username;
        
        var result = await _psRepo.PsLineInsert(entity, cancellationToken);
        
        // Check for error response (format: "-1:::error message")
        if (result.StartsWith("-1:::"))
        {
            var errorMessage = result.Substring(5);
            return ServiceResponse<string>.Failure(errorMessage, StatusCodes.Status400BadRequest);
        }
        
        _dbTransaction.Commit();
        LogPsLineInsertedSuccessfully(psLine.PsRecId);
        return ServiceResponse<string>.Success(result, "Penyesuaian Stock line inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<string>> PsLineUpdate(PsLineVm psLine, CancellationToken cancellationToken)
    {
        LogUpdatingPsLine(psLine.PsLineId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(psLine);
        entity.ModifiedBy = _userClaimDto.Username;
        
        var result = await _psRepo.PsLineUpdate(entity, cancellationToken);
        
        _dbTransaction.Commit();
        LogPsLineUpdatedSuccessfully(psLine.PsLineId);
        return ServiceResponse<string>.Success(result, "Penyesuaian Stock line updated successfully.");
    }

    public async Task<ServiceResponse<string>> PsLineDelete(int psLineId, CancellationToken cancellationToken)
    {
        LogDeletingPsLine(psLineId, _userClaimDto.Username);
        var result = await _psRepo.PsLineDelete(psLineId, _userClaimDto.Username, cancellationToken);
        
        // Check for error response (format: "-1:::error message")
        if (result.StartsWith("-1:::"))
        {
            var errorMessage = result.Substring(5);
            return ServiceResponse<string>.Failure(errorMessage, StatusCodes.Status400BadRequest);
        }
        
        _dbTransaction.Commit();
        LogPsLineDeletedSuccessfully(psLineId);
        return ServiceResponse<string>.Success(result, "Penyesuaian Stock line deleted successfully.");
    }

    public async Task<ServiceResponse<PsVm>> GetPs(int recId, CancellationToken cancellationToken)
    {
        LogRetrievingFullPs(recId);
        var header = await _psRepo.PsSelectById(recId, cancellationToken);
        var lines = await _psRepo.PsLineSelectByPs(recId, cancellationToken);

        var vm = new PsVm
        {
            Header = _mapper.MapToVm(header),
            Lines = _mapper.MapToVm(lines).ToList()
        };

        return ServiceResponse<PsVm>.Success(vm, "Penyesuaian Stock retrieved successfully.");
    }

    #region Logging

    [LoggerMessage(LogLevel.Information, "Retrieving PS list")]
    partial void LogRetrievingPsList();

    [LoggerMessage(LogLevel.Information, "Retrieved {count} Penyesuaian Stock records")]
    partial void LogRetrievedCountPss(int count);

    [LoggerMessage(LogLevel.Information, "Retrieving PS by Id: {id}")]
    partial void LogRetrievingPsById(int id);

    [LoggerMessage(LogLevel.Information, "Creating new PS by user: {username}")]
    partial void LogCreatingNewPsByUser(string username);

    [LoggerMessage(LogLevel.Information, "PS created successfully - RecId: {recId}, PsId: {psId}")]
    partial void LogPsCreatedSuccessfully(int recId, string psId);

    [LoggerMessage(LogLevel.Information, "Updating PS {recId} ({psId}) by user: {username}")]
    partial void LogUpdatingPsByUser(int recId, string psId, string username);

    [LoggerMessage(LogLevel.Information, "PS {recId} updated successfully")]
    partial void LogPsUpdatedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Deleting PS {recId} by user: {username}")]
    partial void LogDeletingPsByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "PS {recId} deleted successfully")]
    partial void LogPsDeletedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Retrieving PS lines for PS RecId: {psRecId}")]
    partial void LogRetrievingPsLines(int psRecId);

    [LoggerMessage(LogLevel.Information, "Retrieving PS line by Id: {psLineId}")]
    partial void LogRetrievingPsLineById(int psLineId);

    [LoggerMessage(LogLevel.Information, "Inserting PS line for PS RecId: {psRecId} by user: {username}")]
    partial void LogInsertingPsLine(int psRecId, string username);

    [LoggerMessage(LogLevel.Information, "PS line inserted successfully for PS RecId: {psRecId}")]
    partial void LogPsLineInsertedSuccessfully(int psRecId);

    [LoggerMessage(LogLevel.Information, "Updating PS line {psLineId} by user: {username}")]
    partial void LogUpdatingPsLine(int psLineId, string username);

    [LoggerMessage(LogLevel.Information, "PS line {psLineId} updated successfully")]
    partial void LogPsLineUpdatedSuccessfully(int psLineId);

    [LoggerMessage(LogLevel.Information, "Deleting PS line {psLineId} by user: {username}")]
    partial void LogDeletingPsLine(int psLineId, string username);

    [LoggerMessage(LogLevel.Information, "PS line {psLineId} deleted successfully")]
    partial void LogPsLineDeletedSuccessfully(int psLineId);

    [LoggerMessage(LogLevel.Information, "Retrieving full PS with lines for RecId: {recId}")]
    partial void LogRetrievingFullPs(int recId);

    #endregion
}
