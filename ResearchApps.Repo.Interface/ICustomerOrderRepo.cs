using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface ICustomerOrderRepo
{
    // Co_Select - Get paginated list of customer orders
    Task<PagedList<CustomerOrderHeader>> CoSelect(PagedListRequest request, CancellationToken cancellationToken);
    
    // Co_SelectForExport - Get all customer orders for export (no pagination)
    Task<IEnumerable<CustomerOrderHeader>> CoSelectForExport(PagedListRequest request, CancellationToken cancellationToken);
    
    // Co_SelectById - Get CO by RecId
    Task<CustomerOrderHeader> CoSelectById(int id, CancellationToken cancellationToken);
    
    // Co_Insert - Create new customer order (returns RecId and CoId)
    Task<(int RecId, string CoId)> CoInsert(CustomerOrderHeader customerOrder, CancellationToken cancellationToken);
    
    // Co_Update - Update existing customer order
    Task CoUpdate(CustomerOrderHeader customerOrder, CancellationToken cancellationToken);
    
    // Co_Delete - Delete customer order (and its lines)
    Task CoDelete(int recId, CancellationToken cancellationToken);
    
    // Co_SubmitById - Submit CO for processing
    Task CoSubmitById(int recId, string modifiedBy, CancellationToken cancellationToken);
    
    // Co_RecallById - Recall submitted CO
    Task CoRecallById(int recId, string modifiedBy, CancellationToken cancellationToken);
    
    // Co_RejectById - Reject CO
    Task CoRejectById(int recId, string modifiedBy, string notes, CancellationToken cancellationToken);
    
    // Co_CloseByNo - Close CO by CoId
    Task CoCloseByNo(string coId, string modifiedBy, CancellationToken cancellationToken);
    
    // Co_ApproveById - Approve CO
    Task CoApproveById(int recId, string modifiedBy, string notes, CancellationToken cancellationToken);
    
    // Co_HdOsSelect - Get outstanding CO headers for a customer
    Task<IEnumerable<CustomerOrderHeaderOutstanding>> CoHdOsSelect(int customerId, CancellationToken cancellationToken);
    
    // Co_OsSelect - Get outstanding CO lines for a customer
    Task<IEnumerable<CustomerOrderLineOutstanding>> CoOsSelect(int customerId, CancellationToken cancellationToken);
    
    // Co_OsById - Get outstanding CO lines for a specific CO
    Task<IEnumerable<CustomerOrderLineOutstanding>> CoOsById(int coRecId, CancellationToken cancellationToken);
    
    // Co_OsByCoLineId - Get outstanding info for specific CO line
    Task<CustomerOrderLineOutstanding?> CoOsByCoLineId(int coLineId, CancellationToken cancellationToken);
    
    // CoLine_SelectByCo - Get lines for a CO
    Task<IEnumerable<CustomerOrderLine>> CoLineSelectByCo(int coRecId, CancellationToken cancellationToken);
    
    // CoLine_SelectById - Get specific line
    Task<CustomerOrderLine?> CoLineSelectById(int coLineId, CancellationToken cancellationToken);
    
    // CoLine_Insert - Insert CO line
    Task<string> CoLineInsert(CustomerOrderLine customerOrderLine, CancellationToken cancellationToken);
    
    // CoLine_Update - Update CO line
    Task CoLineUpdate(CustomerOrderLine customerOrderLine, CancellationToken cancellationToken);
    
    // CoLine_Delete - Delete CO line
    Task CoLineDelete(int coLineId, string modifiedBy, CancellationToken cancellationToken);
    
    // CoType_Cbo - Get order types for dropdown
    Task<IEnumerable<CustomerOrderType>> CoTypeCbo(CancellationToken cancellationToken);
    
    // Co_RptSummary - Get CO summary report data
    Task<IEnumerable<CoSummaryReportItem>> CoRptSummary(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    
    // Co_RptDetail - Get CO detail report data
    Task<IEnumerable<CoDetailReportItem>> CoRptDetail(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    
    // WfTrans_SelectByRefId - Get workflow history
    Task<IEnumerable<WfTransHistory>> WfTransSelectByRefId(string refId, int wfFormId, CancellationToken cancellationToken);
}
