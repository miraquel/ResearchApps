using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class ReportVm
{
    [Display(Name = "ID")]
    public int ReportId { get; set; }
    
    [Display(Name = "Report Name")]
    [Required(ErrorMessage = "Report name is required.")]
    [StringLength(200, ErrorMessage = "Report name cannot exceed 200 characters.")]
    public string ReportName { get; set; } = string.Empty;
    
    [Display(Name = "Description")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string? Description { get; set; }
    
    [Display(Name = "Report Type")]
    [Required(ErrorMessage = "Report type is required.")]
    public int ReportType { get; set; } = 1;
    
    [Display(Name = "Report Type Name")]
    public string ReportTypeName => ReportType switch
    {
        1 => "Fully Generated",
        2 => "Partially Generated",
        _ => "Unknown"
    };
    
    [Display(Name = "SQL Query")]
    public string? SqlQuery { get; set; }
    
    [Display(Name = "Template Path")]
    public string? TemplatePath { get; set; }
    
    [Display(Name = "Template File Name")]
    public string? TemplateFileName { get; set; }
    
    [Display(Name = "Template Content Type")]
    public string? TemplateContentType { get; set; }
    
    [Display(Name = "Has Template")]
    public bool HasTemplateFile => !string.IsNullOrEmpty(TemplateFileName);
    
    [Display(Name = "Status ID")]
    [Required(ErrorMessage = "Status is required.")]
    public int StatusId { get; set; }
    
    [Display(Name = "Status")]
    public string? StatusName { get; set; }
    
    [Display(Name = "Page Size")]
    public string PageSize { get; set; } = "A4";
    
    [Display(Name = "Orientation")]
    public int Orientation { get; set; } = 1;
    
    [Display(Name = "Orientation Name")]
    public string OrientationName => Orientation switch
    {
        1 => "Portrait",
        2 => "Landscape",
        _ => "Unknown"
    };
    
    [Display(Name = "Parameters")]
    public List<ReportParameterVm> Parameters { get; set; } = new();
    
    [Display(Name = "Field Coordinates")]
    public List<ReportFieldCoordinateVm> FieldCoordinates { get; set; } = new();
    
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
    
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;
    
    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }
    
    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
}

