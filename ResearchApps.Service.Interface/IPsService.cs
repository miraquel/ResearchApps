using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IPsService
{
    /// <summary>
    /// Ps_Select - Get list of penyesuaian stock
    /// </summary>
    Task<ServiceResponse<IEnumerable<PsHeaderVm>>> PsSelect(CancellationToken cancellationToken);
    
    /// <summary>
    /// Ps_Select - Get paged list of penyesuaian stock with filtering and sorting
    /// </summary>
    Task<ServiceResponse<PagedListVm<PsHeaderVm>>> PsSelect(PagedListRequestVm request, CancellationToken cancellationToken);
    
    /// <summary>
    /// Ps_SelectById - Get PS by RecId
    /// </summary>
    Task<ServiceResponse<PsHeaderVm>> PsSelectById(int id, CancellationToken cancellationToken);
    
    /// <summary>
    /// Ps_Insert - Create new penyesuaian stock (returns RecId and PsId)
    /// </summary>
    Task<ServiceResponse<(int RecId, string PsId)>> PsInsert(PsVm ps, CancellationToken cancellationToken);
    
    /// <summary>
    /// Ps_Update - Update existing penyesuaian stock header
    /// </summary>
    Task<ServiceResponse> PsUpdate(PsHeaderVm psHeader, CancellationToken cancellationToken);
    
    /// <summary>
    /// Ps_Delete - Delete penyesuaian stock and its lines
    /// </summary>
    Task<ServiceResponse> PsDelete(int recId, CancellationToken cancellationToken);
    
    /// <summary>
    /// PsLine_SelectByPs - Get lines for a PS
    /// </summary>
    Task<ServiceResponse<IEnumerable<PsLineVm>>> PsLineSelectByPs(int psRecId, CancellationToken cancellationToken);
    
    /// <summary>
    /// PsLine_SelectById - Get specific line
    /// </summary>
    Task<ServiceResponse<PsLineVm>> PsLineSelectById(int psLineId, CancellationToken cancellationToken);
    
    /// <summary>
    /// PsLine_Insert - Insert PS line
    /// </summary>
    Task<ServiceResponse<string>> PsLineInsert(PsLineVm psLine, CancellationToken cancellationToken);
    
    /// <summary>
    /// PsLine_Update - Update PS line
    /// </summary>
    Task<ServiceResponse<string>> PsLineUpdate(PsLineVm psLine, CancellationToken cancellationToken);
    
    /// <summary>
    /// PsLine_Delete - Delete PS line
    /// </summary>
    Task<ServiceResponse<string>> PsLineDelete(int psLineId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Get composite Penyesuaian Stock with Header and Lines
    /// </summary>
    Task<ServiceResponse<PsVm>> GetPs(int recId, CancellationToken cancellationToken);
}
