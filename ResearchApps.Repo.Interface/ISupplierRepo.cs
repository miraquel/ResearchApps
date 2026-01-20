using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface ISupplierRepo
{
    Task<IEnumerable<Supplier>> SupplierSelect(CancellationToken cancellationToken);
    Task<PagedList<Supplier>> SupplierSelect(PagedListRequest request, CancellationToken cancellationToken);
    Task<IEnumerable<Supplier>> SupplierSelectForExport(PagedListRequest request, CancellationToken cancellationToken);
    Task<Supplier?> SupplierSelectById(int supplierId, CancellationToken cancellationToken);
    Task<Supplier> SupplierInsert(Supplier supplier, CancellationToken cancellationToken);
    Task<Supplier> SupplierUpdate(Supplier supplier, CancellationToken cancellationToken);
    Task SupplierDelete(int supplierId, string modifiedBy, CancellationToken cancellationToken);
    Task<IEnumerable<Supplier>> SupplierCbo(CancellationToken cancellationToken);
}
