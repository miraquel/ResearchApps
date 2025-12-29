namespace ResearchApps.Domain;

/// <summary>
/// Represents a report definition in the system.
/// </summary>
public class Report
{
    /// <summary>
    /// Primary key for the report.
    /// </summary>
    public int ReportId { get; set; }
    
    /// <summary>
    /// Name of the report.
    /// </summary>
    public string ReportName { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the report.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// The report type: 1 = Fully Generated, 2 = Partially Generated (for continuous paper).
    /// </summary>
    public ReportType ReportType { get; set; } = ReportType.FullyGenerated;
    
    /// <summary>
    /// SQL query or stored procedure name for the report data.
    /// </summary>
    public string? SqlQuery { get; set; }
    
    /// <summary>
    /// The template file name or path (for fully generated reports).
    /// </summary>
    public string? TemplatePath { get; set; }
    
    /// <summary>
    /// The original template file name uploaded by user.
    /// </summary>
    public string? TemplateFileName { get; set; }
    
    /// <summary>
    /// The template file content stored as byte array (for fully generated reports).
    /// </summary>
    public byte[]? TemplateFileContent { get; set; }
    
    /// <summary>
    /// The MIME type of the template file.
    /// </summary>
    public string? TemplateContentType { get; set; }
    
    /// <summary>
    /// Status ID (active/inactive).
    /// </summary>
    public int StatusId { get; set; }
    
    /// <summary>
    /// Status name for display.
    /// </summary>
    public string? StatusName { get; set; }
    
    /// <summary>
    /// Page size for fully generated reports.
    /// </summary>
    public string PageSize { get; set; } = "A4";
    
    /// <summary>
    /// Page orientation: Portrait or Landscape.
    /// </summary>
    public PageOrientation Orientation { get; set; } = PageOrientation.Portrait;
    
    /// <summary>
    /// Report parameters.
    /// </summary>
    public virtual ICollection<ReportParameter>? Parameters { get; set; }
    
    /// <summary>
    /// Field coordinates for partially generated reports (continuous paper).
    /// </summary>
    public virtual ICollection<ReportFieldCoordinate>? FieldCoordinates { get; set; }
    
    /// <summary>
    /// Created date.
    /// </summary>
    public DateTime CreatedDate { get; set; }
    
    /// <summary>
    /// Created by username.
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// Modified date.
    /// </summary>
    public DateTime ModifiedDate { get; set; }
    
    /// <summary>
    /// Modified by username.
    /// </summary>
    public string ModifiedBy { get; set; } = string.Empty;
}

/// <summary>
/// Type of report generation.
/// </summary>
public enum ReportType
{
    /// <summary>
    /// Fully generated template - generates the entire report including header, table, and footer.
    /// </summary>
    FullyGenerated = 1,
    
    /// <summary>
    /// Partially generated template - generates only main information while table/fields are pre-printed on continuous paper.
    /// </summary>
    PartiallyGenerated = 2
}

/// <summary>
/// Page orientation for the report.
/// </summary>
public enum PageOrientation
{
    Portrait = 1,
    Landscape = 2
}
