namespace ResearchApps.Domain;

/// <summary>
/// Composite BPB entity containing Header and Lines
/// </summary>
public class Bpb
{
    public BpbHeader Header { get; set; } = new();
    public IEnumerable<BpbLine> Lines { get; set; } = [];
}
