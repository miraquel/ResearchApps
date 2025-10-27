namespace ResearchApps.Domain.Common;

public class PagedCboRequest : CboRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}