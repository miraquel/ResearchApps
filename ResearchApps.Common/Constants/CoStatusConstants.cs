namespace ResearchApps.Common.Constants;

/// <summary>
/// Constants for Customer Order status IDs.
/// </summary>
public static class CoStatusConstants
{
    /// <summary>Draft - Order is being prepared, not yet submitted.</summary>
    public const int Draft = 0;
    
    /// <summary>Active - Order is active and ready for delivery.</summary>
    public const int Active = 1;
    
    /// <summary>Cancelled - Order has been cancelled.</summary>
    public const int Cancel = 2;
    
    /// <summary>Closed - Order is complete/closed.</summary>
    public const int Closed = 3;
    
    /// <summary>In Review - Order is pending approval.</summary>
    public const int InReview = 4;
    
    /// <summary>Rejected - Order has been rejected by approver.</summary>
    public const int Reject = 5;
}
