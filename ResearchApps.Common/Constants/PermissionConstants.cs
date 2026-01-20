using System.Reflection;

namespace ResearchApps.Common.Constants;

public static class PermissionConstants
{
    public static class ItemTypes
    {
        public const string Index = "ItemTypes.Index";
        public const string Create = "ItemTypes.Create";
        public const string Edit = "ItemTypes.Edit";
        public const string Delete = "ItemTypes.Delete";
        public const string Details = "ItemTypes.Details";
    }
    
    public static class Items
    {
        public const string Index = "Items.Index";
        public const string Create = "Items.Create";
        public const string Edit = "Items.Edit";
        public const string Delete = "Items.Delete";
        public const string Details = "Items.Details";
    }
    
    public static class Roles
    {
        public const string Index = "Roles.Index";
        public const string Create = "Roles.Create";
        public const string Edit = "Roles.Edit";
        public const string Delete = "Roles.Delete";
        public const string Details = "Roles.Details";
        public const string RolePermissions = "Roles.RolePermissions";
    }
    
    public static class Users
    {
        public const string Index = "Users.Index";
        public const string Create = "Users.Create";
        public const string Edit = "Users.Edit";
        public const string Delete = "Users.Delete";
        public const string Details = "Users.Details";
        public const string UserRoles = "Users.UserRoles";
        public const string ChangePassword = "Users.ChangePassword";
        public const string Suspend = "Users.Suspend";
    }
    
    public static class Warehouses
    {
        public const string Index = "Warehouses.Index";
        public const string Create = "Warehouses.Create";
        public const string Edit = "Warehouses.Edit";
        public const string Delete = "Warehouses.Delete";
        public const string Details = "Warehouses.Details";
    }
    
    public static class ItemDepts
    {
        public const string Index = "ItemDepts.Index";
        public const string Create = "ItemDepts.Create";
        public const string Edit = "ItemDepts.Edit";
        public const string Delete = "ItemDepts.Delete";
        public const string Details = "ItemDepts.Details";
    }
    
    public static class Units
    {
        public const string Index = "Units.Index";
        public const string Create = "Units.Create";
        public const string Edit = "Units.Edit";
        public const string Delete = "Units.Delete";
        public const string Details = "Units.Details";
    }
    
    public static class Status
    {
        public const string Index = "Status.Index";
    }
    
    public class Prs
    {
        public const string Index = "Prs.Index";
        public const string Create = "Prs.Create";
        public const string Edit = "Prs.Edit";
        public const string Delete = "Prs.Delete";
        public const string Details = "Prs.Details";
        public const string Submit = "Prs.Submit";
        public const string Approve = "Prs.Approve";
        public const string Reject = "Prs.Reject";
        public const string Recall = "Prs.Recall";
    }
    
    public class PrLines
    {
        public const string Index = "PrLines.Index";
        public const string Create = "PrLines.Create";
        public const string Edit = "PrLines.Edit";
        public const string Delete = "PrLines.Delete";
        public const string Details = "PrLines.Details";
    }
    
    public class Reports
    {
        public const string Index = "Reports.Index";
        public const string Create = "Reports.Create";
        public const string Edit = "Reports.Edit";
        public const string Delete = "Reports.Delete";
        public const string Details = "Reports.Details";
        public const string Generate = "Reports.Generate";
    }
    
    public class Customers
    {
        public const string Index = "Customers.Index";
        public const string Create = "Customers.Create";
        public const string Edit = "Customers.Edit";
        public const string Delete = "Customers.Delete";
        public const string Details = "Customers.Details";
    }
    
    public class CustomerOrders
    {
        public const string Index = "CustomerOrders.Index";
        public const string Create = "CustomerOrders.Create";
        public const string Edit = "CustomerOrders.Edit";
        public const string Delete = "CustomerOrders.Delete";
        public const string Details = "CustomerOrders.Details";
        public const string Submit = "CustomerOrders.Submit";
        public const string Recall = "CustomerOrders.Recall";
        public const string Reject = "CustomerOrders.Reject";
        public const string Close = "CustomerOrders.Close";
        public const string Approve = "CustomerOrders.Approve";
    }
    
    public class DeliveryOrders
    {
        public const string Index = "DeliveryOrders.Index";
        public const string Create = "DeliveryOrders.Create";
        public const string Edit = "DeliveryOrders.Edit";
        public const string Delete = "DeliveryOrders.Delete";
        public const string Details = "DeliveryOrders.Details";
    }
    
    public class SalesInvoices
    {
        public const string Index = "SalesInvoices.Index";
        public const string Create = "SalesInvoices.Create";
        public const string Edit = "SalesInvoices.Edit";
        public const string Delete = "SalesInvoices.Delete";
        public const string Details = "SalesInvoices.Details";
    }
    
    public class Pos
    {
        public const string Index = "Pos.Index";
        public const string Create = "Pos.Create";
        public const string Edit = "Pos.Edit";
        public const string Delete = "Pos.Delete";
        public const string Details = "Pos.Details";
        public const string Submit = "Pos.Submit";
        public const string Approve = "Pos.Approve";
        public const string Reject = "Pos.Reject";
        public const string Recall = "Pos.Recall";
        public const string Close = "Pos.Close";
    }
    
    public class PoLines
    {
        public const string Index = "PoLines.Index";
        public const string Create = "PoLines.Create";
        public const string Edit = "PoLines.Edit";
        public const string Delete = "PoLines.Delete";
        public const string Details = "PoLines.Details";
    }
    
    public class Suppliers
    {
        public const string Index = "Suppliers.Index";
        public const string Create = "Suppliers.Create";
        public const string Edit = "Suppliers.Edit";
        public const string Delete = "Suppliers.Delete";
        public const string Details = "Suppliers.Details";
    }
    
    // Get all permissions from PermissionConstants
    public static List<string> GetAllPermissions()
    {
        var permissionType = typeof(PermissionConstants);
        var members = permissionType.GetNestedTypes();
        var fieldInfos = new List<FieldInfo>();
        foreach (var member in members)
        {
            fieldInfos.AddRange(member.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));
        }

        return fieldInfos
            .Where(f => f is { IsLiteral: true, IsInitOnly: false } && f.FieldType == typeof(string))
            .Select(f => f.GetValue(null)?.ToString() ?? string.Empty)
            .ToList();
    }
    
    public static Dictionary<string, List<string>> GetGroupedPermissions()
    {
        var permissions = new Dictionary<string, List<string>>();
        var permissionType = typeof(PermissionConstants);
        foreach (var nestedType in permissionType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
        {
            var groupName = nestedType.Name;
            var fields =
                nestedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var permissionList = fields
                .Where(f => f.FieldType == typeof(string))
                .Select(f => f.GetValue(null) as string ?? string.Empty)
                .ToList();
            permissions[groupName] = permissionList;
        }
        
        return permissions;
    }
}