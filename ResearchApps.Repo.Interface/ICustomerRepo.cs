using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface ICustomerRepo
{
    // Customer_Select - Get paginated list of customers
    Task<PagedList<Customer>> CustomerSelect(PagedListRequest request, CancellationToken cancellationToken);
    
    // Customer_SelectById - Get customer by ID
    Task<Customer> CustomerSelectById(int id, CancellationToken cancellationToken);
    
    // Customer_Insert - Create new customer
    Task<int> CustomerInsert(Customer customer, CancellationToken cancellationToken);
    
    // Customer_Update - Update existing customer
    Task CustomerUpdate(Customer customer, CancellationToken cancellationToken);
    
    // Customer_Delete - Delete customer
    Task CustomerDelete(int id, CancellationToken cancellationToken);
    
    // Customer_Cbo - Get customers for dropdown
    Task<IEnumerable<Customer>> CustomerCbo(CboRequest request, CancellationToken cancellationToken);
}
