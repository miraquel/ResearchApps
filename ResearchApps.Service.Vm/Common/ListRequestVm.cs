namespace ResearchApps.Service.Vm.Common;

public class ListRequestVm
{
    public string SortBy { get; set; } = string.Empty;
    public bool IsSortAscending { get; set; }   
    public Dictionary<string, string> Filters { get; set; } = new();
}