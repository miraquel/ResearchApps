using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class MaterialCustomerService : IMaterialCustomerService
{
    private readonly IMaterialCustomerRepo _materialCustomerRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<MaterialCustomerService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public MaterialCustomerService(IMaterialCustomerRepo materialCustomerRepo, IDbTransaction dbTransaction,
        UserClaimDto userClaimDto, ILogger<MaterialCustomerService> logger)
    {
        _materialCustomerRepo = materialCustomerRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PagedListVm<MaterialCustomerHeaderVm>>> McSelect(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogRetrievingMcListPagePageSize(request.PageNumber, request.PageSize);
        var mcs = await _materialCustomerRepo.McSelect(_mapper.MapToEntity(request), cancellationToken);
        LogRetrievedCountMcs(mcs.TotalCount);
        return ServiceResponse<PagedListVm<MaterialCustomerHeaderVm>>.Success(_mapper.MapToVm(mcs), "Material Customers retrieved successfully.");
    }

    public async Task<ServiceResponse<MaterialCustomerHeaderVm>> McSelectById(int id, CancellationToken cancellationToken)
    {
        LogRetrievingMcById(id);
        var mc = await _materialCustomerRepo.McSelectById(id, cancellationToken);
        return ServiceResponse<MaterialCustomerHeaderVm>.Success(_mapper.MapToVm(mc), "Material Customer retrieved successfully.");
    }

    public async Task<ServiceResponse<(int RecId, string McId)>> McInsert(MaterialCustomerVm materialCustomer,
        CancellationToken cancellationToken)
    {
        LogCreatingNewMcByUser(_userClaimDto.Username);
        var entity = _mapper.MapFromCompositeVm(materialCustomer);
        entity.CreatedBy = _userClaimDto.Username;
        var result = await _materialCustomerRepo.McInsert(entity, cancellationToken);

        // Insert lines if any
        foreach (var lineVm in materialCustomer.Lines)
        {
            var line = _mapper.MapToEntity(lineVm);
            line.RecId = result.RecId;
            line.CreatedBy = _userClaimDto.Username;
            await _materialCustomerRepo.McLineInsert(line, cancellationToken);
        }

        _dbTransaction.Commit();
        LogMcCreatedSuccessfully(result.RecId, result.McId);
        return ServiceResponse<(int RecId, string McId)>.Success(result, "Material Customer created successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> McUpdate(MaterialCustomerHeaderVm materialCustomerHeader, CancellationToken cancellationToken)
    {
        LogUpdatingMcByUser(materialCustomerHeader.RecId, materialCustomerHeader.McId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(materialCustomerHeader);
        entity.ModifiedBy = _userClaimDto.Username;
        await _materialCustomerRepo.McUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogMcUpdatedSuccessfully(materialCustomerHeader.RecId);
        return ServiceResponse.Success("Material Customer updated successfully.");
    }

    public async Task<ServiceResponse> McDelete(int recId, CancellationToken cancellationToken)
    {
        LogDeletingMcByUser(recId, _userClaimDto.Username);
        await _materialCustomerRepo.McDelete(recId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogMcDeletedSuccessfully(recId);
        return ServiceResponse.Success("Material Customer deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<MaterialCustomerLineVm>>> McLineSelectByMc(int mcRecId, CancellationToken cancellationToken)
    {
        LogRetrievingMcLines(mcRecId);
        var lines = await _materialCustomerRepo.McLineSelectByMc(mcRecId, cancellationToken);
        var linesVm = _mapper.MapToVm(lines);
        return ServiceResponse<IEnumerable<MaterialCustomerLineVm>>.Success(linesVm, "Material Customer lines retrieved successfully.");
    }

    public async Task<ServiceResponse<MaterialCustomerLineVm>> McLineSelectById(int mcLineId, CancellationToken cancellationToken)
    {
        LogRetrievingMcLineById(mcLineId);
        var line = await _materialCustomerRepo.McLineSelectById(mcLineId, cancellationToken);
        if (line == null)
        {
            return ServiceResponse<MaterialCustomerLineVm>.Failure($"Material Customer line with Id {mcLineId} not found.", StatusCodes.Status404NotFound);
        }
        return ServiceResponse<MaterialCustomerLineVm>.Success(_mapper.MapToVm(line), "Material Customer line retrieved successfully.");
    }

    public async Task<ServiceResponse<string>> McLineInsert(MaterialCustomerLineVm materialCustomerLine, CancellationToken cancellationToken)
    {
        LogCreatingMcLineByUser(materialCustomerLine.RecId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(materialCustomerLine);
        entity.CreatedBy = _userClaimDto.Username;
        var result = await _materialCustomerRepo.McLineInsert(entity, cancellationToken);
        _dbTransaction.Commit();
        LogMcLineCreatedSuccessfully(materialCustomerLine.RecId);
        return ServiceResponse<string>.Success(result, "Material Customer line created successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> McLineDelete(int mcLineId, CancellationToken cancellationToken)
    {
        LogDeletingMcLineByUser(mcLineId, _userClaimDto.Username);
        await _materialCustomerRepo.McLineDelete(mcLineId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogMcLineDeletedSuccessfully(mcLineId);
        return ServiceResponse.Success("Material Customer line deleted successfully.");
    }

    public async Task<ServiceResponse<MaterialCustomerVm>> GetMaterialCustomer(int recId, CancellationToken cancellationToken)
    {
        LogRetrievingCompleteMc(recId);
        var header = await _materialCustomerRepo.McSelectById(recId, cancellationToken);
        var lines = await _materialCustomerRepo.McLineSelectByMc(recId, cancellationToken);
        var vm = _mapper.MapToCompositeVm(header, lines);
        return ServiceResponse<MaterialCustomerVm>.Success(vm, "Material Customer retrieved successfully.");
    }

    #region Logging

    [LoggerMessage(LogLevel.Information, "Retrieving MC list - Page: {page}, PageSize: {pageSize}")]
    partial void LogRetrievingMcListPagePageSize(int page, int pageSize);

    [LoggerMessage(LogLevel.Information, "Retrieved {count} Material Customers")]
    partial void LogRetrievedCountMcs(int count);

    [LoggerMessage(LogLevel.Information, "Retrieving MC by Id: {id}")]
    partial void LogRetrievingMcById(int id);

    [LoggerMessage(LogLevel.Information, "Creating new MC by user: {username}")]
    partial void LogCreatingNewMcByUser(string username);

    [LoggerMessage(LogLevel.Information, "MC created successfully - RecId: {recId}, McId: {mcId}")]
    partial void LogMcCreatedSuccessfully(int recId, string mcId);

    [LoggerMessage(LogLevel.Information, "Updating MC {recId} ({mcId}) by user: {username}")]
    partial void LogUpdatingMcByUser(int recId, string mcId, string username);

    [LoggerMessage(LogLevel.Information, "MC updated successfully - RecId: {recId}")]
    partial void LogMcUpdatedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Deleting MC {recId} by user: {username}")]
    partial void LogDeletingMcByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "MC deleted successfully - RecId: {recId}")]
    partial void LogMcDeletedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Retrieving MC lines for RecId: {recId}")]
    partial void LogRetrievingMcLines(int recId);

    [LoggerMessage(LogLevel.Information, "Retrieving MC line by Id: {mcLineId}")]
    partial void LogRetrievingMcLineById(int mcLineId);

    [LoggerMessage(LogLevel.Information, "Creating MC line for RecId: {recId} by user: {username}")]
    partial void LogCreatingMcLineByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "MC line created successfully for RecId: {recId}")]
    partial void LogMcLineCreatedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Deleting MC line {mcLineId} by user: {username}")]
    partial void LogDeletingMcLineByUser(int mcLineId, string username);

    [LoggerMessage(LogLevel.Information, "MC line deleted successfully - McLineId: {mcLineId}")]
    partial void LogMcLineDeletedSuccessfully(int mcLineId);

    [LoggerMessage(LogLevel.Information, "Retrieving complete MC with lines for RecId: {recId}")]
    partial void LogRetrievingCompleteMc(int recId);

    #endregion
}
