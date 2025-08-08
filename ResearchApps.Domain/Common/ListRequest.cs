namespace ResearchApps.Domain.Common;

public class ListRequest
{
    public string SortBy { get; set; } = string.Empty;
    public bool IsSortAscending { get; set; }
    public Dictionary<string, string> Filters { get; set; } = new();
}