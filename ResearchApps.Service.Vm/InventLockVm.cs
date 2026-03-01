using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

/// <summary>
/// ViewModel for Inventory Lock records.
/// </summary>
public class InventLockVm
{
    [Display(Name = "Record ID")]
    public int RecId { get; set; }
    
    [Display(Name = "Year")]
    public int Year { get; set; }
    
    [Display(Name = "Month")]
    public int Month { get; set; }
    
    [Display(Name = "Locked")]
    public bool Lock { get; set; }
    
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
    
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;
    
    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }
    
    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets the month name (e.g., "January", "February", etc.)
    /// </summary>
    public string MonthName => Month >= 1 && Month <= 12 
        ? new DateTime(Year > 0 ? Year : 2000, Month, 1).ToString("MMMM") 
        : string.Empty;
    
    /// <summary>
    /// Gets the period string (e.g., "January 2026")
    /// </summary>
    public string Period => $"{MonthName} {Year}";
    
    /// <summary>
    /// Gets the status display text
    /// </summary>
    public string StatusText => Lock ? "Closed" : "Open";
    
    /// <summary>
    /// Gets the status badge HTML
    /// </summary>
    public string StatusBadge => Lock 
        ? "<span class=\"badge bg-danger-subtle text-danger\">Closed</span>" 
        : "<span class=\"badge bg-success-subtle text-success\">Open</span>";
}
