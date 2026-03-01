using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IPsRepo
{
    /// <summary>
    /// Ps_Select - Get list of penyesuaian stock
    /// </summary>
    Task<IEnumerable<PsHeader>> PsSelect(CancellationToken cancellationToken);
    
    /// <summary>
    /// Ps_Select - Get paged list of penyesuaian stock with filtering and sorting
    /// </summary>
    Task<PagedList<PsHeader>> PsSelect(PagedListRequest request, CancellationToken cancellationToken);
    
    /// <summary>
    /// Ps_SelectById - Get PS by RecId
    /// </summary>
    Task<PsHeader> PsSelectById(int id, CancellationToken cancellationToken);
    
    /// <summary>
    /// Ps_Insert - Create new penyesuaian stock (returns RecId and PsId)
    /// </summary>
    Task<(int RecId, string PsId)> PsInsert(PsHeader ps, CancellationToken cancellationToken);
    
    /// <summary>
    /// Ps_Update - Update existing penyesuaian stock
    /// </summary>
    Task PsUpdate(PsHeader ps, CancellationToken cancellationToken);
    
    /// <summary>
    /// Ps_Delete - Delete penyesuaian stock (and its lines)
    /// </summary>
    Task<string> PsDelete(int recId, string modifiedBy, CancellationToken cancellationToken);
    
    /// <summary>
    /// PsLine_SelectByPs - Get lines for a PS
    /// </summary>
    Task<IEnumerable<PsLine>> PsLineSelectByPs(int psRecId, CancellationToken cancellationToken);
    
    /// <summary>
    /// PsLine_SelectById - Get specific line
    /// </summary>
    Task<PsLine?> PsLineSelectById(int psLineId, CancellationToken cancellationToken);
    
    /// <summary>
    /// PsLine_Insert - Insert PS line (returns result code)
    /// </summary>
    Task<string> PsLineInsert(PsLine psLine, CancellationToken cancellationToken);
    
    /// <summary>
    /// PsLine_Update - Update PS line
    /// </summary>
    Task<string> PsLineUpdate(PsLine psLine, CancellationToken cancellationToken);
    
    /// <summary>
    /// PsLine_Delete - Delete PS line (returns result code)
    /// </summary>
    Task<string> PsLineDelete(int psLineId, string modifiedBy, CancellationToken cancellationToken);
}
