namespace ResearchApps.Domain;

public class DashboardStatistics
{
    public int TotalPrs { get; set; }
    public int PendingPrs { get; set; }
    public int ApprovedPrs { get; set; }
    public int MyPendingApprovals { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal UsedBudget { get; set; }
    public decimal AvailableBudget { get; set; }
    public int UnreadNotifications { get; set; }
    public int ActiveItems { get; set; }
}

public class RecentPr
{
    public string PrId { get; set; } = string.Empty;
    public DateTime PrDate { get; set; }
    public string PrName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string PrStatusName { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string StatusBadge { get; set; } = string.Empty;
}

public class PendingApproval
{
    public string PrId { get; set; } = string.Empty;
    public DateTime PrDate { get; set; }
    public string PrName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string PrStatusName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string ApprovalNotes { get; set; } = string.Empty;
}

public class TopItem
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemTypeName { get; set; } = string.Empty;
    public string ItemDeptName { get; set; } = string.Empty;
    public int RequestCount { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
}

public class PrTrend
{
    public string MonthYear { get; set; } = string.Empty;
    public DateTime MonthDate { get; set; }
    public int PrCount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class BudgetByDepartment
{
    public string Department { get; set; } = string.Empty;
    public int PrCount { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal AvgAmount { get; set; }
}
