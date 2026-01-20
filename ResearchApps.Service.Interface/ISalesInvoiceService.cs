using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface ISalesInvoiceService
{
    /// <summary>
    /// Si_Select - Get paginated list of sales invoices
    /// </summary>
    Task<ServiceResponse<PagedListVm<SalesInvoiceHeaderVm>>> SiSelect(PagedListRequestVm request, CancellationToken cancellationToken);
    
    /// <summary>
    /// Si_SelectById - Get SI by RecId
    /// </summary>
    Task<ServiceResponse<SalesInvoiceHeaderVm>> SiSelectById(int id, CancellationToken cancellationToken);
    
    /// <summary>
    /// Si_Insert - Create new sales invoice with lines (returns RecId and SiId)
    /// </summary>
    Task<ServiceResponse<(int RecId, string SiId)>> SiInsert(SalesInvoiceVm salesInvoice, CancellationToken cancellationToken);
    
    /// <summary>
    /// Si_Update - Update existing sales invoice header
    /// </summary>
    Task<ServiceResponse> SiUpdate(SalesInvoiceHeaderVm salesInvoiceHeader, CancellationToken cancellationToken);
    
    /// <summary>
    /// Si_Delete - Delete sales invoice and its lines
    /// </summary>
    Task<ServiceResponse> SiDelete(int recId, CancellationToken cancellationToken);
    
    /// <summary>
    /// SiLine_SelectBySi - Get lines for a SI
    /// </summary>
    Task<ServiceResponse<IEnumerable<SalesInvoiceLineVm>>> SiLineSelectBySi(int siRecId, CancellationToken cancellationToken);
    
    /// <summary>
    /// SiLine_SelectById - Get specific line
    /// </summary>
    Task<ServiceResponse<SalesInvoiceLineVm>> SiLineSelectById(int siLineId, CancellationToken cancellationToken);
    
    /// <summary>
    /// SiLine_Insert - Insert SI line
    /// </summary>
    Task<ServiceResponse<string>> SiLineInsert(SalesInvoiceLineVm salesInvoiceLine, CancellationToken cancellationToken);
    
    /// <summary>
    /// Get composite Sales Invoice with Header and Lines
    /// </summary>
    Task<ServiceResponse<SalesInvoiceVm>> GetSalesInvoice(int recId, CancellationToken cancellationToken);
}
