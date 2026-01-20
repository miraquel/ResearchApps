using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class SalesInvoiceService : ISalesInvoiceService
{
    private readonly ISalesInvoiceRepo _salesInvoiceRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<SalesInvoiceService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public SalesInvoiceService(ISalesInvoiceRepo salesInvoiceRepo, IDbTransaction dbTransaction,
        UserClaimDto userClaimDto, ILogger<SalesInvoiceService> logger)
    {
        _salesInvoiceRepo = salesInvoiceRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PagedListVm<SalesInvoiceHeaderVm>>> SiSelect(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogRetrievingSiListPagePageSize(request.PageNumber, request.PageSize);
        var sis = await _salesInvoiceRepo.SiSelect(_mapper.MapToEntity(request), cancellationToken);
        LogRetrievedCountSis(sis.TotalCount);
        return ServiceResponse<PagedListVm<SalesInvoiceHeaderVm>>.Success(_mapper.MapToVm(sis), "Sales Invoices retrieved successfully.");
    }

    public async Task<ServiceResponse<SalesInvoiceHeaderVm>> SiSelectById(int id, CancellationToken cancellationToken)
    {
        LogRetrievingSiById(id);
        var si = await _salesInvoiceRepo.SiSelectById(id, cancellationToken);
        return ServiceResponse<SalesInvoiceHeaderVm>.Success(_mapper.MapToVm(si), "Sales Invoice retrieved successfully.");
    }

    public async Task<ServiceResponse<(int RecId, string SiId)>> SiInsert(SalesInvoiceVm salesInvoice, CancellationToken cancellationToken)
    {
        LogCreatingNewSiByUser(_userClaimDto.Username);
        var entity = _mapper.MapToEntity(salesInvoice);
        entity.Header.CreatedBy = _userClaimDto.Username;
        var result = await _salesInvoiceRepo.SiInsert(entity.Header, cancellationToken);

        // Insert lines
        foreach (var line in entity.Lines)
        {
            line.SiRecId = result.RecId;
            line.CreatedBy = _userClaimDto.Username;
            await _salesInvoiceRepo.SiLineInsert(line, cancellationToken);
        }

        // Calculate and update total amount if there are lines
        // (Amount is auto-calculated from lines)
        
        _dbTransaction.Commit();
        LogSiCreatedSuccessfully(result.RecId, result.SiId);
        return ServiceResponse<(int RecId, string SiId)>.Success(result, "Sales Invoice created successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> SiUpdate(SalesInvoiceHeaderVm salesInvoiceHeader, CancellationToken cancellationToken)
    {
        LogUpdatingSiByUser(salesInvoiceHeader.RecId, salesInvoiceHeader.SiId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(salesInvoiceHeader);
        entity.ModifiedBy = _userClaimDto.Username;
        await _salesInvoiceRepo.SiUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogSiUpdatedSuccessfully(salesInvoiceHeader.RecId);
        return ServiceResponse.Success("Sales Invoice updated successfully.");
    }

    public async Task<ServiceResponse> SiDelete(int recId, CancellationToken cancellationToken)
    {
        LogDeletingSiByUser(recId, _userClaimDto.Username);
        await _salesInvoiceRepo.SiDelete(recId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogSiDeletedSuccessfully(recId);
        return ServiceResponse.Success("Sales Invoice deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<SalesInvoiceLineVm>>> SiLineSelectBySi(int siRecId, CancellationToken cancellationToken)
    {
        LogRetrievingSiLines(siRecId);
        var lines = await _salesInvoiceRepo.SiLineSelectBySi(siRecId, cancellationToken);
        return ServiceResponse<IEnumerable<SalesInvoiceLineVm>>.Success(_mapper.MapToVm(lines), "Sales Invoice lines retrieved successfully.");
    }

    public async Task<ServiceResponse<SalesInvoiceLineVm>> SiLineSelectById(int siLineId, CancellationToken cancellationToken)
    {
        LogRetrievingSiLineById(siLineId);
        var line = await _salesInvoiceRepo.SiLineSelectById(siLineId, cancellationToken);
        if (line == null)
        {
            return ServiceResponse<SalesInvoiceLineVm>.Failure($"Sales Invoice line with Id {siLineId} not found.");
        }
        return ServiceResponse<SalesInvoiceLineVm>.Success(_mapper.MapToVm(line), "Sales Invoice line retrieved successfully.");
    }

    public async Task<ServiceResponse<string>> SiLineInsert(SalesInvoiceLineVm salesInvoiceLine, CancellationToken cancellationToken)
    {
        LogInsertingSiLine(salesInvoiceLine.SiRecId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(salesInvoiceLine);
        entity.CreatedBy = _userClaimDto.Username;
        var result = await _salesInvoiceRepo.SiLineInsert(entity, cancellationToken);
        _dbTransaction.Commit();
        LogSiLineInsertedSuccessfully(salesInvoiceLine.SiRecId);
        return ServiceResponse<string>.Success(result, "Sales Invoice line inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<SalesInvoiceVm>> GetSalesInvoice(int recId, CancellationToken cancellationToken)
    {
        LogRetrievingFullSi(recId);
        var header = await _salesInvoiceRepo.SiSelectById(recId, cancellationToken);
        var lines = await _salesInvoiceRepo.SiLineSelectBySi(recId, cancellationToken);

        var vm = new SalesInvoiceVm
        {
            Header = _mapper.MapToVm(header),
            Lines = _mapper.MapToVm(lines).ToList()
        };

        return ServiceResponse<SalesInvoiceVm>.Success(vm, "Sales Invoice retrieved successfully.");
    }

    #region Logging

    [LoggerMessage(LogLevel.Information, "Retrieving SI list - Page: {page}, PageSize: {pageSize}")]
    partial void LogRetrievingSiListPagePageSize(int page, int pageSize);

    [LoggerMessage(LogLevel.Information, "Retrieved {count} Sales Invoices")]
    partial void LogRetrievedCountSis(int count);

    [LoggerMessage(LogLevel.Information, "Retrieving SI by Id: {id}")]
    partial void LogRetrievingSiById(int id);

    [LoggerMessage(LogLevel.Information, "Creating new SI by user: {username}")]
    partial void LogCreatingNewSiByUser(string username);

    [LoggerMessage(LogLevel.Information, "SI created successfully - RecId: {recId}, SiId: {siId}")]
    partial void LogSiCreatedSuccessfully(int recId, string siId);

    [LoggerMessage(LogLevel.Information, "Updating SI {recId} ({siId}) by user: {username}")]
    partial void LogUpdatingSiByUser(int recId, string siId, string username);

    [LoggerMessage(LogLevel.Information, "SI {recId} updated successfully")]
    partial void LogSiUpdatedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Deleting SI {recId} by user: {username}")]
    partial void LogDeletingSiByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "SI {recId} deleted successfully")]
    partial void LogSiDeletedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Retrieving SI lines for SI RecId: {siRecId}")]
    partial void LogRetrievingSiLines(int siRecId);

    [LoggerMessage(LogLevel.Information, "Retrieving SI line by Id: {siLineId}")]
    partial void LogRetrievingSiLineById(int siLineId);

    [LoggerMessage(LogLevel.Information, "Inserting SI line for SI RecId: {siRecId} by user: {username}")]
    partial void LogInsertingSiLine(int siRecId, string username);

    [LoggerMessage(LogLevel.Information, "SI line inserted successfully for SI RecId: {siRecId}")]
    partial void LogSiLineInsertedSuccessfully(int siRecId);

    [LoggerMessage(LogLevel.Information, "Retrieving full SI with lines for RecId: {recId}")]
    partial void LogRetrievingFullSi(int recId);

    #endregion
}
