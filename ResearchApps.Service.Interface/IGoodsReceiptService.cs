using ResearchApps.Domain;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IGoodsReceiptService
{
    // Gr_Select
    Task<ServiceResponse<PagedListVm<GoodsReceiptHeaderVm>>> GrSelect(PagedListRequestVm request, CancellationToken cancellationToken);
    
    // Gr_SelectById
    Task<ServiceResponse<GoodsReceiptHeaderVm>> GrSelectById(int id, CancellationToken cancellationToken);
    
    // Gr_Insert
    Task<ServiceResponse<int>> GrInsert(GoodsReceiptVm goodsReceipt, CancellationToken cancellationToken);
    
    // Gr_Update
    Task<ServiceResponse> GrUpdate(GoodsReceiptHeaderVm goodsReceiptHeader, CancellationToken cancellationToken);
    
    // Gr_Delete
    Task<ServiceResponse> GrDelete(int recId, CancellationToken cancellationToken);
    
    // Lines
    Task<ServiceResponse<IEnumerable<GoodsReceiptLineVm>>> GrLineSelectByGr(int grRecId, CancellationToken cancellationToken);
    Task<ServiceResponse<GoodsReceiptLineVm>> GrLineSelectById(int grLineId, CancellationToken cancellationToken);
    Task<ServiceResponse> GrLineInsert(GoodsReceiptLineVm goodsReceiptLine, CancellationToken cancellationToken);
    Task<ServiceResponse> GrLineUpdate(GoodsReceiptLineVm goodsReceiptLine, CancellationToken cancellationToken);
    Task<ServiceResponse> GrLineDelete(int grLineId, CancellationToken cancellationToken);
    
    // Outstanding PO Lines
    Task<ServiceResponse<IEnumerable<PoLineOutstandingVm>>> PoOsSelectBySupplier(int supplierId, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<PoLineOutstandingVm>>> PoOsSelectById(int poLineId, CancellationToken cancellationToken);

    // Composite ViewModel
    Task<ServiceResponse<GoodsReceiptVm>> GetGoodsReceipt(int recId, CancellationToken cancellationToken);
    
    // Reports
    Task<IEnumerable<GrReportItem>> GetGrReportData(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    
    // Export
    Task<IEnumerable<GoodsReceiptHeader>> GetGrExportData(PagedListRequestVm request, CancellationToken cancellationToken);
}
