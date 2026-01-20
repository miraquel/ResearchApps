using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

/// <summary>
/// ViewModel for generating a report with parameters.
/// </summary>
public class ReportGenerateVm
{
    [Display(Name = "Report ID")]
    [Required(ErrorMessage = "Report ID is required.")]
    public int ReportId { get; set; }
    
    [Display(Name = "Report Name")]
    public string ReportName { get; set; } = string.Empty;
    
    [Display(Name = "Report Type")]
    public int ReportType { get; set; }
    
    [Display(Name = "Parameters")]
    public List<ReportParameterVm> Parameters { get; set; } = new();
    
    /// <summary>
    /// Dictionary of parameter values keyed by parameter name.
    /// </summary>
    public Dictionary<string, string> ParameterValues { get; set; } = new();
    
    [Display(Name = "Output Format")]
    public ReportOutputFormat OutputFormat { get; set; } = ReportOutputFormat.Pdf;
}

/// <summary>
/// Output format for the report.
/// </summary>
public enum ReportOutputFormat
{
    Pdf = 1,
    Excel = 2,
    Html = 3
}

