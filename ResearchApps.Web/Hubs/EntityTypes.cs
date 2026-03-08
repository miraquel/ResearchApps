namespace ResearchApps.Web.Hubs;

public static class EntityTypes
{
    public const string Pr = "pr";
    public const string Co = "co";
    public const string Po = "po";

    private static readonly HashSet<string> Registered = [Pr, Co, Po];

    public static bool IsValid(string entityType) => Registered.Contains(entityType);
}
