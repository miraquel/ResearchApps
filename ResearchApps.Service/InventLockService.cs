using System.Data;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class InventLockService : IInventLockService
{
    private readonly IInventLockRepo _inventLockRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<InventLockService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public InventLockService(
        IInventLockRepo inventLockRepo, 
        IDbTransaction dbTransaction, 
        UserClaimDto userClaimDto, 
        ILogger<InventLockService> logger)
    {
        _inventLockRepo = inventLockRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<InventLockVm>>> SelectByYearAsync(int year, CancellationToken cancellationToken)
    {
        LogRetrievingInventoryLocks(year);
        var inventLocks = await _inventLockRepo.SelectByYearAsync(year, cancellationToken);
        var result = _mapper.MapToVm(inventLocks);
        var inventLockVms = result as InventLockVm[] ?? result.ToArray();
        LogInventoryLocksRetrieved(inventLockVms.Count());
        return ServiceResponse<IEnumerable<InventLockVm>>.Success(inventLockVms, "Inventory locks retrieved successfully.");
    }

    public async Task<ServiceResponse> CloseAsync(CancellationToken cancellationToken)
    {
        LogClosingInventory(_userClaimDto.Username);
        
        await _inventLockRepo.CloseAsync(cancellationToken);
        _dbTransaction.Commit();
        
        LogInventoryClosed();
        return ServiceResponse.Success("Inventory closing process completed successfully.");
    }

    public async Task<ServiceResponse> OpenAsync(InventLockActionVm action, CancellationToken cancellationToken)
    {
        LogOpeningInventory(action.RecId, action.Year, action.Month, _userClaimDto.Username);
        
        await _inventLockRepo.OpenAsync(action.RecId, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        
        LogInventoryOpened(action.RecId, action.Year, action.Month);
        return ServiceResponse.Success($"Inventory for {GetMonthName(action.Month)} {action.Year} unlocked successfully.");
    }

    public async Task<ServiceResponse> RunClosingAsync(InventLockActionVm action, CancellationToken cancellationToken)
    {
        LogRunningClosingProcess(action.Year, action.Month, _userClaimDto.Username);
        
        await _inventLockRepo.RunClosingAsync(action.Year, action.Month, _userClaimDto.Username, cancellationToken);
        _dbTransaction.Commit();
        
        LogClosingProcessCompleted(action.Year, action.Month);
        return ServiceResponse.Success($"Inventory closing for {GetMonthName(action.Month)} {action.Year} completed successfully.");
    }

    public async Task<ServiceResponse> InitializeYearAsync(int year, CancellationToken cancellationToken)
    {
        LogInitializingYear(year, _userClaimDto.Username);
        
        // Check if records already exist for this year
        var existingRecords = await _inventLockRepo.SelectByYearAsync(year, cancellationToken);
        if (existingRecords.Any())
        {
            LogYearAlreadyInitialized(year);
            return ServiceResponse.Success($"Inventory lock records for {year} already exist.");
        }
        
        // Note: This would require an additional stored procedure to insert records
        // For now, we return a message indicating manual setup may be required
        LogYearInitializationRequired(year);
        return ServiceResponse.Success($"No inventory lock records found for {year}. Please create them manually or contact your administrator.");
    }

    private static string GetMonthName(int month)
    {
        return month >= 1 && month <= 12 
            ? new DateTime(2000, month, 1).ToString("MMMM") 
            : string.Empty;
    }

    // Source-generated logging methods
    [LoggerMessage(LogLevel.Information, "Retrieving inventory locks for year {Year}")]
    partial void LogRetrievingInventoryLocks(int year);

    [LoggerMessage(LogLevel.Information, "Retrieved {Count} inventory lock records")]
    partial void LogInventoryLocksRetrieved(int count);

    [LoggerMessage(LogLevel.Information, "Closing inventory by user: {Username}")]
    partial void LogClosingInventory(string username);

    [LoggerMessage(LogLevel.Information, "Inventory closing process completed")]
    partial void LogInventoryClosed();

    [LoggerMessage(LogLevel.Information, "Opening inventory RecId: {RecId} ({Year}-{Month}) by user: {Username}")]
    partial void LogOpeningInventory(int recId, int year, int month, string username);

    [LoggerMessage(LogLevel.Information, "Inventory RecId: {RecId} ({Year}-{Month}) opened successfully")]
    partial void LogInventoryOpened(int recId, int year, int month);

    [LoggerMessage(LogLevel.Information, "Running closing process for {Year}-{Month} by user: {Username}")]
    partial void LogRunningClosingProcess(int year, int month, string username);

    [LoggerMessage(LogLevel.Information, "Closing process completed for {Year}-{Month}")]
    partial void LogClosingProcessCompleted(int year, int month);

    [LoggerMessage(LogLevel.Information, "Initializing inventory lock records for year {Year} by user: {Username}")]
    partial void LogInitializingYear(int year, string username);

    [LoggerMessage(LogLevel.Information, "Inventory lock records for year {Year} already exist")]
    partial void LogYearAlreadyInitialized(int year);

    [LoggerMessage(LogLevel.Warning, "No inventory lock records found for year {Year}. Manual setup may be required.")]
    partial void LogYearInitializationRequired(int year);
}
