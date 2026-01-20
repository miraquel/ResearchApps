namespace ResearchApps.Common.Constants;

public static class PoStatusConstants
{
    public const int Draft = 0;
    public const int Active = 1;
    public const int Cancelled = 2;
    public const int Closed = 3;
    public const int Pending = 4;
    public const int Approved = 5;
    public const int Rejected = 6;
    
    public static string GetStatusName(int statusId) => statusId switch
    {
        Draft => "Draft",
        Active => "Active",
        Cancelled => "Cancelled",
        Closed => "Closed",
        Pending => "Pending Approval",
        Approved => "Approved",
        Rejected => "Rejected",
        _ => "Unknown"
    };
}
