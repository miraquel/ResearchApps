using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface ISalesInvoiceRepo
{
    /// <summary>
    /// Si_Select - Get paginated list of sales invoices
    /// </summary>
    Task<PagedList<SalesInvoiceHeader>> SiSelect(PagedListRequest request, CancellationToken cancellationToken);
    
    /// <summary>
    /// Si_SelectById - Get SI by RecId
    /// </summary>
    Task<SalesInvoiceHeader> SiSelectById(int id, CancellationToken cancellationToken);
    
    /// <summary>
    /// Si_Insert - Create new sales invoice (returns RecId and SiId)
    /// </summary>
    Task<(int RecId, string SiId)> SiInsert(SalesInvoiceHeader salesInvoice, CancellationToken cancellationToken);
    
    /// <summary>
    /// Si_Update - Update existing sales invoice
    /// </summary>
    Task SiUpdate(SalesInvoiceHeader salesInvoice, CancellationToken cancellationToken);
    
    /// <summary>
    /// Si_Delete - Delete sales invoice (and its lines)
    /// </summary>
    Task SiDelete(int recId, string modifiedBy, CancellationToken cancellationToken);
    
    /// <summary>
    /// SiLine_SelectBySi - Get lines for a SI
    /// </summary>
    Task<IEnumerable<SalesInvoiceLine>> SiLineSelectBySi(int siRecId, CancellationToken cancellationToken);
    
    /// <summary>
    /// SiLine_SelectById - Get specific line
    /// </summary>
    Task<SalesInvoiceLine?> SiLineSelectById(int siLineId, CancellationToken cancellationToken);
    
    /// <summary>
    /// SiLine_Insert - Insert SI line
    /// </summary>
    Task<string> SiLineInsert(SalesInvoiceLine salesInvoiceLine, CancellationToken cancellationToken);
}
