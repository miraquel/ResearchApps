namespace ResearchApps.Domain;

/// <summary>
/// Represents the print coordinates for a field in a partially generated report.
/// Used to position data on pre-printed continuous paper.
/// </summary>
public class ReportFieldCoordinate
{
    /// <summary>
    /// Primary key for the field coordinate.
    /// </summary>
    public int CoordinateId { get; set; }
    
    /// <summary>
    /// Foreign key to the Report.
    /// </summary>
    public int ReportId { get; set; }
    
    /// <summary>
    /// Name of the field (e.g., "CustomerName", "InvoiceNo", "Date").
    /// </summary>
    public string FieldName { get; set; } = string.Empty;
    
    /// <summary>
    /// Display label for the field.
    /// </summary>
    public string DisplayLabel { get; set; } = string.Empty;
    
    /// <summary>
    /// The X coordinate (horizontal position) in millimeters or points from the left edge.
    /// </summary>
    public decimal XPosition { get; set; }
    
    /// <summary>
    /// The Y coordinate (vertical position) in millimeters or points from the top edge.
    /// </summary>
    public decimal YPosition { get; set; }
    
    /// <summary>
    /// Width of the field area in millimeters or points.
    /// </summary>
    public decimal Width { get; set; }
    
    /// <summary>
    /// Height of the field area in millimeters or points.
    /// </summary>
    public decimal Height { get; set; }
    
    /// <summary>
    /// Font size for the field text.
    /// </summary>
    public decimal FontSize { get; set; } = 10;
    
    /// <summary>
    /// Font family for the field text.
    /// </summary>
    public string FontFamily { get; set; } = "Arial";
    
    /// <summary>
    /// Text alignment: Left, Center, Right.
    /// </summary>
    public TextAlignment Alignment { get; set; } = TextAlignment.Left;
    
    /// <summary>
    /// Indicates if this field is bold.
    /// </summary>
    public bool IsBold { get; set; }
    
    /// <summary>
    /// Indicates if this field is italic.
    /// </summary>
    public bool IsItalic { get; set; }
    
    /// <summary>
    /// The data source binding (SQL column name or parameter name).
    /// </summary>
    public string? DataBinding { get; set; }
    
    /// <summary>
    /// Format string for the field value (e.g., "N2" for decimal, "dd/MM/yyyy" for date).
    /// </summary>
    public string? FormatString { get; set; }
    
    /// <summary>
    /// Order of the field for processing.
    /// </summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Indicates if this field is part of a repeating section (like table rows).
    /// </summary>
    public bool IsRepeating { get; set; }
    
    /// <summary>
    /// Row increment in points/mm for repeating fields.
    /// </summary>
    public decimal RowIncrement { get; set; }
    
    /// <summary>
    /// Navigation property to Report.
    /// </summary>
    public virtual Report? Report { get; set; }
}

/// <summary>
/// Text alignment options for field coordinates.
/// </summary>
public enum TextAlignment
{
    Left = 1,
    Center = 2,
    Right = 3
}

