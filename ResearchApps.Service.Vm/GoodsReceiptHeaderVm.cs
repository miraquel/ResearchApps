using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class GoodsReceiptHeaderVm
{
    [Display(Name = "GR No")]
    public string GrId { get; set; } = string.Empty;

    [Display(Name = "GR Date")]
    [Required(ErrorMessage = "GR Date is required")]
    public DateTime GrDate { get; set; }

    [Display(Name = "GR Date")]
    public string? GrDateStr { get; set; }

    [Display(Name = "Supplier")]
    [Required(ErrorMessage = "Supplier is required")]
    public int SupplierId { get; set; }

    [Display(Name = "Supplier")]
    public string? SupplierName { get; set; }

    [Display(Name = "Reference No")]
    public string? RefNo { get; set; }

    [Display(Name = "Subtotal")]
    public decimal SubTotal { get; set; }

    [Display(Name = "PPN (11%)")]
    public decimal Ppn { get; set; }

    [Display(Name = "Total")]
    public decimal Total { get; set; }

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Status")]
    public int GrStatusId { get; set; } = 1;

    [Display(Name = "Status")]
    public string? GrStatusName { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }

    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;

    [Display(Name = "Record ID")]
    public int RecId { get; set; }
}
