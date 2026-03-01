using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IGoodsReceiptRepo
{
    // Gr_Select - Get paginated list of goods receipts
    Task<PagedList<GoodsReceiptHeader>> GrSelect(PagedListRequest request, CancellationToken cancellationToken);
    
    // Gr_Select - Get all goods receipts for export (no pagination)
    Task<IEnumerable<GoodsReceiptHeader>> GrSelectForExport(PagedListRequest request, CancellationToken cancellationToken);
    
    // Gr_SelectById - Get GR by RecId
    Task<GoodsReceiptHeader> GrSelectById(int id, CancellationToken cancellationToken);
    
    // Gr_Insert - Create new goods receipt (returns RecId)
    Task<int> GrInsert(GoodsReceiptHeader goodsReceipt, CancellationToken cancellationToken);
    
    // Gr_Update - Update existing goods receipt
    Task GrUpdate(GoodsReceiptHeader goodsReceipt, CancellationToken cancellationToken);
    
    // Gr_Delete - Delete goods receipt (and its lines)
    Task GrDelete(int recId, CancellationToken cancellationToken);
    
    // GrLine_SelectByGr - Get lines for a GR
    Task<IEnumerable<GoodsReceiptLine>> GrLineSelectByGr(int grRecId, CancellationToken cancellationToken);
    
    // GrLine_SelectById - Get specific line
    Task<GoodsReceiptLine?> GrLineSelectById(int grLineId, CancellationToken cancellationToken);
    
    // GrLine_Insert - Insert GR line
    Task<int> GrLineInsert(GoodsReceiptLine goodsReceiptLine, CancellationToken cancellationToken);
    
    // GrLine_Update - Update GR line
    Task GrLineUpdate(GoodsReceiptLine goodsReceiptLine, CancellationToken cancellationToken);
    
    // GrLine_Delete - Delete GR line
    Task GrLineDelete(int grLineId, string modifiedBy, CancellationToken cancellationToken);
    
    // Po_OsSelectBySupplier - Get outstanding PO lines for a supplier
    Task<IEnumerable<PoLineOutstanding>> PoOsSelectBySupplier(int supplierId, CancellationToken cancellationToken);
    
    // Po_OsSelectById - Get outstanding data for a specific PO line
    Task<PoLineOutstanding?> PoOsSelectById(int poLineId, CancellationToken cancellationToken);
    
    // Gr_Rpt - Get GR report data
    Task<IEnumerable<GrReportItem>> GrRpt(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}
