namespace ResearchApps.Common.Constants;

public static class PoStatusConstants
{
    public const int Draft = 0;
    public const int Active = 1;
    public const int Closed = 2;
    public const int Cancelled = 3;
    public const int InReview = 4;
    public const int Rejected = 5;
    
    public static string GetStatusName(int statusId) => statusId switch
    {
        Draft => "Draft",
        Active => "Active",
        Cancelled => "Cancelled",
        Closed => "Closed",
        InReview => "Pending Approval",
        Rejected => "Rejected",
        _ => "Unknown"
    };
}
