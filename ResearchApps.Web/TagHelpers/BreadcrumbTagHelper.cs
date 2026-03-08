using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ResearchApps.Web.TagHelpers;

/// <summary>
/// A breadcrumb item used for explicit ViewData overrides.
/// Set ViewData["Breadcrumbs"] = new List&lt;BreadcrumbItem&gt; in any view
/// to bypass URL-based generation entirely.
/// </summary>
/// <param name="Label">The display text shown in the breadcrumb.</param>
/// <param name="Url">The link href. Null or empty means this is the active (last) item.</param>
public record BreadcrumbItem(string Label, string? Url = null);

/// <summary>
/// Targets &lt;ol breadcrumb&gt; and auto-generates breadcrumb &lt;li&gt; items.
///
/// Priority order:
///   1. ViewData["Breadcrumbs"] (List&lt;BreadcrumbItem&gt;) — explicit, URL-free override
///   2. URL path segments — label resolved from [BreadcrumbLabel] on the matching controller,
///      falling back to a Friendly() transformation; area and numeric segments are suppressed.
/// </summary>
[HtmlTargetElement("ol", Attributes = "breadcrumb")]
public class BreadcrumbTagHelper(IActionDescriptorCollectionProvider actionDescriptors) : TagHelper
{
    // Cached once for the lifetime of the app — built by reflecting [BreadcrumbLabel] attributes.
    private static Dictionary<string, string>? _labelMap;
    private static readonly Lock _lock = new();

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.RemoveAll("breadcrumb");

        // Priority 1: explicit ViewData override (URL-free)
        if (ViewContext.ViewData["Breadcrumbs"] is List<BreadcrumbItem> explicitCrumbs)
        {
            RenderCrumbs(output, explicitCrumbs);
            return;
        }

        // Priority 2: derive from URL path using controller-declared labels
        var area = ViewContext.RouteData.Values.TryGetValue("area", out var areaValue)
            ? areaValue?.ToString()
            : null;

        var path = ViewContext.HttpContext.Request.Path.Value ?? string.Empty;
        var segments = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);

        RenderCrumbs(output, BuildCrumbs(segments, area, GetLabelMap()));
    }

    /// <summary>
    /// Builds (or returns cached) a dictionary of controller name → breadcrumb label
    /// sourced from [BreadcrumbLabel] attributes on each controller class.
    /// </summary>
    private Dictionary<string, string> GetLabelMap()
    {
        if (_labelMap is not null) return _labelMap;

        lock (_lock)
        {
            if (_labelMap is not null) return _labelMap;

            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var descriptor in actionDescriptors.ActionDescriptors.Items)
            {
                if (descriptor is not ControllerActionDescriptor cad) continue;

                var attr = cad.ControllerTypeInfo.GetCustomAttribute<BreadcrumbLabelAttribute>();
                if (attr is not null)
                    map.TryAdd(cad.ControllerName, attr.Label);
            }

            _labelMap = map;
            return _labelMap;
        }
    }

    private static List<BreadcrumbItem> BuildCrumbs(string[] segments, string? area, Dictionary<string, string> labelMap)
    {
        var crumbs = new List<BreadcrumbItem>(segments.Length);
        var urlBuilder = new StringBuilder("/");

        foreach (var segment in segments)
        {
            urlBuilder.Append(segment);

            var isAreaSegment = string.Equals(segment, area, StringComparison.OrdinalIgnoreCase);
            var isNumeric = int.TryParse(segment, out _);

            if (!isAreaSegment && !isNumeric)
            {
                var label = labelMap.TryGetValue(segment, out var mapped) ? mapped : Friendly(segment);
                crumbs.Add(new BreadcrumbItem(label, urlBuilder.ToString()));
            }

            urlBuilder.Append('/');
        }

        return crumbs;
    }

    private static void RenderCrumbs(TagHelperOutput output, List<BreadcrumbItem> crumbs)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < crumbs.Count; i++)
        {
            var (label, href) = crumbs[i];
            var isLast = i == crumbs.Count - 1 || string.IsNullOrEmpty(href);

            if (isLast)
                sb.Append($"""<li class="breadcrumb-item active" aria-current="page">{label}</li>""");
            else
                sb.Append($"""<li class="breadcrumb-item"><a href="{href}">{label}</a></li>""");
        }

        output.Content.SetHtmlContent(sb.ToString());
    }

    private static string Friendly(string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;

        var replaced = s.Replace('-', ' ').Replace('_', ' ');
        var withSpaces = System.Text.RegularExpressions.Regex.Replace(replaced, "(?<!^)([A-Z])", " $1");
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(withSpaces.Trim());
    }
}
