using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResearchApps.Repo.Interface;

namespace ResearchApps.Repo;

public static class ServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        // register IDbConnection
        services.AddScoped<IDbConnection>(sp =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");
            return new SqlConnection(connectionString);
        });
        // register IDbTransaction
        services.AddScoped<IDbTransaction>(sp =>
        {
            var dbConnection = sp.GetRequiredService<IDbConnection>();
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }

            return dbConnection.BeginTransaction();
        });
        
        services.AddScoped<IItemTypeRepo, ItemTypeRepo>();
        services.AddScoped<IItemRepo, ItemRepo>();
        services.AddScoped<IWarehouseRepo, WarehouseRepo>();
        services.AddScoped<IItemDeptRepo, ItemDeptRepo>();
        services.AddScoped<IItemGroup01Repo, ItemGroup01Repo>();
        services.AddScoped<IItemGroup02Repo, ItemGroup02Repo>();
        services.AddScoped<IStatusRepo, StatusRepo>();
        services.AddScoped<IUnitRepo, UnitRepo>();
        services.AddScoped<IPrRepo, PrRepo>();
        services.AddScoped<IPrLineRepo, PrLineRepo>();
        services.AddScoped<IBudgetRepo, BudgetRepo>();
        services.AddScoped<IPrStatusRepo, PrStatusRepo>();
        services.AddScoped<IReportRepo, ReportRepo>();
        services.AddScoped<IReportParameterRepo, ReportParameterRepo>();
        services.AddScoped<INotificationRepo, NotificationRepo>();
        services.AddScoped<IDashboardRepo, DashboardRepo>();
        
        // Customer Order Management
        services.AddScoped<ICustomerRepo, CustomerRepo>();
        services.AddScoped<ICustomerOrderRepo, CustomerOrderRepo>();
        services.AddScoped<IDeliveryOrderRepo, DeliveryOrderRepo>();
        services.AddScoped<ISalesInvoiceRepo, SalesInvoiceRepo>();
        
        // Purchase Order Management
        services.AddScoped<IPoRepo, PoRepo>();
        services.AddScoped<IPoLineRepo, PoLineRepo>();
        
        // Supplier Management
        services.AddScoped<ISupplierRepo, SupplierRepo>();
        
        // Goods Receipt Management
        services.AddScoped<IGoodsReceiptRepo, GoodsReceiptRepo>();
        
        // BPB (Bon Pengambilan Barang) Management
        services.AddScoped<IBpbRepo, BpbRepo>();
        
        // Production Management
        services.AddScoped<IProdRepo, ProdRepo>();
        
        // Material Customer Management
        services.AddScoped<IMaterialCustomerRepo, MaterialCustomerRepo>();
        
        // Penerimaan Hasil Produksi Management
        services.AddScoped<IPhpRepo, PhpRepo>();
        
        // Penyesuaian Stock Management
        services.AddScoped<IPsRepo, PsRepo>();
        
        // Inventory Lock Management
        services.AddScoped<IInventLockRepo, InventLockRepo>();
        
        // Workflow Management
        services.AddScoped<IWfFormRepo, WfFormRepo>();
        services.AddScoped<IWfRepo, WfRepo>();
        services.AddScoped<IWfTransRepo, WfTransRepo>();
        
        // Sales Price & TOP Management
        services.AddScoped<ISalesPriceRepo, SalesPriceRepo>();
        services.AddScoped<ITopRepo, TopRepo>();
        
        // Report Repositories
        services.AddScoped<IRepInventTransRepo, RepInventTransRepo>();
        services.AddScoped<IRepStockRepo, RepStockRepo>();
        services.AddScoped<IRepCustomRepo, RepCustomRepo>();
    }
}