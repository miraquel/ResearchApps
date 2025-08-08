namespace ResearchApps.Service.Vm.Common;

public class PagedListRequestVm : ListRequestVm
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}