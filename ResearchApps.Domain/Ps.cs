namespace ResearchApps.Domain;

/// <summary>
/// Composite entity for Penyesuaian Stock with Header and Lines
/// </summary>
public class Ps
{
    public PsHeader Header { get; set; } = new();
    public List<PsLine> Lines { get; set; } = [];
}
