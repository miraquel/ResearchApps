using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Common.Exceptions;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class SupplierService : ISupplierService
{
    private readonly ISupplierRepo _supplierRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<SupplierService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public SupplierService(ISupplierRepo supplierRepo, IDbTransaction dbTransaction, 
        UserClaimDto userClaimDto, ILogger<SupplierService> logger)
    {
        _supplierRepo = supplierRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<SupplierVm>>> SupplierSelect(CancellationToken cancellationToken)
    {
        LogRetrievingSuppliersList();
        var suppliers = await _supplierRepo.SupplierSelect(cancellationToken);
        LogRetrievedCountSuppliers(suppliers.Count());
        return ServiceResponse<IEnumerable<SupplierVm>>.Success(_mapper.MapToVm(suppliers), "Suppliers retrieved successfully.");
    }

    public async Task<ServiceResponse<PagedListVm<SupplierVm>>> SupplierSelect(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogRetrievingSupplierListPagePageSize(request.PageNumber, request.PageSize);
        var suppliers = await _supplierRepo.SupplierSelect(_mapper.MapToEntity(request), cancellationToken);
        LogRetrievedCountSuppliers(suppliers.TotalCount);
        return ServiceResponse<PagedListVm<SupplierVm>>.Success(_mapper.MapToVm(suppliers), "Suppliers retrieved successfully.");
    }

    public async Task<ServiceResponse<SupplierVm>> SupplierSelectById(int supplierId, CancellationToken cancellationToken)
    {
        LogRetrievingSupplierById(supplierId);
        var supplier = await _supplierRepo.SupplierSelectById(supplierId, cancellationToken);
        
        if (supplier == null)
        {
            LogSupplierNotFound(supplierId);
            return ServiceResponse<SupplierVm>.Failure("Supplier not found.", StatusCodes.Status404NotFound);
        }
        
        return ServiceResponse<SupplierVm>.Success(_mapper.MapToVm(supplier), "Supplier retrieved successfully.");
    }

    public async Task<ServiceResponse<SupplierVm>> SupplierInsert(SupplierVm supplierVm, CancellationToken cancellationToken)
    {
        LogCreatingNewSupplierByUser(supplierVm.SupplierName, _userClaimDto.Username);
        
        try
        {
            var entity = _mapper.MapToEntity(supplierVm);
            entity.CreatedBy = _userClaimDto.Username;
            var result = await _supplierRepo.SupplierInsert(entity, cancellationToken);
            _dbTransaction.Commit();
            LogSupplierCreatedSuccessfully(result.SupplierId);
            return ServiceResponse<SupplierVm>.Success(_mapper.MapToVm(result), "Supplier created successfully.", StatusCodes.Status201Created);
        }
        catch (RepoException<Domain.Supplier> ex)
        {
            LogSupplierNameAlreadyExists(supplierVm.SupplierName);
            return ServiceResponse<SupplierVm>.Failure(ex.Message, StatusCodes.Status400BadRequest);
        }
    }

    public async Task<ServiceResponse<SupplierVm>> SupplierUpdate(SupplierVm supplierVm, CancellationToken cancellationToken)
    {
        LogUpdatingSupplierByUser(supplierVm.SupplierId, supplierVm.SupplierName, _userClaimDto.Username);
        
        try
        {
            var entity = _mapper.MapToEntity(supplierVm);
            entity.ModifiedBy = _userClaimDto.Username;
            var result = await _supplierRepo.SupplierUpdate(entity, cancellationToken);
            _dbTransaction.Commit();
            LogSupplierUpdatedSuccessfully(supplierVm.SupplierId);
            return ServiceResponse<SupplierVm>.Success(_mapper.MapToVm(result), "Supplier updated successfully.");
        }
        catch (RepoException<Domain.Supplier> ex)
        {
            LogSupplierNameAlreadyExists(supplierVm.SupplierName);
            return ServiceResponse<SupplierVm>.Failure(ex.Message, StatusCodes.Status400BadRequest);
        }
    }

    public async Task<ServiceResponse> SupplierDelete(int supplierId, CancellationToken cancellationToken)
    {
        LogDeletingSupplierByUser(supplierId, _userClaimDto.Username);
        await _supplierRepo.SupplierDelete(supplierId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        LogSupplierDeletedSuccessfully(supplierId);
        return ServiceResponse.Success("Supplier deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<SupplierVm>>> SupplierCbo(CancellationToken cancellationToken)
    {
        var suppliers = await _supplierRepo.SupplierCbo(cancellationToken);
        return ServiceResponse<IEnumerable<SupplierVm>>.Success(_mapper.MapToVm(suppliers), "Suppliers retrieved successfully.");
    }

    // Logging methods
    [LoggerMessage(LogLevel.Information, "Retrieving suppliers list")]
    partial void LogRetrievingSuppliersList();

    [LoggerMessage(LogLevel.Information, "Retrieving supplier list - Page: {PageNumber}, PageSize: {PageSize}")]
    partial void LogRetrievingSupplierListPagePageSize(int pageNumber, int pageSize);

    [LoggerMessage(LogLevel.Information, "Retrieved {Count} suppliers")]
    partial void LogRetrievedCountSuppliers(int count);

    [LoggerMessage(LogLevel.Information, "Retrieving supplier by ID: {SupplierId}")]
    partial void LogRetrievingSupplierById(int supplierId);

    [LoggerMessage(LogLevel.Warning, "Supplier not found: {SupplierId}")]
    partial void LogSupplierNotFound(int supplierId);

    [LoggerMessage(LogLevel.Information, "Creating new supplier '{SupplierName}' by user '{Username}'")]
    partial void LogCreatingNewSupplierByUser(string supplierName, string username);

    [LoggerMessage(LogLevel.Information, "Supplier created successfully with ID: {SupplierId}")]
    partial void LogSupplierCreatedSuccessfully(int supplierId);

    [LoggerMessage(LogLevel.Warning, "Supplier name already exists: '{SupplierName}'")]
    partial void LogSupplierNameAlreadyExists(string supplierName);

    [LoggerMessage(LogLevel.Information, "Updating supplier ID {SupplierId} ('{SupplierName}') by user '{Username}'")]
    partial void LogUpdatingSupplierByUser(int supplierId, string supplierName, string username);

    [LoggerMessage(LogLevel.Information, "Supplier updated successfully: {SupplierId}")]
    partial void LogSupplierUpdatedSuccessfully(int supplierId);

    [LoggerMessage(LogLevel.Information, "Deleting supplier ID {SupplierId} by user '{Username}'")]
    partial void LogDeletingSupplierByUser(int supplierId, string username);

    [LoggerMessage(LogLevel.Information, "Supplier deleted successfully: {SupplierId}")]
    partial void LogSupplierDeletedSuccessfully(int supplierId);
}
