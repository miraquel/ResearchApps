namespace ResearchApps.Domain;

/// <summary>
/// Represents a parameter definition for a report.
/// </summary>
public class ReportParameter
{
    /// <summary>
    /// Primary key for the report parameter.
    /// </summary>
    public int ParameterId { get; set; }
    
    /// <summary>
    /// Foreign key to the Report.
    /// </summary>
    public int ReportId { get; set; }
    
    /// <summary>
    /// Parameter name (used in SQL query).
    /// </summary>
    public string ParameterName { get; set; } = string.Empty;
    
    /// <summary>
    /// Display label for the parameter.
    /// </summary>
    public string DisplayLabel { get; set; } = string.Empty;
    
    /// <summary>
    /// Data type of the parameter.
    /// </summary>
    public ParameterDataType DataType { get; set; } = ParameterDataType.String;
    
    /// <summary>
    /// Default value for the parameter.
    /// </summary>
    public string? DefaultValue { get; set; }
    
    /// <summary>
    /// Indicates if the parameter is required.
    /// </summary>
    public bool IsRequired { get; set; } = true;
    
    /// <summary>
    /// Order of display in the UI.
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Lookup source for dropdown parameters (e.g., SQL query or table name).
    /// </summary>
    public string? LookupSource { get; set; }
    
    /// <summary>
    /// Placeholder text for input fields.
    /// </summary>
    public string? Placeholder { get; set; }
    
    /// <summary>
    /// Navigation property to Report.
    /// </summary>
    public virtual Report? Report { get; set; }
}

/// <summary>
/// Data type for report parameters.
/// </summary>
public enum ParameterDataType
{
    String = 1,
    Integer = 2,
    Decimal = 3,
    DateTime = 4,
    Date = 5,
    Boolean = 6,
    Dropdown = 7
}

