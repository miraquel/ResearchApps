using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class ReportParameterVm
{
    [Display(Name = "Parameter ID")]
    public int ParameterId { get; set; }
    
    [Display(Name = "Report ID")]
    [Required(ErrorMessage = "Report ID is required.")]
    public int ReportId { get; set; }
    
    [Display(Name = "Parameter Name")]
    [Required(ErrorMessage = "Parameter name is required.")]
    [StringLength(100, ErrorMessage = "Parameter name cannot exceed 100 characters.")]
    public string ParameterName { get; set; } = string.Empty;
    
    [Display(Name = "Display Label")]
    [Required(ErrorMessage = "Display label is required.")]
    [StringLength(200, ErrorMessage = "Display label cannot exceed 200 characters.")]
    public string DisplayLabel { get; set; } = string.Empty;
    
    [Display(Name = "Data Type")]
    [Required(ErrorMessage = "Data type is required.")]
    public int DataType { get; set; } = 1;
    
    [Display(Name = "Data Type Name")]
    public string DataTypeName => DataType switch
    {
        1 => "String",
        2 => "Integer",
        3 => "Decimal",
        4 => "DateTime",
        5 => "Date",
        6 => "Boolean",
        7 => "Dropdown",
        _ => "Unknown"
    };
    
    [Display(Name = "Default Value")]
    [StringLength(500, ErrorMessage = "Default value cannot exceed 500 characters.")]
    public string? DefaultValue { get; set; }
    
    [Display(Name = "Is Required")]
    public bool IsRequired { get; set; } = true;
    
    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }
    
    [Display(Name = "Lookup Source")]
    [StringLength(1000, ErrorMessage = "Lookup source cannot exceed 1000 characters.")]
    public string? LookupSource { get; set; }
    
    [Display(Name = "Placeholder")]
    [StringLength(200, ErrorMessage = "Placeholder cannot exceed 200 characters.")]
    public string? Placeholder { get; set; }
}

