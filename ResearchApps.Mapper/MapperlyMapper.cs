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
}