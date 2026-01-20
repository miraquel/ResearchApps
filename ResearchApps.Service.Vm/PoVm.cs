using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

/// <summary>
/// Composite ViewModel for Purchase Order with Header and Lines
/// </summary>
public class PoVm
{
    public PoHeaderVm Header { get; set; } = new();
    public List<PoLineVm> Lines { get; set; } = new();
}

public class PoHeaderVm
{
    [Display(Name = "PO Number")]
    public string PoId { get; set; } = string.Empty;

    public int RecId { get; set; }

    [Required(ErrorMessage = "PO Date is required")]
    [Display(Name = "PO Date")]
    public DateTime PoDate { get; set; }

    [Required(ErrorMessage = "Supplier is required")]
    [Display(Name = "Supplier")]
    public int SupplierId { get; set; }

    [Display(Name = "Supplier Name")]
    public string SupplierName { get; set; } = string.Empty;
    
    [Display(Name = "PIC")]
    public string Pic { get; set; } = string.Empty;
    
    [Display(Name = "Ref No.")]
    public string RefNo { get; set; } = string.Empty;
    
    [Display(Name = "Include PPN")]
    public bool IsPpn { get; set; }
    
    [Display(Name = "Sub Total")]
    public decimal SubTotal { get; set; }
    
    [Display(Name = "VAT Total")]
    public decimal Ppn { get; set; }

    [Display(Name = "Total Amount")]
    public decimal Total { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    // Workflow Fields
    [Display(Name = "Status")]
    public int PoStatusId { get; set; }

    [Display(Name = "Status")]
    public string PoStatusName { get; set; } = string.Empty;

    public int? WfTransId { get; set; }

    [Display(Name = "Current Approver")]
    public string? CurrentApprover { get; set; }

    public int? CurrentIndex { get; set; }

    // Audit Fields
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;

    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }
}
