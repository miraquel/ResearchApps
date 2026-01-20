using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class CustomerService : ICustomerService
{
    private readonly ICustomerRepo _customerRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<CustomerService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public CustomerService(ICustomerRepo customerRepo, IDbTransaction dbTransaction, 
        UserClaimDto userClaimDto, ILogger<CustomerService> logger)
    {
        _customerRepo = customerRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<PagedListVm<CustomerVm>>> CustomerSelect(PagedListRequestVm request, CancellationToken cancellationToken)
    {
        LogRetrievingCustomersListPagePagesize(request.PageNumber, request.PageSize);
        var customers = await _customerRepo.CustomerSelect(_mapper.MapToEntity(request), cancellationToken);
        LogRetrievedCountCustomers(customers.TotalCount);
        return ServiceResponse<PagedListVm<CustomerVm>>.Success(_mapper.MapToVm(customers), "Customers retrieved successfully.");
    }

    public async Task<ServiceResponse<CustomerVm>> CustomerSelectById(int id, CancellationToken cancellationToken)
    {
        LogRetrievingCustomerById(id);
        var customer = await _customerRepo.CustomerSelectById(id, cancellationToken);
        return ServiceResponse<CustomerVm>.Success(_mapper.MapToVm(customer), "Customer retrieved successfully.");
    }

    public async Task<ServiceResponse<int>> CustomerInsert(CustomerVm customer, CancellationToken cancellationToken)
    {
        LogCreatingNewCustomerByUser(customer.CustomerName, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(customer);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedId = await _customerRepo.CustomerInsert(entity, cancellationToken);
        _dbTransaction.Commit();
        LogCustomerCreatedSuccessfully(insertedId);
        return ServiceResponse<int>.Success(insertedId, "Customer created successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse> CustomerUpdate(CustomerVm customer, CancellationToken cancellationToken)
    {
        LogUpdatingCustomerByUser(customer.CustomerId, customer.CustomerName, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(customer);
        entity.ModifiedBy = _userClaimDto.Username;
        await _customerRepo.CustomerUpdate(entity, cancellationToken);
        _dbTransaction.Commit();
        LogCustomerUpdatedSuccessfully(customer.CustomerId);
        return ServiceResponse.Success("Customer updated successfully.");
    }

    public async Task<ServiceResponse> CustomerDelete(int id, CancellationToken cancellationToken)
    {
        LogDeletingCustomerByUser(id, _userClaimDto.Username);
        await _customerRepo.CustomerDelete(id, cancellationToken);
        _dbTransaction.Commit();
        LogCustomerDeletedSuccessfully(id);
        return ServiceResponse.Success("Customer deleted successfully.");
    }

    public async Task<ServiceResponse<IEnumerable<CustomerVm>>> CustomerCbo(CboRequestVm request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepo.CustomerCbo(_mapper.MapToEntity(request), cancellationToken);
        return ServiceResponse<IEnumerable<CustomerVm>>.Success(_mapper.MapToVm(customers), "Customers retrieved successfully.");
    }

    [LoggerMessage(LogLevel.Debug, "Retrieving customers list - Page: {page}, PageSize: {pageSize}")]
    partial void LogRetrievingCustomersListPagePagesize(int page, int pageSize);

    [LoggerMessage(LogLevel.Debug, "Retrieved {count} customers")]
    partial void LogRetrievedCountCustomers(int count);

    [LoggerMessage(LogLevel.Debug, "Retrieving customer by Id: {id}")]
    partial void LogRetrievingCustomerById(int id);

    [LoggerMessage(LogLevel.Information, "Creating new customer: {customerName} by user: {username}")]
    partial void LogCreatingNewCustomerByUser(string customerName, string username);

    [LoggerMessage(LogLevel.Information, "Customer created successfully with Id: {id}")]
    partial void LogCustomerCreatedSuccessfully(int id);

    [LoggerMessage(LogLevel.Information, "Updating customer {id}: {customerName} by user: {username}")]
    partial void LogUpdatingCustomerByUser(int id, string customerName, string username);

    [LoggerMessage(LogLevel.Information, "Customer {id} updated successfully")]
    partial void LogCustomerUpdatedSuccessfully(int id);

    [LoggerMessage(LogLevel.Information, "Deleting customer with Id: {id} by user: {username}")]
    partial void LogDeletingCustomerByUser(int id, string username);

    [LoggerMessage(LogLevel.Information, "Customer {id} deleted successfully")]
    partial void LogCustomerDeletedSuccessfully(int id);
}
