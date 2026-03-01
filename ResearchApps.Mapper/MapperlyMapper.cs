using ResearchApps.Domain;
using ResearchApps.Domain.Common;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;
using Riok.Mapperly.Abstractions;

namespace ResearchApps.Mapper;

[Mapper]
public partial class MapperlyMapper
{
    // PagedListRequest
    public partial PagedListRequest MapToEntity(PagedListRequestVm pagedListRequestDto);
    public partial PagedListRequestVm MapToVm(PagedListRequest pagedListRequest);
    
    // ListRequest
    public partial ListRequest MapToEntity(ListRequestVm listRequestDto);
    public partial ListRequestVm MapToVm(ListRequest listRequest);
    
    // CboRequest
    public partial CboRequest MapToEntity(CboRequestVm cboRequestDto);
    public partial CboRequestVm MapToVm(CboRequest cboRequest);
    
    // PagedCboRequest
    public partial PagedCboRequest MapToEntity(PagedCboRequestVm pagedCboRequestDto);
    public partial PagedCboRequestVm MapToVm(PagedCboRequest pagedCboRequest);
    
    // ItemType
    public partial ItemType MapToEntity(ItemTypeVm itemTypeDto);
    public partial IEnumerable<ItemType> MapToEntity(IEnumerable<ItemTypeVm> itemTypeDto);
    public partial PagedList<ItemType> MapToEntity(PagedListVm<ItemTypeVm> pagedListDto);
    public partial ItemTypeVm MapToVm(ItemType itemType);
    public partial IEnumerable<ItemTypeVm> MapToVm(IEnumerable<ItemType> itemTypeDto);
    public partial PagedListVm<ItemTypeVm> MapToVm(PagedList<ItemType> pagedListDto);
    
    // Status
    public partial Status MapToEntity(StatusVm statusDto);
    public partial IEnumerable<StatusVm> MapToEntity(IEnumerable<StatusVm> statusDto);
    public partial PagedList<Status> MapToEntity(PagedListVm<StatusVm> pagedListDto);
    public partial StatusVm MapToVm(Status status);
    public partial IEnumerable<StatusVm> MapToVm(IEnumerable<Status> status);
    public partial PagedListVm<StatusVm> MapToVm(PagedList<Status> pagedListDto);
    
    // Item
    public partial Item MapToEntity(ItemVm itemDto);
    public partial IEnumerable<Item> MapToEntity(IEnumerable<ItemVm> itemDto);
    public partial PagedList<Item> MapToEntity(PagedListVm<ItemVm> pagedListDto);
    public partial ItemVm MapToVm(Item item);
    public partial IEnumerable<ItemVm> MapToVm(IEnumerable<Item> item);
    public partial PagedListVm<ItemVm> MapToVm(PagedList<Item> pagedListDto);
    
    // Warehouse
    public partial Warehouse MapToEntity(WarehouseVm warehouseDto);
    public partial IEnumerable<Warehouse> MapToEntity(IEnumerable<WarehouseVm> warehouseDto);
    public partial PagedList<Warehouse> MapToEntity(PagedListVm<WarehouseVm> pagedListDto);
    public partial WarehouseVm MapToVm(Warehouse warehouse);
    public partial IEnumerable<WarehouseVm> MapToVm(IEnumerable<Warehouse> warehouse);
    public partial PagedListVm<WarehouseVm> MapToVm(PagedList<Warehouse> pagedListDto);
    
    // ItemDept
    public partial ItemDept MapToEntity(ItemDeptVm itemDeptDto);
    public partial IEnumerable<ItemDept> MapToEntity(IEnumerable<ItemDeptVm> itemDeptDto);
    public partial PagedList<ItemDept> MapToEntity(PagedListVm<ItemDeptVm> pagedListDto);
    public partial ItemDeptVm MapToVm(ItemDept itemDept);
    public partial IEnumerable<ItemDeptVm> MapToVm(IEnumerable<ItemDept> itemDeptDto);
    public partial PagedListVm<ItemDeptVm> MapToVm(PagedList<ItemDept> pagedListDto);
    
    // ItemGroup01
    public partial ItemGroup01 MapToEntity(ItemGroup01Vm itemGroup01Dto);
    public partial IEnumerable<ItemGroup01> MapToEntity(IEnumerable<ItemGroup01Vm> itemGroup01Dto);
    public partial PagedList<ItemGroup01> MapToEntity(PagedListVm<ItemGroup01Vm> pagedListDto);
    public partial ItemGroup01Vm MapToVm(ItemGroup01 itemGroup01);
    public partial IEnumerable<ItemGroup01Vm> MapToVm(IEnumerable<ItemGroup01> itemGroup01Dto);
    public partial PagedListVm<ItemGroup01Vm> MapToVm(PagedList<ItemGroup01> pagedListDto);
    
    // ItemGroup02
    public partial ItemGroup02 MapToEntity(ItemGroup02Vm itemGroup02Dto);
    public partial IEnumerable<ItemGroup02> MapToEntity(IEnumerable<ItemGroup02Vm> itemGroup02Dto);
    public partial PagedList<ItemGroup02> MapToEntity(PagedListVm<ItemGroup02Vm> pagedListDto);
    public partial ItemGroup02Vm MapToVm(ItemGroup02 itemGroup02);
    public partial IEnumerable<ItemGroup02Vm> MapToVm(IEnumerable<ItemGroup02> itemGroup02Dto);
    public partial PagedListVm<ItemGroup02Vm> MapToVm(PagedList<ItemGroup02> pagedListDto);
    
    // Unit
    public partial Unit MapToEntity(UnitVm unitDto);
    public partial IEnumerable<Unit> MapToEntity(IEnumerable<UnitVm> unitDto);
    public partial PagedList<Unit> MapToEntity(PagedListVm<UnitVm> pagedListDto);
    public partial UnitVm MapToVm(Unit unit);
    public partial IEnumerable<UnitVm> MapToVm(IEnumerable<Unit> unitDto);
    public partial PagedListVm<UnitVm> MapToVm(PagedList<Unit> pagedListDto);
    
    // Pr
    public partial Pr MapToEntity(PrVm prDto);
    public partial IEnumerable<Pr> MapToEntity(IEnumerable<PrVm> prDto);
    public partial PagedList<Pr> MapToEntity(PagedListVm<PrVm> pagedListDto);
    public partial PrVm MapToVm(Pr pr);
    public partial IEnumerable<PrVm> MapToVm(IEnumerable<Pr> prDto);
    public partial PagedListVm<PrVm> MapToVm(PagedList<Pr> pagedListDto);
    
    // PrLine
    public partial PrLine MapToEntity(PrLineVm prLineDto);
    public partial IEnumerable<PrLine> MapToEntity(IEnumerable<PrLineVm> prLineDto);
    public partial PagedList<PrLine> MapToEntity(PagedListVm<PrLineVm> pagedListDto);
    public partial PrLineVm MapToVm(PrLine prLine);
    public partial IEnumerable<PrLineVm> MapToVm(IEnumerable<PrLine> prLineDto);
    public partial PagedListVm<PrLineVm> MapToVm(PagedList<PrLine> pagedListDto);
    
    // Budget
    public partial Budget MapToEntity(BudgetVm budgetDto);
    public partial IEnumerable<Budget> MapToEntity(IEnumerable<BudgetVm> budgetDto);
    public partial PagedList<Budget> MapToEntity(PagedListVm<BudgetVm> pagedListDto);
    public partial BudgetVm MapToVm(Budget budget);
    public partial IEnumerable<BudgetVm> MapToVm(IEnumerable<Budget> budgetDto);
    public partial PagedListVm<BudgetVm> MapToVm(PagedList<Budget> pagedListDto);
    
    // PrStatus
    public partial PrStatus MapToEntity(PrStatusVm prStatusDto);
    public partial IEnumerable<PrStatus> MapToEntity(IEnumerable<PrStatusVm> prStatusDto);
    public partial PagedList<PrStatus> MapToEntity(PagedListVm<PrStatusVm> pagedListDto);
    public partial PrStatusVm MapToVm(PrStatus prStatus);
    public partial IEnumerable<PrStatusVm> MapToVm(IEnumerable<PrStatus> prStatusDto);
    public partial PagedListVm<PrStatusVm> MapToVm(PagedList<PrStatus> pagedListDto);
    
    // Report
    public partial Report MapToEntity(ReportVm reportDto);
    public partial IEnumerable<Report> MapToEntity(IEnumerable<ReportVm> reportDto);
    public partial PagedList<Report> MapToEntity(PagedListVm<ReportVm> pagedListDto);
    public partial ReportVm MapToVm(Report report);
    public partial IEnumerable<ReportVm> MapToVm(IEnumerable<Report> reportDto);
    public partial PagedListVm<ReportVm> MapToVm(PagedList<Report> pagedListDto);
    
    // ReportParameter
    public partial ReportParameter MapToEntity(ReportParameterVm reportParameterDto);
    public partial IEnumerable<ReportParameter> MapToEntity(IEnumerable<ReportParameterVm> reportParameterDto);
    public partial ReportParameterVm MapToVm(ReportParameter reportParameter);
    public partial IEnumerable<ReportParameterVm> MapToVm(IEnumerable<ReportParameter> reportParameterDto);
    
    // ReportFieldCoordinate
    public partial ReportFieldCoordinate MapToEntity(ReportFieldCoordinateVm reportFieldCoordinateDto);
    public partial IEnumerable<ReportFieldCoordinate> MapToEntity(IEnumerable<ReportFieldCoordinateVm> reportFieldCoordinateDto);
    public partial ReportFieldCoordinateVm MapToVm(ReportFieldCoordinate reportFieldCoordinate);
    public partial IEnumerable<ReportFieldCoordinateVm> MapToVm(IEnumerable<ReportFieldCoordinate> reportFieldCoordinateDto);
    
    // Dashboard
    public partial DashboardStatisticsVm Map(DashboardStatistics statistics);
    public partial RecentPrVm Map(RecentPr recentPr);
    public partial PendingApprovalVm Map(PendingApproval pendingApproval);
    public partial TopItemVm Map(TopItem topItem);
    public partial PrTrendVm Map(PrTrend prTrend);
    public partial BudgetByDepartmentVm Map(BudgetByDepartment budgetByDepartment);
    
    // Customer
    public partial Customer MapToEntity(CustomerVm customerDto);
    public partial IEnumerable<Customer> MapToEntity(IEnumerable<CustomerVm> customerDto);
    public partial PagedList<Customer> MapToEntity(PagedListVm<CustomerVm> pagedListDto);
    public partial CustomerVm MapToVm(Customer customer);
    public partial IEnumerable<CustomerVm> MapToVm(IEnumerable<Customer> customerDto);
    public partial PagedListVm<CustomerVm> MapToVm(PagedList<Customer> pagedListDto);
    
    // Co (Customer Order)
    public partial CustomerOrder MapToEntity(CustomerOrderVm customerOrderHeaderDto);
    public partial IEnumerable<CustomerOrder> MapToEntity(IEnumerable<CustomerOrderVm> coDto);
    public partial PagedList<CustomerOrder> MapToEntity(PagedListVm<CustomerOrderVm> pagedListDto);
    public partial CustomerOrderVm MapToVm(CustomerOrder customerOrder);
    public partial IEnumerable<CustomerOrderVm> MapToVm(IEnumerable<CustomerOrder> coDto);
    public partial PagedListVm<CustomerOrderVm> MapToVm(PagedList<CustomerOrder> pagedListDto);
    
    // CoHeader
    public partial CustomerOrderHeader MapToEntity(CustomerOrderHeaderVm customerOrderHeaderDto);
    public partial IEnumerable<CustomerOrderHeader> MapToEntity(IEnumerable<CustomerOrderHeaderVm> coHeader);
    public partial CustomerOrderHeaderVm MapToVm(CustomerOrderHeader customerOrderHeader);
    public partial IEnumerable<CustomerOrderHeaderVm> MapToVm(IEnumerable<CustomerOrderHeader> coHeader);
    public partial PagedListVm<CustomerOrderHeaderVm> MapToVm(PagedList<CustomerOrderHeader> pagedListDto);
    public partial PagedList<CustomerOrderHeader> MapToEntity(PagedListVm<CustomerOrderHeaderVm> pagedListDto);
    
    // CoLine
    public partial CustomerOrderLine MapToEntity(CustomerOrderLineVm customerOrderLineDto);
    public partial IEnumerable<CustomerOrderLine> MapToEntity(IEnumerable<CustomerOrderLineVm> coLineDto);
    public partial CustomerOrderLineVm MapToVm(CustomerOrderLine customerOrderLine);
    public partial IEnumerable<CustomerOrderLineVm> MapToVm(IEnumerable<CustomerOrderLine> coLineDto);
    
    // CoHeaderOutstanding
    public partial CustomerOrderHeaderOutstanding MapToEntity(CustomerOrderHeaderOutstandingVm customerOrderHeaderOutstandingDto);
    public partial IEnumerable<CustomerOrderHeaderOutstanding> MapToEntity(IEnumerable<CustomerOrderHeaderOutstandingVm> coHeaderOutstandingDto);
    public partial CustomerOrderHeaderOutstandingVm MapToVm(CustomerOrderHeaderOutstanding customerOrderHeaderOutstanding);
    public partial IEnumerable<CustomerOrderHeaderOutstandingVm> MapToVm(IEnumerable<CustomerOrderHeaderOutstanding> coHeaderOutstandingDto);
    public partial PagedListVm<CustomerOrderHeaderOutstandingVm> MapToVm(PagedList<CustomerOrderHeaderOutstanding> pagedListDto);
    public partial PagedList<CustomerOrderHeaderOutstanding> MapToEntity(PagedListVm<CustomerOrderHeaderOutstandingVm> pagedListDto);
    
    // CoLineOutstanding
    public partial CustomerOrderLineOutstanding MapToEntity(CustomerOrderOutstandingVm customerOrderOutstandingDto);
    public partial IEnumerable<CustomerOrderLineOutstanding> MapToEntity(IEnumerable<CustomerOrderOutstandingVm> coOutstandingDto);
    public partial CustomerOrderOutstandingVm MapToVm(CustomerOrderLineOutstanding customerOrderLineOutstanding);
    public partial IEnumerable<CustomerOrderOutstandingVm> MapToVm(IEnumerable<CustomerOrderLineOutstanding> coOutstanding);
    public partial PagedListVm<CustomerOrderOutstandingVm> MapToVm(PagedList<CustomerOrderLineOutstanding> pagedListDto);
    public partial PagedList<CustomerOrderLineOutstanding> MapToEntity(PagedListVm<CustomerOrderOutstandingVm> pagedListDto);
    
    // CoType
    public partial CustomerOrderType MapToEntity(CustomerOrderTypeVm customerOrderTypeDto);
    public partial CustomerOrderTypeVm MapToVm(CustomerOrderType customerOrderType);
    public partial IEnumerable<CustomerOrderTypeVm> MapToVm(IEnumerable<CustomerOrderType> coTypeDto);
    
    // CoStatus
    public partial CustomerOrderStatus MapToEntity(CustomerOrderStatusVm customerOrderStatusDto);
    public partial CustomerOrderStatusVm MapToVm(CustomerOrderStatus customerOrderStatus);
    public partial IEnumerable<CustomerOrderStatusVm> MapToVm(IEnumerable<CustomerOrderStatus> coStatusDto);
    
    // Do (Delivery Order)
    public partial DeliveryOrder MapToEntity(DeliveryOrderVm deliveryOrderHeaderDto);
    public partial IEnumerable<DeliveryOrder> MapToEntity(IEnumerable<DeliveryOrderVm> doDto);
    public partial PagedList<DeliveryOrder> MapToEntity(PagedListVm<DeliveryOrderVm> pagedListDto);
    public partial DeliveryOrderVm MapToVm(DeliveryOrder deliveryOrder);
    public partial IEnumerable<DeliveryOrderVm> MapToVm(IEnumerable<DeliveryOrder> doDto);
    public partial PagedListVm<DeliveryOrderVm> MapToVm(PagedList<DeliveryOrder> pagedListDto);
    
    // DoHeader
    public partial DeliveryOrderHeader MapToEntity(DeliveryOrderHeaderVm deliveryOrderHeaderDto);
    public partial IEnumerable<DeliveryOrderHeader> MapToEntity(IEnumerable<DeliveryOrderHeaderVm> doHeader);
    public partial DeliveryOrderHeaderVm MapToVm(DeliveryOrderHeader deliveryOrderHeader);
    public partial IEnumerable<DeliveryOrderHeaderVm> MapToVm(IEnumerable<DeliveryOrderHeader> doHeader);
    public partial PagedListVm<DeliveryOrderHeaderVm> MapToVm(PagedList<DeliveryOrderHeader> pagedListDto);
    public partial PagedList<DeliveryOrderHeader> MapToEntity(PagedListVm<DeliveryOrderHeaderVm> pagedListDto);
    
    // DoLine
    public partial DeliveryOrderLine MapToEntity(DeliveryOrderLineVm deliveryOrderLineDto);
    public partial IEnumerable<DeliveryOrderLine> MapToEntity(IEnumerable<DeliveryOrderLineVm> doLineDto);
    public partial DeliveryOrderLineVm MapToVm(DeliveryOrderLine deliveryOrderLine);
    public partial IEnumerable<DeliveryOrderLineVm> MapToVm(IEnumerable<DeliveryOrderLine> doLineDto);
    
    // DoHeaderOutstanding
    public partial DeliveryOrderHeaderOutstanding MapToEntity(DeliveryOrderHeaderOutstandingVm deliveryOrderHeaderOutstandingDto);
    public partial IEnumerable<DeliveryOrderHeaderOutstanding> MapToEntity(IEnumerable<DeliveryOrderHeaderOutstandingVm> doHeaderOutstandingDto);
    public partial DeliveryOrderHeaderOutstandingVm MapToVm(DeliveryOrderHeaderOutstanding deliveryOrderHeaderOutstanding);
    public partial IEnumerable<DeliveryOrderHeaderOutstandingVm> MapToVm(IEnumerable<DeliveryOrderHeaderOutstanding> doHeaderOutstandingDto);
    public partial PagedListVm<DeliveryOrderHeaderOutstandingVm> MapToVm(PagedList<DeliveryOrderHeaderOutstanding> pagedListDto);
    public partial PagedList<DeliveryOrderHeaderOutstanding> MapToEntity(PagedListVm<DeliveryOrderHeaderOutstandingVm> pagedListDto);
    
    // DoLineOutstanding
    public partial DeliveryOrderLineOutstanding MapToEntity(DeliveryOrderOutstandingVm deliveryOrderOutstandingDto);
    public partial IEnumerable<DeliveryOrderLineOutstanding> MapToEntity(IEnumerable<DeliveryOrderOutstandingVm> doOutstandingDto);
    public partial DeliveryOrderOutstandingVm MapToVm(DeliveryOrderLineOutstanding deliveryOrderLineOutstanding);
    public partial IEnumerable<DeliveryOrderOutstandingVm> MapToVm(IEnumerable<DeliveryOrderLineOutstanding> doOutstanding);
    public partial PagedListVm<DeliveryOrderOutstandingVm> MapToVm(PagedList<DeliveryOrderLineOutstanding> pagedListDto);
    public partial PagedList<DeliveryOrderLineOutstanding> MapToEntity(PagedListVm<DeliveryOrderOutstandingVm> pagedListDto);
    
    // DoStatus
    public partial DeliveryOrderStatus MapToEntity(DeliveryOrderStatusVm deliveryOrderStatusDto);
    public partial DeliveryOrderStatusVm MapToVm(DeliveryOrderStatus deliveryOrderStatus);
    public partial IEnumerable<DeliveryOrderStatusVm> MapToVm(IEnumerable<DeliveryOrderStatus> doStatusDto);
    
    // Si (Sales Invoice)
    public partial SalesInvoice MapToEntity(SalesInvoiceVm salesInvoiceDto);
    public partial IEnumerable<SalesInvoice> MapToEntity(IEnumerable<SalesInvoiceVm> siDto);
    public partial PagedList<SalesInvoice> MapToEntity(PagedListVm<SalesInvoiceVm> pagedListDto);
    public partial SalesInvoiceVm MapToVm(SalesInvoice salesInvoice);
    public partial IEnumerable<SalesInvoiceVm> MapToVm(IEnumerable<SalesInvoice> siDto);
    public partial PagedListVm<SalesInvoiceVm> MapToVm(PagedList<SalesInvoice> pagedListDto);
    
    // SiHeader
    public partial SalesInvoiceHeader MapToEntity(SalesInvoiceHeaderVm salesInvoiceHeaderDto);
    public partial IEnumerable<SalesInvoiceHeader> MapToEntity(IEnumerable<SalesInvoiceHeaderVm> siHeader);
    public partial SalesInvoiceHeaderVm MapToVm(SalesInvoiceHeader salesInvoiceHeader);
    public partial IEnumerable<SalesInvoiceHeaderVm> MapToVm(IEnumerable<SalesInvoiceHeader> siHeader);
    public partial PagedListVm<SalesInvoiceHeaderVm> MapToVm(PagedList<SalesInvoiceHeader> pagedListDto);
    public partial PagedList<SalesInvoiceHeader> MapToEntity(PagedListVm<SalesInvoiceHeaderVm> pagedListDto);
    
    // SiLine
    public partial SalesInvoiceLine MapToEntity(SalesInvoiceLineVm salesInvoiceLineDto);
    public partial IEnumerable<SalesInvoiceLine> MapToEntity(IEnumerable<SalesInvoiceLineVm> siLineDto);
    public partial SalesInvoiceLineVm MapToVm(SalesInvoiceLine salesInvoiceLine);
    public partial IEnumerable<SalesInvoiceLineVm> MapToVm(IEnumerable<SalesInvoiceLine> siLineDto);
    
    // SiStatus
    public partial SalesInvoiceStatus MapToEntity(SalesInvoiceStatusVm salesInvoiceStatusDto);
    public partial SalesInvoiceStatusVm MapToVm(SalesInvoiceStatus salesInvoiceStatus);
    public partial IEnumerable<SalesInvoiceStatusVm> MapToVm(IEnumerable<SalesInvoiceStatus> siStatusDto);
    
    // Po (Purchase Order) - Header mappings
    public partial Po MapToEntity(PoHeaderVm poHeaderDto);
    public partial PoHeaderVm MapToVm(Po po);
    
    // Po - Composite ViewModel (Header + Lines)
    public PoVm MapToCompositeVm(Po po, IEnumerable<PoLine> lines)
    {
        return new PoVm
        {
            Header = MapToVm(po),
            Lines = MapToVm(lines).ToList()
        };
    }
    
    public Po MapFromCompositeVm(PoVm poVm)
    {
        return MapToEntity(poVm.Header);
    }
    
    // Po - List mappings (for Index page - uses Header only)
    public partial IEnumerable<PoHeaderVm> MapToVm(IEnumerable<Po> poDto);
    public partial PagedListVm<PoHeaderVm> MapToVm(PagedList<Po> pagedListDto);
    
    // PoLine
    public partial PoLine MapToEntity(PoLineVm poLineDto);
    public partial IEnumerable<PoLine> MapToEntity(IEnumerable<PoLineVm> poLineDto);
    public partial PoLineVm MapToVm(PoLine poLine);
    public partial IEnumerable<PoLineVm> MapToVm(IEnumerable<PoLine> poLineDto);
    
    // PoHeaderOutstanding
    public partial PoHeaderOutstanding MapToEntity(PoHeaderOutstandingVm poHeaderOutstandingDto);
    public partial IEnumerable<PoHeaderOutstanding> MapToEntity(IEnumerable<PoHeaderOutstandingVm> poHeaderOutstandingDto);
    public partial PoHeaderOutstandingVm MapToVm(PoHeaderOutstanding poHeaderOutstanding);
    public partial IEnumerable<PoHeaderOutstandingVm> MapToVm(IEnumerable<PoHeaderOutstanding> poHeaderOutstandingDto);
    
    // PoLineOutstanding
    public partial PoLineOutstanding MapToEntity(PoLineOutstandingVm poLineOutstandingDto);
    public partial IEnumerable<PoLineOutstanding> MapToEntity(IEnumerable<PoLineOutstandingVm> poLineOutstandingDto);
    public partial PoLineOutstandingVm MapToVm(PoLineOutstanding poLineOutstanding);
    public partial IEnumerable<PoLineOutstandingVm> MapToVm(IEnumerable<PoLineOutstanding> poLineOutstandingDto);
    
    // Supplier
    public partial Supplier MapToEntity(SupplierVm supplierVm);
    public partial IEnumerable<Supplier> MapToEntity(IEnumerable<SupplierVm> supplierVms);
    public partial SupplierVm MapToVm(Supplier supplier);
    public partial IEnumerable<SupplierVm> MapToVm(IEnumerable<Supplier> suppliers);
    public partial PagedListVm<SupplierVm> MapToVm(PagedList<Supplier> pagedListDto);
    
    // Gr (Goods Receipt) - Composite
    public partial GoodsReceipt MapToEntity(GoodsReceiptVm goodsReceiptDto);
    public partial IEnumerable<GoodsReceipt> MapToEntity(IEnumerable<GoodsReceiptVm> grDto);
    public partial GoodsReceiptVm MapToVm(GoodsReceipt goodsReceipt);
    public partial IEnumerable<GoodsReceiptVm> MapToVm(IEnumerable<GoodsReceipt> grDto);
    
    // GrHeader
    public partial GoodsReceiptHeader MapToEntity(GoodsReceiptHeaderVm goodsReceiptHeaderDto);
    public partial IEnumerable<GoodsReceiptHeader> MapToEntity(IEnumerable<GoodsReceiptHeaderVm> grHeader);
    public partial GoodsReceiptHeaderVm MapToVm(GoodsReceiptHeader goodsReceiptHeader);
    public partial IEnumerable<GoodsReceiptHeaderVm> MapToVm(IEnumerable<GoodsReceiptHeader> grHeader);
    public partial PagedListVm<GoodsReceiptHeaderVm> MapToVm(PagedList<GoodsReceiptHeader> pagedListDto);
    public partial PagedList<GoodsReceiptHeader> MapToEntity(PagedListVm<GoodsReceiptHeaderVm> pagedListDto);
    
    // GrLine
    public partial GoodsReceiptLine MapToEntity(GoodsReceiptLineVm goodsReceiptLineDto);
    public partial IEnumerable<GoodsReceiptLine> MapToEntity(IEnumerable<GoodsReceiptLineVm> grLineDto);
    public partial GoodsReceiptLineVm MapToVm(GoodsReceiptLine goodsReceiptLine);
    public partial IEnumerable<GoodsReceiptLineVm> MapToVm(IEnumerable<GoodsReceiptLine> grLineDto);
    
    // GrStatus
    public partial GoodsReceiptStatus MapToEntity(GoodsReceiptStatusVm goodsReceiptStatusDto);
    public partial GoodsReceiptStatusVm MapToVm(GoodsReceiptStatus goodsReceiptStatus);
    public partial IEnumerable<GoodsReceiptStatusVm> MapToVm(IEnumerable<GoodsReceiptStatus> grStatusDto);
    
    // GrReportItem
    public partial GrReportItem MapToEntity(GrReportItemVm grReportItemDto);
    public partial IEnumerable<GrReportItem> MapToEntity(IEnumerable<GrReportItemVm> grReportItemDto);
    public partial GrReportItemVm MapToVm(GrReportItem grReportItem);
    public partial IEnumerable<GrReportItemVm> MapToVm(IEnumerable<GrReportItem> grReportItemDto);
    
    // Prod (Production)
    public partial Prod MapToEntity(ProdVm prodVm);
    public partial IEnumerable<Prod> MapToEntity(IEnumerable<ProdVm> prodVms);
    public partial PagedList<Prod> MapToEntity(PagedListVm<ProdVm> pagedListVm);
    public partial ProdVm MapToVm(Prod prod);
    public partial IEnumerable<ProdVm> MapToVm(IEnumerable<Prod> prods);
    public partial PagedListVm<ProdVm> MapToVm(PagedList<Prod> pagedList);
    
    // ProdStatus
    public partial ProdStatus MapToEntity(ProdStatusVm prodStatusVm);
    public partial IEnumerable<ProdStatus> MapToEntity(IEnumerable<ProdStatusVm> prodStatusVms);
    public partial ProdStatusVm MapToVm(ProdStatus prodStatus);
    public partial IEnumerable<ProdStatusVm> MapToVm(IEnumerable<ProdStatus> prodStatuses);
    
    // Bpb (Bon Pengambilan Barang) - Composite
    public partial Bpb MapToEntity(BpbVm bpbDto);
    public partial IEnumerable<Bpb> MapToEntity(IEnumerable<BpbVm> bpbDto);
    public partial BpbVm MapToVm(Bpb bpb);
    public partial IEnumerable<BpbVm> MapToVm(IEnumerable<Bpb> bpbDto);
    
    // BpbHeader
    public partial BpbHeader MapToEntity(BpbHeaderVm bpbHeaderDto);
    public partial IEnumerable<BpbHeader> MapToEntity(IEnumerable<BpbHeaderVm> bpbHeader);
    public partial BpbHeaderVm MapToVm(BpbHeader bpbHeader);
    public partial IEnumerable<BpbHeaderVm> MapToVm(IEnumerable<BpbHeader> bpbHeader);
    public partial PagedListVm<BpbHeaderVm> MapToVm(PagedList<BpbHeader> pagedListDto);
    public partial PagedList<BpbHeader> MapToEntity(PagedListVm<BpbHeaderVm> pagedListDto);
    
    // BpbLine
    public partial BpbLine MapToEntity(BpbLineVm bpbLineDto);
    public partial IEnumerable<BpbLine> MapToEntity(IEnumerable<BpbLineVm> bpbLineDto);
    public partial BpbLineVm MapToVm(BpbLine bpbLine);
    public partial IEnumerable<BpbLineVm> MapToVm(IEnumerable<BpbLine> bpbLineDto);
    
    // BpbStatus
    public partial BpbStatus MapToEntity(BpbStatusVm bpbStatusDto);
    public partial BpbStatusVm MapToVm(BpbStatus bpbStatus);
    public partial IEnumerable<BpbStatusVm> MapToVm(IEnumerable<BpbStatus> bpbStatusDto);
    
    // Mc (Material Customer) - Header mappings
    public partial MaterialCustomerHeader MapToEntity(MaterialCustomerHeaderVm mcHeaderDto);
    public partial MaterialCustomerHeaderVm MapToVm(MaterialCustomerHeader mcHeader);
    
    // Mc - Composite ViewModel (Header + Lines)
    public MaterialCustomerVm MapToCompositeVm(MaterialCustomerHeader header, IEnumerable<MaterialCustomerLine> lines)
    {
        return new MaterialCustomerVm
        {
            Header = MapToVm(header),
            Lines = MapToVm(lines).ToList()
        };
    }
    
    public MaterialCustomerHeader MapFromCompositeVm(MaterialCustomerVm mcVm)
    {
        return MapToEntity(mcVm.Header);
    }
    
    // Mc - List mappings (for Index page - uses Header only)
    public partial IEnumerable<MaterialCustomerHeaderVm> MapToVm(IEnumerable<MaterialCustomerHeader> mcDto);
    public partial PagedListVm<MaterialCustomerHeaderVm> MapToVm(PagedList<MaterialCustomerHeader> pagedListDto);
    
    // McLine
    public partial MaterialCustomerLine MapToEntity(MaterialCustomerLineVm mcLineDto);
    public partial IEnumerable<MaterialCustomerLine> MapToEntity(IEnumerable<MaterialCustomerLineVm> mcLineDto);
    public partial MaterialCustomerLineVm MapToVm(MaterialCustomerLine mcLine);
    public partial IEnumerable<MaterialCustomerLineVm> MapToVm(IEnumerable<MaterialCustomerLine> mcLineDto);
    
    // McStatus
    public partial MaterialCustomerStatus MapToEntity(MaterialCustomerStatusVm mcStatusDto);
    public partial MaterialCustomerStatusVm MapToVm(MaterialCustomerStatus mcStatus);
    public partial IEnumerable<MaterialCustomerStatusVm> MapToVm(IEnumerable<MaterialCustomerStatus> mcStatusDto);
    
    // Php (Penerimaan Hasil Produksi) - Header mappings
    public partial PhpHeader MapToEntity(PhpHeaderVm phpHeaderDto);
    public partial PhpHeaderVm MapToVm(PhpHeader phpHeader);
    
    // Php - Composite ViewModel (Header + Lines)
    public PhpVm MapToCompositeVm(PhpHeader header, IEnumerable<PhpLine> lines)
    {
        return new PhpVm
        {
            Header = MapToVm(header),
            Lines = MapToVm(lines).ToList()
        };
    }
    
    public Php MapToEntity(PhpVm phpVm)
    {
        return new Php
        {
            Header = MapToEntity(phpVm.Header),
            Lines = MapToEntity(phpVm.Lines).ToList()
        };
    }
    
    public PhpHeader MapFromCompositeVm(PhpVm phpVm)
    {
        return MapToEntity(phpVm.Header);
    }
    
    // Php - List mappings (for Index page - uses Header only)
    public partial IEnumerable<PhpHeaderVm> MapToVm(IEnumerable<PhpHeader> phpDto);
    public partial PagedListVm<PhpHeaderVm> MapToVm(PagedList<PhpHeader> pagedListDto);
    
    // PhpLine
    public partial PhpLine MapToEntity(PhpLineVm phpLineDto);
    public partial IEnumerable<PhpLine> MapToEntity(IEnumerable<PhpLineVm> phpLineDto);
    public partial PhpLineVm MapToVm(PhpLine phpLine);
    public partial IEnumerable<PhpLineVm> MapToVm(IEnumerable<PhpLine> phpLineDto);
    
    // PhpStatus
    public partial PhpStatus MapToEntity(PhpStatusVm phpStatusDto);
    public partial PhpStatusVm MapToVm(PhpStatus phpStatus);
    public partial IEnumerable<PhpStatusVm> MapToVm(IEnumerable<PhpStatus> phpStatusDto);
    
    // Ps (Penyesuaian Stock) - Composite
    public partial Ps MapToEntity(PsVm psDto);
    public partial PsVm MapToVm(Ps ps);
    
    // PsHeader
    public partial PsHeader MapToEntity(PsHeaderVm psHeaderDto);
    public partial IEnumerable<PsHeader> MapToEntity(IEnumerable<PsHeaderVm> psHeader);
    public partial PsHeaderVm MapToVm(PsHeader psHeader);
    public partial IEnumerable<PsHeaderVm> MapToVm(IEnumerable<PsHeader> psHeader);
    
    // PsLine
    public partial PsLine MapToEntity(PsLineVm psLineDto);
    public partial IEnumerable<PsLine> MapToEntity(IEnumerable<PsLineVm> psLineDto);
    public partial PsLineVm MapToVm(PsLine psLine);
    public partial IEnumerable<PsLineVm> MapToVm(IEnumerable<PsLine> psLineDto);
    
    // PsStatus
    public partial PsStatus MapToEntity(PsStatusVm psStatusDto);
    public partial PsStatusVm MapToVm(PsStatus psStatus);
    public partial IEnumerable<PsStatusVm> MapToVm(IEnumerable<PsStatus> psStatusDto);
    
    // InventLock
    public partial InventLock MapToEntity(InventLockVm inventLockDto);
    public partial IEnumerable<InventLock> MapToEntity(IEnumerable<InventLockVm> inventLockDto);
    public partial InventLockVm MapToVm(InventLock inventLock);
    public partial IEnumerable<InventLockVm> MapToVm(IEnumerable<InventLock> inventLockDto);
    
    // WfForm (Workflow Form)
    public partial WfForm MapToEntity(WfFormVm wfFormDto);
    public partial IEnumerable<WfForm> MapToEntity(IEnumerable<WfFormVm> wfFormDto);
    public partial PagedList<WfForm> MapToEntity(PagedListVm<WfFormVm> pagedListDto);
    public partial WfFormVm MapToVm(WfForm wfForm);
    public partial IEnumerable<WfFormVm> MapToVm(IEnumerable<WfForm> wfFormDto);
    public partial PagedListVm<WfFormVm> MapToVm(PagedList<WfForm> pagedListDto);
    
    // Wf (Workflow Approval Steps)
    public partial Wf MapToEntity(WfVm wfDto);
    public partial IEnumerable<Wf> MapToEntity(IEnumerable<WfVm> wfDto);
    public partial WfVm MapToVm(Wf wf);
    public partial IEnumerable<WfVm> MapToVm(IEnumerable<Wf> wfDto);
    
    // WfStatusAction
    public partial WfStatusAction MapToEntity(WfStatusActionVm wfStatusActionDto);
    public partial IEnumerable<WfStatusAction> MapToEntity(IEnumerable<WfStatusActionVm> wfStatusActionDto);
    public partial WfStatusActionVm MapToVm(WfStatusAction wfStatusAction);
    public partial IEnumerable<WfStatusActionVm> MapToVm(IEnumerable<WfStatusAction> wfStatusActionDto);
    
    // WfTransHistory
    public partial WfTransHistory MapToEntity(WfTransHistoryVm wfTransHistoryDto);
    public partial IEnumerable<WfTransHistory> MapToEntity(IEnumerable<WfTransHistoryVm> wfTransHistoryDto);
    public partial WfTransHistoryVm MapToVm(WfTransHistory wfTransHistory);
    public partial IEnumerable<WfTransHistoryVm> MapToVm(IEnumerable<WfTransHistory> wfTransHistoryDto);
    
    // SalesPrice
    public partial SalesPrice MapToEntity(SalesPriceVm salesPriceVm);
    public partial IEnumerable<SalesPrice> MapToEntity(IEnumerable<SalesPriceVm> salesPriceVms);
    public partial PagedList<SalesPrice> MapToEntity(PagedListVm<SalesPriceVm> pagedListDto);
    public partial SalesPriceVm MapToVm(SalesPrice salesPrice);
    public partial IEnumerable<SalesPriceVm> MapToVm(IEnumerable<SalesPrice> salesPrices);
    public partial PagedListVm<SalesPriceVm> MapToVm(PagedList<SalesPrice> pagedListDto);
    
    // Top (Terms of Payment)
    public partial Top MapToEntity(TopVm topVm);
    public partial IEnumerable<Top> MapToEntity(IEnumerable<TopVm> topVms);
    public partial PagedList<Top> MapToEntity(PagedListVm<TopVm> pagedListDto);
    public partial TopVm MapToVm(Top top);
    public partial IEnumerable<TopVm> MapToVm(IEnumerable<Top> tops);
    public partial PagedListVm<TopVm> MapToVm(PagedList<Top> pagedListDto);
    
    // RepInventTransByItem (Report: Inventory Transactions)
    public partial RepInventTransByItemVm MapToVm(RepInventTransByItem repInventTransByItem);
    public partial IEnumerable<RepInventTransByItemVm> MapToVm(IEnumerable<RepInventTransByItem> repInventTransByItems);
    
    // RepStockCardMonthly (Report: Stock Card Monthly)
    public partial RepStockCardMonthlyVm MapToVm(RepStockCardMonthly repStockCardMonthly);
    public partial IEnumerable<RepStockCardMonthlyVm> MapToVm(IEnumerable<RepStockCardMonthly> repStockCardMonthlies);
    
    // RepTools (Report: Tools Consumption)
    public partial RepToolsVm MapToVm(RepTools repTools);
    public partial IEnumerable<RepToolsVm> MapToVm(IEnumerable<RepTools> repToolsList);
    
    // RepToolsAnalysis (Report: Tools Analysis)
    public partial RepToolsAnalysisVm MapToVm(RepToolsAnalysis repToolsAnalysis);
    public partial IEnumerable<RepToolsAnalysisVm> MapToVm(IEnumerable<RepToolsAnalysis> repToolsAnalysisList);
}