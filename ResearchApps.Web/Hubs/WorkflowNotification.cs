namespace ResearchApps.Web.Hubs;

/// <summary>
/// Strongly-typed notification DTO for workflow events (PR, CO, etc.)
/// Replaces anonymous objects for compile-time safety and serialization consistency.
/// </summary>
public sealed record WorkflowNotification
{
    public required string Type { get; init; }
    public required string EntityId { get; init; }
    public required int RecId { get; init; }
    public string? ActionBy { get; init; }
    public string? NextApprover { get; init; }
    public string? Reason { get; init; }
    public bool? IsFullyApproved { get; init; }
    public required string Message { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Generic notification DTO for non-workflow notifications.
/// </summary>
public sealed record GenericNotification
{
    public required string Type { get; init; }
    public required string Message { get; init; }
    public object? Data { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
