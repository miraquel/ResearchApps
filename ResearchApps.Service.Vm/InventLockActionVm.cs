using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

/// <summary>
/// ViewModel for Inventory Lock actions (Close/Open).
/// </summary>
public class InventLockActionVm
{
    [Display(Name = "Record ID")]
    public int RecId { get; set; }
    
    [Display(Name = "Year")]
    public int Year { get; set; }
    
    [Display(Name = "Month")]
    public int Month { get; set; }
}
