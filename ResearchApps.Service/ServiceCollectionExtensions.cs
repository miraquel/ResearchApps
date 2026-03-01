using Microsoft.Extensions.DependencyInjection;
using ResearchApps.Service.Interface;

namespace ResearchApps.Service;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        // Register service interfaces and their implementations
        services.AddScoped<IItemTypeService, ItemTypeService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IItemDeptService, ItemDeptService>();
        services.AddScoped<IItemGroup01Service, ItemGroup01Service>();
        services.AddScoped<IItemGroup02Service, ItemGroup02Service>();
        services.AddScoped<IStatusService, StatusService>();
        services.AddScoped<IUnitService, UnitService>();
        services.AddScoped<IPrService, PrService>();
        services.AddScoped<IPrLineService, PrLineService>();
        services.AddScoped<IBudgetService, BudgetService>();
        services.AddScoped<IPrStatusService, PrStatusService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IDashboardService, DashboardService>();
        
        // Customer Order Management
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICustomerOrderService, CustomerOrderService>();
        services.AddScoped<IDeliveryOrderService, DeliveryOrderService>();
        services.AddScoped<ISalesInvoiceService, SalesInvoiceService>();
        
        // Purchase Order Management
        services.AddScoped<IPoService, PoService>();
        services.AddScoped<IPoLineService, PoLineService>();
        
        // Supplier Management
        services.AddScoped<ISupplierService, SupplierService>();
        
        // Goods Receipt Management
        services.AddScoped<IGoodsReceiptService, GoodsReceiptService>();
        
        // Production Management
        services.AddScoped<IProdService, ProdService>();
        
        // BPB (Bon Pengambilan Barang) Management
        services.AddScoped<IBpbService, BpbService>();
        
        // Material Customer Management
        services.AddScoped<IMaterialCustomerService, MaterialCustomerService>();
        
        // Penerimaan Hasil Produksi Management
        services.AddScoped<IPhpService, PhpService>();
        
        // Penyesuaian Stock Management
        services.AddScoped<IPsService, PsService>();
        
        // Inventory Lock Management
        services.AddScoped<IInventLockService, InventLockService>();
        
        // Workflow Management
        services.AddScoped<IWfFormService, WfFormService>();
        services.AddScoped<IWfService, WfService>();
        services.AddScoped<IWfTransService, WfTransService>();
        
        // Sales Price & TOP Management
        services.AddScoped<ISalesPriceService, SalesPriceService>();
        services.AddScoped<ITopService, TopService>();
        
        // Report Services
        services.AddScoped<IRepInventTransService, RepInventTransService>();
        services.AddScoped<IRepStockService, RepStockService>();
        services.AddScoped<IRepCustomService, RepCustomService>();
    }
}