using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class PoHeaderOutstandingVm
{
    [Display(Name = "PO Number")]
    public string PoId { get; set; } = string.Empty;

    public int RecId { get; set; }

    [Display(Name = "PO Date")]
    public DateTime PoDate { get; set; }

    [Display(Name = "PO Name")]
    public string PoName { get; set; } = string.Empty;

    [Display(Name = "Supplier")]
    public int SupplierId { get; set; }

    [Display(Name = "Supplier Name")]
    public string SupplierName { get; set; } = string.Empty;

    [Display(Name = "Total Amount")]
    public decimal? TotalAmount { get; set; }

    [Display(Name = "Status")]
    public int PoStatusId { get; set; }

    [Display(Name = "Status")]
    public string PoStatusName { get; set; } = string.Empty;
}
