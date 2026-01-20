using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IDeliveryOrderRepo
{
    // Do_Select - Get paginated list of delivery orders
    Task<PagedList<DeliveryOrderHeader>> DoSelect(PagedListRequest request, CancellationToken cancellationToken);
    
    // Do_SelectForExport - Get all delivery orders for export (no pagination)
    Task<IEnumerable<DeliveryOrderHeader>> DoSelectForExport(PagedListRequest request, CancellationToken cancellationToken);
    
    // Do_SelectById - Get DO by RecId
    Task<DeliveryOrderHeader> DoSelectById(int id, CancellationToken cancellationToken);
    
    // Do_SelectCompositeById - Get DO header, lines, and outstanding in single call
    Task<DeliveryOrder> DoSelectCompositeById(int recId, CancellationToken cancellationToken);
    
    // Do_Insert - Create new delivery order (returns RecId and DoId)
    Task<(int RecId, string DoId)> DoInsert(DeliveryOrderHeader deliveryOrder, CancellationToken cancellationToken);
    
    // Do_Update - Update existing delivery order
    Task DoUpdate(DeliveryOrderHeader deliveryOrder, CancellationToken cancellationToken);
    
    // Do_Delete - Delete delivery order (and its lines)
    Task DoDelete(int recId, CancellationToken cancellationToken);
    
    // Do_HdOsSelect - Get outstanding DO headers for a customer
    Task<IEnumerable<DeliveryOrderHeaderOutstanding>> DoHdOsSelect(int customerId, CancellationToken cancellationToken);
    
    // Do_OsSelect - Get outstanding DO lines for a customer
    Task<IEnumerable<DeliveryOrderLineOutstanding>> DoOsSelect(int customerId, CancellationToken cancellationToken);
    
    // Do_OsByDoLineId - Get outstanding info for specific DO line
    Task<DeliveryOrderLineOutstanding> DoOsByDoLineId(int doLineId, CancellationToken cancellationToken);
    
    // DoLine_SelectByDo - Get lines for a DO
    Task<IEnumerable<DeliveryOrderLine>> DoLineSelectByDo(int doRecId, CancellationToken cancellationToken);
    
    // DoLine_SelectById - Get specific line
    Task<DeliveryOrderLine> DoLineSelectById(int doLineId, CancellationToken cancellationToken);
    
    // DoLine_Insert - Insert DO line
    Task<string> DoLineInsert(DeliveryOrderLine deliveryOrderLine, CancellationToken cancellationToken);
    
    // DoLine_Update - Update DO line
    Task DoLineUpdate(DeliveryOrderLine deliveryOrderLine, CancellationToken cancellationToken);
    
    // DoLine_Delete - Delete DO line
    Task DoLineDelete(int doLineId, string modifiedBy, CancellationToken cancellationToken);
    
    // WfTrans_SelectByRefId - Get workflow history
    Task<IEnumerable<WfTransHistory>> WfTransSelectByRefId(string refId, int wfFormId, CancellationToken cancellationToken);
}
