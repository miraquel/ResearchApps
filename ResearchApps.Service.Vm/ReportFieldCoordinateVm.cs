using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

/// <summary>
/// ViewModel for report field coordinates used in partially generated reports.
/// </summary>
public class ReportFieldCoordinateVm
{
    [Display(Name = "Coordinate ID")]
    public int CoordinateId { get; set; }
    
    [Display(Name = "Report ID")]
    [Required(ErrorMessage = "Report ID is required.")]
    public int ReportId { get; set; }
    
    [Display(Name = "Field Name")]
    [Required(ErrorMessage = "Field name is required.")]
    [StringLength(100, ErrorMessage = "Field name cannot exceed 100 characters.")]
    public string FieldName { get; set; } = string.Empty;
    
    [Display(Name = "Display Label")]
    [Required(ErrorMessage = "Display label is required.")]
    [StringLength(200, ErrorMessage = "Display label cannot exceed 200 characters.")]
    public string DisplayLabel { get; set; } = string.Empty;
    
    [Display(Name = "X Position (mm)")]
    [Required(ErrorMessage = "X position is required.")]
    [Range(0, 1000, ErrorMessage = "X position must be between 0 and 1000.")]
    public decimal XPosition { get; set; }
    
    [Display(Name = "Y Position (mm)")]
    [Required(ErrorMessage = "Y position is required.")]
    [Range(0, 1000, ErrorMessage = "Y position must be between 0 and 1000.")]
    public decimal YPosition { get; set; }
    
    [Display(Name = "Width (mm)")]
    [Range(1, 500, ErrorMessage = "Width must be between 1 and 500.")]
    public decimal Width { get; set; } = 50;
    
    [Display(Name = "Height (mm)")]
    [Range(1, 100, ErrorMessage = "Height must be between 1 and 100.")]
    public decimal Height { get; set; } = 5;
    
    [Display(Name = "Font Size")]
    [Range(6, 72, ErrorMessage = "Font size must be between 6 and 72.")]
    public decimal FontSize { get; set; } = 10;
    
    [Display(Name = "Font Family")]
    [StringLength(100, ErrorMessage = "Font family cannot exceed 100 characters.")]
    public string FontFamily { get; set; } = "Arial";
    
    [Display(Name = "Alignment")]
    public int Alignment { get; set; } = 1;
    
    [Display(Name = "Alignment Name")]
    public string AlignmentName => Alignment switch
    {
        1 => "Left",
        2 => "Center",
        3 => "Right",
        _ => "Unknown"
    };
    
    [Display(Name = "Bold")]
    public bool IsBold { get; set; }
    
    [Display(Name = "Italic")]
    public bool IsItalic { get; set; }
    
    [Display(Name = "Data Binding")]
    [StringLength(200, ErrorMessage = "Data binding cannot exceed 200 characters.")]
    public string? DataBinding { get; set; }
    
    [Display(Name = "Format String")]
    [StringLength(50, ErrorMessage = "Format string cannot exceed 50 characters.")]
    public string? FormatString { get; set; }
    
    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }
    
    [Display(Name = "Is Repeating")]
    public bool IsRepeating { get; set; }
    
    [Display(Name = "Row Increment (mm)")]
    [Range(0, 100, ErrorMessage = "Row increment must be between 0 and 100.")]
    public decimal RowIncrement { get; set; }
}

