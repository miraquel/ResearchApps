using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class TopVm
{
    [Display(Name = "ID")]
    public int TopId { get; set; }

    [Required(ErrorMessage = "TOP Name is required")]
    [StringLength(20, ErrorMessage = "TOP Name cannot exceed 20 characters")]
    [Display(Name = "TOP Name")]
    public string TopName { get; set; } = string.Empty;

    [Required(ErrorMessage = "TOP Day is required")]
    [Range(0, int.MaxValue, ErrorMessage = "TOP Day must be a positive number")]
    [Display(Name = "TOP Day")]
    public int TopDay { get; set; }

    [Display(Name = "Status ID")]
    public int StatusId { get; set; } = 1;

    [Display(Name = "Status")]
    public string StatusName { get; set; } = string.Empty;

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;

    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }

    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
}
