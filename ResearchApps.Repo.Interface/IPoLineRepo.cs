using ResearchApps.Domain;

namespace ResearchApps.Repo.Interface;

public interface IPoLineRepo
{
    Task<PoLine?> PoLineSelectById(int poLineId, CancellationToken ct);
    Task<IEnumerable<PoLine>> PoLineSelectByPo(int recId, CancellationToken ct);
    Task<string> PoLineInsert(PoLine poLine, CancellationToken ct);
    Task PoLineUpdate(PoLine poLine, CancellationToken ct);
    Task PoLineDelete(int poLineId, CancellationToken ct);
}
