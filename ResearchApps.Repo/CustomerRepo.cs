using System.Data;
using Dapper;
using ResearchApps.Common.Exceptions;
using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public class CustomerRepo : ICustomerRepo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public CustomerRepo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public async Task<PagedList<Customer>> CustomerSelect(PagedListRequest request, CancellationToken cancellationToken)
    {
        const string query = "Customer_Select";
        var parameters = new DynamicParameters();
        
        foreach (var filter in request.Filters)
        {
            if (filter.Value is { } strValue && strValue.Contains('%'))
            {
                parameters.Add($"@{filter.Key}", strValue);
            }
            else
            {
                parameters.Add($"@{filter.Key}", $"%{filter.Value}%");
            }
        }
        
        parameters.Add("@PageNumber", request.PageNumber);
        parameters.Add("@PageSize", request.PageSize);
        // parameters.Add("@SortOrder", request.IsSortAscending ? "ASC" : "DESC");
        // parameters.Add("@SortColumn", request.SortBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

        var result = await _dbConnection.QueryMultipleAsync(command);
        var items = result.Read<Customer>();
        var totalCount = result.ReadSingle<int>();
        
        return new PagedList<Customer>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<Customer> CustomerSelectById(int id, CancellationToken cancellationToken)
    {
        const string query = "Customer_SelectById";
        var parameters = new DynamicParameters();
        parameters.Add("@CustomerId", id);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QueryFirstOrDefaultAsync<Customer>(command);
        
        if (result == null)
        {
            throw new RepoException($"Customer with Id {id} not found.");
        }

        return result;
    }

    public async Task<int> CustomerInsert(Customer customer, CancellationToken cancellationToken)
    {
        const string query = "Customer_Insert";
        var parameters = new DynamicParameters();
        
        parameters.Add("@CustomerName", customer.CustomerName);
        parameters.Add("@Address", customer.Address);
        parameters.Add("@City", customer.City ?? string.Empty);
        parameters.Add("@Telp", customer.Telp ?? string.Empty);
        parameters.Add("@Fax", customer.Fax ?? string.Empty);
        parameters.Add("@Email", customer.Email ?? string.Empty);
        parameters.Add("@TopId", customer.TopId);
        parameters.Add("@IsPpn", customer.IsPpn);
        parameters.Add("@Npwp", customer.Npwp ?? string.Empty);
        parameters.Add("@Notes", customer.Notes ?? string.Empty);
        parameters.Add("@StatusId", customer.StatusId);
        parameters.Add("@CreatedBy", customer.CreatedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.QuerySingleAsync<int>(command);
        return result;
    }

    public async Task CustomerUpdate(Customer customer, CancellationToken cancellationToken)
    {
        const string query = "Customer_Update";
        var parameters = new DynamicParameters();
        
        parameters.Add("@CustomerId", customer.CustomerId);
        parameters.Add("@CustomerName", customer.CustomerName);
        parameters.Add("@Address", customer.Address);
        parameters.Add("@City", customer.City ?? string.Empty);
        parameters.Add("@Telp", customer.Telp ?? string.Empty);
        parameters.Add("@Fax", customer.Fax ?? string.Empty);
        parameters.Add("@Email", customer.Email ?? string.Empty);
        parameters.Add("@TopId", customer.TopId);
        parameters.Add("@IsPpn", customer.IsPpn);
        parameters.Add("@Npwp", customer.Npwp ?? string.Empty);
        parameters.Add("@Notes", customer.Notes ?? string.Empty);
        parameters.Add("@StatusId", customer.StatusId);
        parameters.Add("@ModifiedBy", customer.ModifiedBy);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.ExecuteAsync(command);
        
        if (result == 0)
        {
            throw new RepoException($"Customer with Id {customer.CustomerId} could not be updated.");
        }
    }

    public async Task CustomerDelete(int id, CancellationToken cancellationToken)
    {
        const string query = "Customer_Delete";
        var parameters = new DynamicParameters();
        parameters.Add("@CustomerId", id);

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        var result = await _dbConnection.ExecuteAsync(command);
        
        if (result == 0)
        {
            throw new RepoException($"Customer with Id {id} could not be deleted.");
        }
    }

    public async Task<IEnumerable<Customer>> CustomerCbo(CboRequest request, CancellationToken cancellationToken)
    {
        const string query = "Customer_Cbo";
        var parameters = new DynamicParameters();
        
        if (request.Id.HasValue)
        {
            parameters.Add("@Id", request.Id.Value);
        }
        if (!string.IsNullOrEmpty(request.Term))
        {
            parameters.Add("@Term", request.Term);
        }

        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
        
        var command = new CommandDefinition(
            query,
            parameters,
            _dbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        
        return await _dbConnection.QueryAsync<Customer>(command);
    }
}
