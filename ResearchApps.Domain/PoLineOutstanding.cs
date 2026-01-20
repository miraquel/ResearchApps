namespace ResearchApps.Domain;

public class PoLineOutstanding
{
    // Header Info
    public string PoId { get; set; } = string.Empty;
    public int RecId { get; set; }
    public DateTime PoDate { get; set; }

    // Line Info
    public int PoLineId { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;

    // Quantities
    public decimal OrderQty { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal OutstandingQty { get; set; }

    // Pricing
    public decimal Price { get; set; }
    public decimal OutstandingAmount { get; set; }
}
