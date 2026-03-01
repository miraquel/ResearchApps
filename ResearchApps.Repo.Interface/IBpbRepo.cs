using ResearchApps.Domain;
using ResearchApps.Domain.Common;

namespace ResearchApps.Repo.Interface;

public interface IBpbRepo
{
    // Bpb_Select - Get paginated list of BPBs
    Task<PagedList<BpbHeader>> BpbSelect(PagedListRequest request, CancellationToken cancellationToken);
    
    // Bpb_Select - Get all BPBs for export (no pagination)
    Task<IEnumerable<BpbHeader>> BpbSelectForExport(PagedListRequest request, CancellationToken cancellationToken);
    
    // Bpb_SelectById - Get BPB by RecId
    Task<BpbHeader?> BpbSelectById(int id, CancellationToken cancellationToken);
    
    // Bpb_SelectByProd - Get BPBs by Production ID
    Task<IEnumerable<BpbHeader>> BpbSelectByProd(string prodId, CancellationToken cancellationToken);
    
    // Bpb_Insert - Create new BPB (returns RecId and BpbId)
    Task<(int RecId, string BpbId)> BpbInsert(BpbHeader bpb, CancellationToken cancellationToken);
    
    // Bpb_Update - Update existing BPB
    Task BpbUpdate(BpbHeader bpb, CancellationToken cancellationToken);
    
    // Bpb_Delete - Delete BPB (and its lines)
    Task BpbDelete(int recId, string modifiedBy, CancellationToken cancellationToken);
    
    // BpbLine_SelectByBpb - Get lines for a BPB
    Task<IEnumerable<BpbLine>> BpbLineSelectByBpb(int bpbRecId, CancellationToken cancellationToken);
    
    // BpbLine_SelectById - Get specific line
    Task<BpbLine?> BpbLineSelectById(int bpbLineId, CancellationToken cancellationToken);
    
    // BpbLine_Insert - Insert BPB line (returns result message)
    Task<string> BpbLineInsert(BpbLine bpbLine, CancellationToken cancellationToken);
    
    // BpbLine_Update - Update BPB line
    Task<string> BpbLineUpdate(BpbLine bpbLine, CancellationToken cancellationToken);
    
    // BpbLine_Delete - Delete BPB line (returns result message)
    Task<string> BpbLineDelete(int bpbLineId, string modifiedBy, CancellationToken cancellationToken);
    
    // Stock check for item/warehouse
    Task<(decimal OnHand, decimal BufferStock)> GetStockInfo(int itemId, int whId, CancellationToken cancellationToken);
}
