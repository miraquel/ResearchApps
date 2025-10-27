namespace ResearchApps.Service.Vm.Common;

public class CboRequestVm
{
    public int? Id { get; set; }
    public string SortBy { get; set; } = string.Empty;
    public bool IsSortAscending { get; set; }
    public string? Term { get; set; } = string.Empty;
}