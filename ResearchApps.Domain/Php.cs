namespace ResearchApps.Domain;

/// <summary>
/// Composite entity for Php (Penerimaan Hasil Produksi) with Header and Lines
/// </summary>
public class Php
{
    public PhpHeader Header { get; set; } = new();
    public List<PhpLine> Lines { get; set; } = [];
}
