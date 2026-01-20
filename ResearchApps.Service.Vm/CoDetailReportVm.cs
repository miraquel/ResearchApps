namespace ResearchApps.Service.Vm;

public class CoDetailReportVm
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<CoDetailReportItemVm> Items { get; set; } = new();
}

public class CoDetailReportItemVm
{
    public string CoId { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string PoCustomer { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public DateTime? WantedDeliveryDate { get; set; }
    public string WantedDeliveryDateStr { get; set; } = string.Empty;
    public decimal QtyCo { get; set; }
    public decimal QtyDo { get; set; }
    public decimal QtyOs { get; set; }
}
