using ResearchApps.Domain;

namespace ResearchApps.Repo.Interface;

public interface IRepCustomRepo
{
    /// <summary>
    /// Calls [Rep_Tools] stored procedure
    /// </summary>
    Task<IEnumerable<RepTools>> RepTools(int year, int month, CancellationToken ct);

    /// <summary>
    /// Calls [Rep_ToolsAnalysis] stored procedure
    /// </summary>
    Task<IEnumerable<RepToolsAnalysis>> RepToolsAnalysis(int year, int month, CancellationToken ct);
}
