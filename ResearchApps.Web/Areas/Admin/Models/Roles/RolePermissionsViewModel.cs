using System.Globalization;
using System.Text.RegularExpressions;

namespace ResearchApps.Web.Areas.Admin.Models.Roles;

public class RolePermissionsViewModel
{
    public string RoleId { get; set; } = string.Empty;
    public List<string> SelectedPermissions { get; set; } = new();
    public Dictionary<string, List<string>> GroupedPermissions { get; set; } = new();
    public string GetPermissionFriendlyName(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            return permission;
        var parts = permission.Split('.');
        var last = parts.Length > 1 ? parts[1] : parts[0];
        var friendly = Regex.Replace(last, "([a-z])([A-Z])", "$1 $2");
        friendly = Regex.Replace(friendly, "([A-Z])([A-Z][a-z])", "$1 $2");
        return char.ToUpper(friendly[0]) + friendly[1..];
    }
    public string GetGroupFriendlyName(string group)
    {
        if (string.IsNullOrWhiteSpace(group))
            return group;
        var friendly = Regex.Replace(group, "([a-z])([A-Z])", "$1 $2");
        friendly = Regex.Replace(friendly, "([A-Z])([A-Z][a-z])", "$1 $2");
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(friendly.Trim());
    }
}