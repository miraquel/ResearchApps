using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class ProdStatusVm
{
    public int ProdStatusId { get; set; }

    [Display(Name = "Status Name")]
    public string ProdStatusName { get; set; } = string.Empty;
}
