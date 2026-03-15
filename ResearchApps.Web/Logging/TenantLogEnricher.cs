using Finbuckle.MultiTenant.AspNetCore.Extensions;
using ResearchApps.Common.Tenant;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace ResearchApps.Web.Logging;

/// <summary>
/// Serilog enricher that adds TenantId and TenantIdentifier to every log entry.
/// </summary>
public class TenantLogEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantLogEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null) return;

        var tenantInfo = httpContext.GetMultiTenantContext<AppTenantInfo>()?.TenantInfo;
        if (tenantInfo is null) return;

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TenantId", tenantInfo.Id));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TenantIdentifier", tenantInfo.Identifier));
    }
}

public static class TenantLogEnricherExtensions
{
    public static LoggerConfiguration WithTenantInfo(this LoggerEnrichmentConfiguration enrichmentConfiguration, IServiceProvider serviceProvider)
    {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        return enrichmentConfiguration.With(new TenantLogEnricher(httpContextAccessor));
    }
}
