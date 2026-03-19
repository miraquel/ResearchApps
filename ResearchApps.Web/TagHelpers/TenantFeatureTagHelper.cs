using Microsoft.AspNetCore.Razor.TagHelpers;
using ResearchApps.Web.Services;

namespace ResearchApps.Web.TagHelpers;

/// <summary>
/// Conditionally renders inner content based on whether a feature is enabled
/// for the current tenant.
///
/// Usage:
///   &lt;tenant-feature name="PurchaseRequisitions"&gt;
///       &lt;li&gt;&lt;a href="/Prs"&gt;Purchase Requisitions&lt;/a&gt;&lt;/li&gt;
///   &lt;/tenant-feature&gt;
///
///   &lt;tenant-feature name="Budget" negate="true"&gt;
///       &lt;p&gt;Budget module not available for your plan.&lt;/p&gt;
///   &lt;/tenant-feature&gt;
/// </summary>
[HtmlTargetElement("tenant-feature", Attributes = "name")]
public class TenantFeatureTagHelper(ITenantFeatureService featureService) : TagHelper
{
    /// <summary>Feature key to check (use TenantFeatureConstants).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>When true, renders content only when the feature is DISABLED.</summary>
    public bool Negate { get; set; } = false;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null; // renders children without a wrapper element

        var isEnabled = featureService.IsEnabled(Name);
        var shouldRender = Negate ? !isEnabled : isEnabled;

        if (!shouldRender)
            output.SuppressOutput();
    }
}
