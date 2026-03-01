using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IMaterialCustomerRepo
{
    // Mc_Select - Get paginated list of material customers
    Task<PagedList<MaterialCustomerHeader>> McSelect(PagedListRequest request, CancellationToken cancellationToken);
    
    // Mc_SelectById - Get MC by RecId
    Task<MaterialCustomerHeader> McSelectById(int id, CancellationToken cancellationToken);
    
    // Mc_Insert - Create new material customer (returns RecId and McId)
    Task<(int RecId, string McId)> McInsert(MaterialCustomerHeader materialCustomer, CancellationToken cancellationToken);
    
    // Mc_Update - Update existing material customer
    Task McUpdate(MaterialCustomerHeader materialCustomer, CancellationToken cancellationToken);
    
    // Mc_Delete - Delete material customer (and its lines)
    Task McDelete(int recId, string modifiedBy, CancellationToken cancellationToken);
    
    // McLine_SelectByMc - Get lines for a MC
    Task<IEnumerable<MaterialCustomerLine>> McLineSelectByMc(int mcRecId, CancellationToken cancellationToken);
    
    // McLine_SelectById - Get specific line
    Task<MaterialCustomerLine?> McLineSelectById(int mcLineId, CancellationToken cancellationToken);
    
    // McLine_Insert - Insert MC line
    Task<string> McLineInsert(MaterialCustomerLine materialCustomerLine, CancellationToken cancellationToken);
    
    // McLine_Delete - Delete MC line
    Task<string> McLineDelete(int mcLineId, string modifiedBy, CancellationToken cancellationToken);
}
