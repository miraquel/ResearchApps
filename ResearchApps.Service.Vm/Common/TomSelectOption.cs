namespace ResearchApps.Service.Vm.Common;

/// <summary>
/// TomSelect-compatible option format
/// Used for dropdown options in Alpine.js + TomSelect integration
/// </summary>
public class TomSelectOption
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
