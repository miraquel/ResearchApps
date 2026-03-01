using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IBpbService
{
    // Bpb_Select - Get paginated list of BPBs
    Task<ServiceResponse<PagedListVm<BpbHeaderVm>>> BpbSelect(PagedListRequestVm request, CancellationToken cancellationToken);
    
    // Bpb_SelectById - Get BPB by RecId
    Task<ServiceResponse<BpbHeaderVm>> BpbSelectById(int id, CancellationToken cancellationToken);
    
    // Get full BPB with header and lines
    Task<ServiceResponse<BpbVm>> GetBpb(int recId, CancellationToken cancellationToken);
    
    // Get BPBs by Production ID
    Task<ServiceResponse<IEnumerable<BpbHeaderVm>>> BpbSelectByProd(string prodId, CancellationToken cancellationToken);
    
    // Bpb_Insert - Create new BPB (returns RecId)
    Task<ServiceResponse<int>> BpbInsert(BpbVm bpb, CancellationToken cancellationToken);
    
    // Bpb_Update - Update existing BPB header
    Task<ServiceResponse> BpbUpdate(BpbHeaderVm bpbHeader, CancellationToken cancellationToken);
    
    // Bpb_Delete - Delete BPB (and its lines)
    Task<ServiceResponse> BpbDelete(int recId, CancellationToken cancellationToken);
    
    // BpbLine_SelectByBpb - Get lines for a BPB
    Task<ServiceResponse<IEnumerable<BpbLineVm>>> BpbLineSelectByBpb(int bpbRecId, CancellationToken cancellationToken);
    
    // BpbLine_SelectById - Get specific line
    Task<ServiceResponse<BpbLineVm>> BpbLineSelectById(int bpbLineId, CancellationToken cancellationToken);
    
    // BpbLine_Insert - Insert BPB line
    Task<ServiceResponse<string>> BpbLineInsert(BpbLineVm bpbLine, CancellationToken cancellationToken);
    
    // BpbLine_Update - Update BPB line
    Task<ServiceResponse<string>> BpbLineUpdate(BpbLineVm bpbLine, CancellationToken cancellationToken);
    
    // BpbLine_Delete - Delete BPB line
    Task<ServiceResponse<string>> BpbLineDelete(int bpbLineId, CancellationToken cancellationToken);
    
    // Check stock availability
    Task<ServiceResponse<StockCheckVm>> CheckStock(int itemId, int whId, decimal requestedQty, CancellationToken cancellationToken);
}
