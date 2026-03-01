namespace ResearchApps.Domain;

/// <summary>
/// Represents an inventory lock record for monthly inventory closing.
/// </summary>
public class InventLock
{
    public int RecId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public bool Lock { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets the month name (e.g., "January", "February", etc.)
    /// </summary>
    public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM");
    
    /// <summary>
    /// Gets the period string (e.g., "January 2026")
    /// </summary>
    public string Period => $"{MonthName} {Year}";
}
