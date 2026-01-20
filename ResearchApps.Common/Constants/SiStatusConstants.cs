namespace ResearchApps.Common.Constants;

/// <summary>
/// Sales Invoice status constants matching database SiStatus table
/// </summary>
public static class SiStatusConstants
{
    /// <summary>
    /// Draft - Invoice is being prepared (label-warning)
    /// </summary>
    public const int Draft = 0;
    
    /// <summary>
    /// Active/Completed - Invoice is finalized (label-success)
    /// </summary>
    public const int Active = 1;
    
    /// <summary>
    /// Processing - Invoice is being processed (label-primary)
    /// </summary>
    public const int Processing = 2;
    
    /// <summary>
    /// Cancelled/Void - Invoice has been cancelled (label-danger)
    /// </summary>
    public const int Cancelled = 3;
}
