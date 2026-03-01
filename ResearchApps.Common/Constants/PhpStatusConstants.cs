namespace ResearchApps.Common.Constants;

/// <summary>
/// Constants for Php (Penerimaan Hasil Produksi) status values
/// </summary>
public static class PhpStatusConstants
{
    /// <summary>
    /// Draft - Php is being prepared, can be edited or deleted
    /// </summary>
    public const int Draft = 0;
    
    /// <summary>
    /// Active - Php has been activated/posted
    /// </summary>
    public const int Active = 1;
    
    /// <summary>
    /// Closed - Php period has been closed
    /// </summary>
    public const int Closed = 2;
    
    /// <summary>
    /// Cancelled - Php has been cancelled
    /// </summary>
    public const int Cancelled = 3;
    
    /// <summary>
    /// Gets the status name for a given status ID
    /// </summary>
    public static string GetStatusName(int statusId) => statusId switch
    {
        Draft => "Draft",
        Active => "Active",
        Closed => "Closed",
        Cancelled => "Cancelled",
        _ => "Unknown"
    };
}
