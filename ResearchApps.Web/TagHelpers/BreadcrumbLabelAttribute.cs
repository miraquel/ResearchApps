namespace ResearchApps.Web.TagHelpers;

/// <summary>
/// Declares a human-readable breadcrumb label for an MVC controller.
/// The BreadcrumbTagHelper reads this attribute to map URL segments to display names.
/// </summary>
/// <example>
/// [BreadcrumbLabel("Purchase Requisitions")]
/// public class PrsController : Controller { }
/// </example>
[AttributeUsage(AttributeTargets.Class)]
public sealed class BreadcrumbLabelAttribute(string label) : Attribute
{
    public string Label { get; } = label;
}
