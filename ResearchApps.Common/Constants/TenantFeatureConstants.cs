using System.Reflection;

namespace ResearchApps.Common.Constants;

/// <summary>
/// Well-known feature flag keys for tenant-level feature gating.
/// Use these constants everywhere instead of raw strings.
/// </summary>
public static class TenantFeatureConstants
{
    // ── Sales ──────────────────────────────────────────────────────────────────
    public const string Customers = "Customers";
    public const string CustomerOrders = "CustomerOrders";
    public const string DeliveryOrders = "DeliveryOrders";
    public const string SalesInvoices = "SalesInvoices";
    public const string SalesPrices = "SalesPrices";

    // ── Procurement ────────────────────────────────────────────────────────────
    public const string Budgets = "Budgets";
    public const string Suppliers = "Suppliers";
    public const string PurchaseRequisitions = "PurchaseRequisitions";
    public const string PurchaseOrders = "PurchaseOrders";
    public const string GoodsReceipts = "GoodsReceipts";

    // ── Inventory ──────────────────────────────────────────────────────────────
    public const string Items = "Items";
    public const string ItemTypes = "ItemTypes";
    public const string ItemDepts = "ItemDepts";
    public const string ItemGroup01s = "ItemGroup01s";
    public const string ItemGroup02s = "ItemGroup02s";
    public const string Warehouses = "Warehouses";
    public const string Units = "Units";
    public const string StockAdjustments = "StockAdjustments";
    public const string StockReport = "StockReport";

    // ── Production ─────────────────────────────────────────────────────────────
    public const string ProductionOrders = "ProductionOrders";
    public const string MaterialCustomers = "MaterialCustomers";
    public const string MaterialWithdrawals = "MaterialWithdrawals";
    public const string Phps = "Phps";
    public const string ToolsReport = "ToolsReport";

    // ── Finance ────────────────────────────────────────────────────────────────
    public const string TermsOfPayment = "TermsOfPayment";
    public const string InventoryClosing = "InventoryClosing";
    public const string InventTransReport = "InventTransReport";

    // ── Reports ────────────────────────────────────────────────────────────────
    public const string Reports = "Reports";

    // ── Section aliases (nested classes for section-qualified access) ──────────
    public static class Sales
    {
        public const string Customers = "Customers";
        public const string CustomerOrders = "CustomerOrders";
        public const string DeliveryOrders = "DeliveryOrders";
        public const string SalesInvoices = "SalesInvoices";
        public const string SalesPrices = "SalesPrices";
    }

    public static class Procurement
    {
        public const string Budgets = "Budgets";
        public const string Suppliers = "Suppliers";
        public const string PurchaseRequisitions = "PurchaseRequisitions";
        public const string PurchaseOrders = "PurchaseOrders";
        public const string GoodsReceipts = "GoodsReceipts";
    }

    public static class Inventory
    {
        public const string Items = "Items";
        public const string ItemTypes = "ItemTypes";
        public const string ItemDepts = "ItemDepts";
        public const string ItemGroup01s = "ItemGroup01s";
        public const string ItemGroup02s = "ItemGroup02s";
        public const string Warehouses = "Warehouses";
        public const string Units = "Units";
        public const string StockAdjustments = "StockAdjustments";
        public const string StockReport = "StockReport";
    }

    public static class Production
    {
        public const string ProductionOrders = "ProductionOrders";
        public const string MaterialCustomers = "MaterialCustomers";
        public const string MaterialWithdrawals = "MaterialWithdrawals";
        public const string Phps = "Phps";
        public const string ToolsReport = "ToolsReport";
    }

    public static class Finance
    {
        public const string TermsOfPayment = "TermsOfPayment";
        public const string InventoryClosing = "InventoryClosing";
        public const string InventTransReport = "InventTransReport";
    }

    public static class ReportsSection
    {
        public const string Reports = "Reports";
    }

    /// <summary>
    /// Returns all feature keys grouped by section for admin UI rendering.
    /// </summary>
    public static Dictionary<string, List<(string Key, string DisplayName)>> GetAllBySection()
    {
        return new Dictionary<string, List<(string Key, string DisplayName)>>
        {
            ["Sales"] =
            [
                (Customers, "Customers"),
                (CustomerOrders, "Customer Orders"),
                (DeliveryOrders, "Delivery Orders"),
                (SalesInvoices, "Sales Invoices"),
                (SalesPrices, "Sales Prices")
            ],
            ["Procurement"] =
            [
                (Budgets, "Budgets"),
                (Suppliers, "Suppliers"),
                (PurchaseRequisitions, "Purchase Requisitions"),
                (PurchaseOrders, "Purchase Orders"),
                (GoodsReceipts, "Goods Receipts")
            ],
            ["Inventory"] =
            [
                (Items, "Items"),
                (ItemTypes, "Item Types"),
                (ItemDepts, "Item Departments"),
                (ItemGroup01s, "Item Group 01"),
                (ItemGroup02s, "Item Group 02"),
                (Warehouses, "Warehouses"),
                (Units, "Units"),
                (StockAdjustments, "Stock Adjustments"),
                (StockReport, "Stock Report")
            ],
            ["Production"] =
            [
                (ProductionOrders, "Production Orders"),
                (MaterialCustomers, "Material Customers"),
                (MaterialWithdrawals, "Material Withdrawals (BPB)"),
                (Phps, "Penerimaan Hasil Produksi (PHP)"),
                (ToolsReport, "Tools Report")
            ],
            ["Finance"] =
            [
                (TermsOfPayment, "Terms of Payment"),
                (InventoryClosing, "Inventory Closing"),
                (InventTransReport, "Invent Trans Report")
            ],
            ["Reports"] =
            [
                (Reports, "Reports")
            ]
        };
    }

    /// <summary>
    /// Returns all feature keys as a flat set.
    /// </summary>
    public static HashSet<string> GetAll()
    {
        return typeof(TenantFeatureConstants)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToHashSet();
    }
}
