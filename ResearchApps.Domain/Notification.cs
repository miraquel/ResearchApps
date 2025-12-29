namespace ResearchApps.Domain;

public class Notification
{
    public int NotificationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string NotificationType { get; set; } = string.Empty; // PrSubmitted, PrApproved, PrRejected, PrRecalled, PendingApproval
    public string? RefId { get; set; } // Reference ID (e.g., PrId)
    public int? RefRecId { get; set; } // Reference Record ID (e.g., PR RecId)
    public bool IsRead { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ReadDate { get; set; }
}

