using ResearchApps.Domain;

namespace ResearchApps.Repo.Interface;

public interface IInventLockRepo
{
    /// <summary>
    /// Selects all inventory lock records for a specific year.
    /// Stored procedure: InventLock_Select
    /// </summary>
    Task<IEnumerable<InventLock>> SelectByYearAsync(int year, CancellationToken cancellationToken);
    
    /// <summary>
    /// Closes inventory for the previous month (locks records and runs inventory closing).
    /// This stored procedure only runs on day 5 of each month.
    /// Stored procedure: InventLock_Close
    /// </summary>
    Task CloseAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Opens/unlocks an inventory lock record.
    /// Stored procedure: InventLock_Open
    /// </summary>
    Task OpenAsync(int recId, string modifiedBy, CancellationToken cancellationToken);
    
    /// <summary>
    /// Runs the manual inventory closing process for a specific year and month.
    /// Stored procedure: Invent_Closing
    /// </summary>
    Task RunClosingAsync(int year, int month, string createdBy, CancellationToken cancellationToken);
}
