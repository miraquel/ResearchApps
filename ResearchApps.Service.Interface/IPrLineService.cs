using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IPrLineService
{
    // PrLine_Delete
    Task<ServiceResponse<string>> PrLineDelete(int id, CancellationToken cancellationToken);
    // PrLine_Insert
    Task<ServiceResponse<string>> PrLineInsert(PrLineVm prLine, CancellationToken cancellationToken);
    // PrLine_SelectById
    Task<ServiceResponse<PrLineVm>> PrLineSelectById(int id, CancellationToken cancellationToken);
    // PrLine_SelectByPr
    Task<ServiceResponse<IEnumerable<PrLineVm>>> PrLineSelectByPr(string prId, CancellationToken cancellationToken);
    // PrLine_Update
    Task<ServiceResponse<string>> PrLineUpdate(PrLineVm prLine, CancellationToken cancellationToken);
}