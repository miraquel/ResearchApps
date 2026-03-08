using System.Collections.Frozen;

namespace ResearchApps.Web.Hubs;

public sealed record WorkflowEntityConfig(string DisplayName, string UrlTemplate);

public static class WorkflowEntityRegistry
{
    private static readonly FrozenDictionary<string, WorkflowEntityConfig> Entities =
        new Dictionary<string, WorkflowEntityConfig>
        {
            [EntityTypes.Pr] = new("PR", "/Prs/Details/{0}"),
            [EntityTypes.Co] = new("Customer Order", "/CustomerOrders/Details/{0}"),
            [EntityTypes.Po] = new("Purchase Order", "/Pos/Details/{0}"),
        }.ToFrozenDictionary();

    public static string GetDisplayName(string entityType) =>
        Entities.TryGetValue(entityType, out var config) ? config.DisplayName : entityType.ToUpperInvariant();

    public static string GetUrl(string entityType, int recId) =>
        Entities.TryGetValue(entityType, out var config)
            ? string.Format(config.UrlTemplate, recId)
            : $"/{entityType}/Details/{recId}";

    public static bool IsRegistered(string entityType) => Entities.ContainsKey(entityType);
}
