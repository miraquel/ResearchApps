namespace ResearchApps.Service.Vm.Common;

public class UserClaimDto
{
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string TenantId { get; init; } = string.Empty;
    public string TenantIdentifier { get; init; } = string.Empty;
}