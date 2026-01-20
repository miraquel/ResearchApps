using ResearchApps.Domain;

namespace ResearchApps.Repo.Interface;

public interface IPrLineRepo
{
    // PrLine_Delete
    Task<string> PrLineDelete(int id, CancellationToken cancellationToken);
    // PrLine_Insert
    Task<string> PrLineInsert(PrLine prLine, CancellationToken cancellationToken);
    // PrLine_SelectById
    Task<PrLine> PrLineSelectById(int id, CancellationToken cancellationToken);
    // PrLine_SelectByPr
    Task<IEnumerable<PrLine>> PrLineSelectByPr(string prId, CancellationToken cancellationToken);
    // PrLine_SelectForPo
    Task<IEnumerable<PrLine>> PrLineSelectForPo(
        int poRecId, 
        int pageNumber, 
        int pageSize, 
        string? prId, 
        string? itemName, 
        DateTime? dateFrom, 
        CancellationToken cancellationToken);
    // PrLine_Update
    Task<string> PrLineUpdate(PrLine prLine, CancellationToken cancellationToken);
}