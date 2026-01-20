namespace ResearchApps.Web.Extensions;

/// <summary>
/// Extension methods for HttpRequest to support htmx and TomSelect detection
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Checks if the request is an htmx request by looking for the HX-Request header
    /// </summary>
    public static bool IsHtmxRequest(this HttpRequest request)
    {
        return request.Headers.ContainsKey("HX-Request");
    }

    /// <summary>
    /// Checks if the request is from TomSelect dropdown by looking for the X-TomSelect header
    /// </summary>
    public static bool IsTomSelectRequest(this HttpRequest request)
    {
        return request.Headers.ContainsKey("X-TomSelect");
    }

    /// <summary>
    /// Gets the htmx trigger element ID if present
    /// </summary>
    public static string? GetHtmxTrigger(this HttpRequest request)
    {
        return request.Headers.TryGetValue("HX-Trigger", out var value) ? value.ToString() : null;
    }

    /// <summary>
    /// Gets the htmx target element ID if present
    /// </summary>
    public static string? GetHtmxTarget(this HttpRequest request)
    {
        return request.Headers.TryGetValue("HX-Target", out var value) ? value.ToString() : null;
    }
}
