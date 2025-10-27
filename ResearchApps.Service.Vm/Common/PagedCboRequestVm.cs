namespace ResearchApps.Service.Vm.Common;

public class PagedCboRequestVm : CboRequestVm
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}