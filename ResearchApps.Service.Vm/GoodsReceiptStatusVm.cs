using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class GoodsReceiptStatusVm
{
    [Display(Name = "Status ID")]
    public int GrStatusId { get; set; }

    [Display(Name = "Status")]
    public string GrStatusName { get; set; } = string.Empty;
}
