namespace ResearchApps.Domain;

public class PoLine
{
    // Primary Keys
    public int PoLineId { get; set; }
    public string PoId { get; set; } = string.Empty;
    public int RecId { get; set; }
    public int PrLineId { get; set; }
    public string? PrLineName { get; set; }  // For display in edit modal

    // Business Fields
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public DateTime? DeliveryDate { get; set; }
    public decimal Qty { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Partial fulfillment (for validation during edit)
    public decimal? RequestedQty { get; set; }      // Original PR line requested quantity
    public decimal? OrderedQty { get; set; }        // Total ordered across all POs (excluding current line)
    public decimal? OutstandingQty { get; set; }    // Available to order (for validation)

    // Status (inherited from header)
    public int PoStatusId { get; set; }

    // Audit Fields
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
}
