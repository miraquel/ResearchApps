using System.ComponentModel.DataAnnotations;

namespace ResearchApps.Service.Vm;

public class NotificationVm
{
    [Display(Name = "Notification ID")]
    public int NotificationId { get; set; }
    
    [Display(Name = "User ID")]
    public string UserId { get; set; } = string.Empty;
    
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;
    
    [Display(Name = "Message")]
    public string Message { get; set; } = string.Empty;
    
    [Display(Name = "URL")]
    public string? Url { get; set; }
    
    [Display(Name = "Notification Type")]
    public string NotificationType { get; set; } = string.Empty;
    
    [Display(Name = "Reference ID")]
    public string? RefId { get; set; }
    
    [Display(Name = "Reference Record ID")]
    public int? RefRecId { get; set; }
    
    [Display(Name = "Is Read")]
    public bool IsRead { get; set; }
    
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
    
    [Display(Name = "Read Date")]
    public DateTime? ReadDate { get; set; }
    
    // Computed property for time ago display
    public string TimeAgo => GetTimeAgo(CreatedDate);
    
    private static string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.Now - dateTime;
        
        if (timeSpan.TotalMinutes < 1)
            return "Just now";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} min ago";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours > 1 ? "s" : "")} ago";
        if (timeSpan.TotalDays < 7)
            return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays > 1 ? "s" : "")} ago";
        
        return dateTime.ToString("MMM dd, yyyy");
    }
}

public class NotificationCountVm
{
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
}

