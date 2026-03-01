using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IInventLockService
{
    /// <summary>
    /// Selects all inventory lock records for a specific year.
    /// </summary>
    Task<ServiceResponse<IEnumerable<InventLockVm>>> SelectByYearAsync(int year, CancellationToken cancellationToken);
    
    /// <summary>
    /// Closes inventory (locks records and runs inventory closing).
    /// </summary>
    Task<ServiceResponse> CloseAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Opens/unlocks an inventory lock record.
    /// </summary>
    Task<ServiceResponse> OpenAsync(InventLockActionVm action, CancellationToken cancellationToken);
    
    /// <summary>
    /// Runs the manual inventory closing process for a specific year and month.
    /// </summary>
    Task<ServiceResponse> RunClosingAsync(InventLockActionVm action, CancellationToken cancellationToken);
    
    /// <summary>
    /// Initializes inventory lock records for a year (creates 12 monthly records if they don't exist).
    /// </summary>
    Task<ServiceResponse> InitializeYearAsync(int year, CancellationToken cancellationToken);
}
