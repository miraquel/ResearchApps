namespace ResearchApps.Common.Constants;

/// <summary>
/// Constants for Goods Receipt status values
/// </summary>
public static class GrStatusConstants
{
    /// <summary>
    /// Draft - GR is being prepared, can be edited or deleted
    /// </summary>
    public const int Draft = 0;
    
    /// <summary>
    /// Posted - GR has been posted, cannot be edited or deleted
    /// </summary>
    public const int Posted = 1;
    
    /// <summary>
    /// Gets the status name for a given status ID
    /// </summary>
    public static string GetStatusName(int statusId) => statusId switch
    {
        Draft => "Draft",
        Posted => "Posted",
        _ => "Unknown"
    };
}
