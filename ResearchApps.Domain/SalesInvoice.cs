namespace ResearchApps.Domain;

/// <summary>
/// Composite entity for Sales Invoice with Header and Lines
/// </summary>
public class SalesInvoice
{
    public SalesInvoiceHeader Header { get; set; } = new();
    public List<SalesInvoiceLine> Lines { get; set; } = [];
}
