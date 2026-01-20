# ResearchApps Coding Style Guide

**Generated from**: Customer Order, Delivery Order, Sales Invoice, Purchase Order modules  
**Purpose**: Ensure consistency across all modules in ResearchApps

---

## Table of Contents

1. [Naming Conventions](#1-naming-conventions)
2. [Domain Entities](#2-domain-entities)
3. [View Models](#3-view-models)
4. [Repository Layer](#4-repository-layer)
5. [Service Layer](#5-service-layer)
6. [Controller Layer](#6-controller-layer)
7. [Stored Procedures](#7-stored-procedures)
8. [Logging Patterns](#8-logging-patterns)
9. [Transaction Management](#9-transaction-management)
10. [Authorization](#10-authorization)

---

## 1. Naming Conventions

### 1.1 Module Abbreviations

Always use consistent 2-3 letter abbreviations for module names:

| Module | Abbreviation | Example |
|--------|--------------|---------|
| Customer Order | `Co` | `CoSelect`, `Co_Select` |
| Delivery Order | `Do` | `DoSelect`, `Do_Select` |
| Sales Invoice | `Si` | `SiSelect`, `Si_Select` |
| Purchase Order | `Po` | `PoSelect`, `Po_Select` |
| Purchase Request | `Pr` | `PrSelect`, `Pr_Select` |

### 1.2 Method Naming Patterns

**Repository/Service Methods:**
```csharp
// ✅ Correct patterns
Task<PagedList<Entity>> {Module}Select(PagedListRequest request, CancellationToken ct);
Task<Entity> {Module}SelectById(int id, CancellationToken ct);
Task<(int RecId, string {Module}Id)> {Module}Insert(Entity entity, CancellationToken ct);
Task<ServiceResponse> {Module}Update(Entity entity, CancellationToken ct);
Task<ServiceResponse> {Module}Delete(int recId, CancellationToken ct);

// Workflow operations
Task<ServiceResponse> {Module}SubmitById(int recId, string username, CancellationToken ct);
Task<ServiceResponse> {Module}RecallById(int recId, string username, CancellationToken ct);
Task<ServiceResponse> {Module}ApproveById(int recId, string username, string notes, CancellationToken ct);
Task<ServiceResponse> {Module}RejectById(int recId, string username, string notes, CancellationToken ct);

// Line operations (for header-line modules)
Task<IEnumerable<Line>> {Module}LineSelectBy{Module}(int headerRecId, CancellationToken ct);
Task<Line> {Module}LineSelectById(int lineId, CancellationToken ct);
Task<string> {Module}LineInsert(Line line, CancellationToken ct);
Task<ServiceResponse> {Module}LineUpdate(Line line, CancellationToken ct);
Task<ServiceResponse> {Module}LineDelete(int lineId, CancellationToken ct);

// Outstanding operations
Task<IEnumerable<Outstanding>> {Module}OsSelect(int relatedId, CancellationToken ct);
Task<IEnumerable<HeaderOutstanding>> {Module}HdOsSelect(int relatedId, CancellationToken ct);
```

**Controller Actions:**
```csharp
// ✅ MVC Actions
Index()                     // List view
List()                      // HTMX partial for table
Details(int id)             // Detail view
Create()                    // GET create form
Create(Vm vm)               // POST create
Edit(int id)                // GET edit form
Edit(Vm vm)                 // POST edit
Delete(int id)              // GET delete confirmation
DeleteConfirmed(int id)     // POST delete
```

**Stored Procedures:**
```sql
-- ✅ Pattern: {Module}_{Action}
Co_Select
Co_SelectById
Co_Insert
Co_Update
Co_Delete
Co_SubmitById
Co_ApproveById
Co_RejectById
Co_RecallById
Co_CloseByNo

-- Line operations
Co_LineSelect
Co_LineSelectByRecId
Co_LineInsert
Co_LineUpdate
Co_LineDelete

-- Outstanding
Co_OsSelect
Co_HdOsSelect
```

---

## 2. Domain Entities

### 2.1 Standard Fields

**Every header entity must have:**

```csharp
namespace ResearchApps.Domain;

public class {Module}Header
{
    // Primary Keys
    public string {Module}Id { get; set; } = string.Empty;  // Business key (auto-generated)
    public int RecId { get; set; }                           // Surrogate key
    
    // Business Fields
    public DateTime {Module}Date { get; set; }
    public string? {Module}DateStr { get; set; }             // For display (dd Mon yyyy)
    // ... other business fields
    
    // Workflow Fields (if applicable)
    public int {Module}StatusId { get; set; }
    public string? {Module}StatusName { get; set; }
    public int? WfTransId { get; set; }
    public string? CurrentApprover { get; set; }
    public int? CurrentIndex { get; set; }
    
    // Audit Fields (ALWAYS include)
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}
```

**Line entities:**

```csharp
public class {Module}Line
{
    // Primary Key
    public int {Module}LineId { get; set; }
    
    // Foreign Keys
    public int {Module}RecId { get; set; }
    public string {Module}Id { get; set; } = string.Empty;
    
    // Business Fields
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public decimal Qty { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public decimal QtyOs { get; set; }  // Outstanding quantity
    
    // Audit Fields
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}
```

### 2.2 Naming Rules

- Use `{Module}Id` for business key (string, auto-generated like "CO-2024-0001")
- Use `RecId` for surrogate key (int, identity)
- Use `{Module}Date` for date fields
- Use `{Module}StatusId` for status reference
- Join entity names for related entities (e.g., `CustomerName` not `Customer.Name`)
- Nullable fields use `?` annotation

---

## 3. View Models

### 3.1 Composite ViewModels

**Pattern for modules with header-line structure:**

```csharp
namespace ResearchApps.Service.Vm;

/// <summary>
/// Composite ViewModel for {Module Name} with Header, Lines, and Outstanding info
/// </summary>
public class {Module}Vm
{
    public {Module}HeaderVm Header { get; set; } = new();
    public List<{Module}LineVm> Lines { get; set; } = new();
    public IEnumerable<{Module}OutstandingVm> Outstanding { get; set; } = [];
    
    // Computed properties for UI logic
    public string Status => Header.{Module}StatusName ?? "Unknown";
    public bool CanSubmit => Header.{Module}StatusId == 0;     // Draft
    public bool CanRecall => Header.{Module}StatusId == 4;     // Pending
    public bool CanReject => Header.{Module}StatusId == 4;     // Pending
    public bool CanEdit => Header.{Module}StatusId == 0;       // Draft only
}
```

### 3.2 HeaderVm Pattern

```csharp
public class {Module}HeaderVm
{
    // Primary Keys
    [Display(Name = "{Module} Number")]
    public string {Module}Id { get; set; } = string.Empty;
    
    public int RecId { get; set; }
    
    // Business Fields with Validation
    [Required(ErrorMessage = "{Field} is required")]
    [Display(Name = "{Display Name}")]
    public DateTime {Module}Date { get; set; }
    
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }
    
    // Workflow Fields
    [Display(Name = "Status")]
    public int {Module}StatusId { get; set; }
    
    [Display(Name = "Status")]
    public string {Module}StatusName { get; set; } = string.Empty;
    
    public int? WfTransId { get; set; }
    
    [Display(Name = "Current Approver")]
    public string? CurrentApprover { get; set; }
    
    public int? CurrentIndex { get; set; }
    
    // Audit Fields
    [Display(Name = "Created By")]
    public string CreatedBy { get; set; } = string.Empty;
    
    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }
    
    [Display(Name = "Modified By")]
    public string ModifiedBy { get; set; } = string.Empty;
    
    [Display(Name = "Modified Date")]
    public DateTime ModifiedDate { get; set; }
}
```

### 3.3 LineVm Pattern

```csharp
public class {Module}LineVm
{
    public int {Module}LineId { get; set; }
    public int {Module}RecId { get; set; }
    
    [Required(ErrorMessage = "Item is required")]
    [Display(Name = "Item")]
    public int ItemId { get; set; }
    
    [Display(Name = "Item Name")]
    public string ItemName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    [Display(Name = "Quantity")]
    public decimal Qty { get; set; }
    
    [Required(ErrorMessage = "Price is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }
    
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }
    
    // Audit
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
```

### 3.4 Workflow Action ViewModel

```csharp
public class {Module}WorkflowActionVm
{
    public int RecId { get; set; }
    public string? {Module}Id { get; set; }
    public string? Notes { get; set; }
}
```

---

## 4. Repository Layer

### 4.1 Constructor Pattern

```csharp
public class {Module}Repo : I{Module}Repo
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _dbTransaction;

    public {Module}Repo(IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }
```

### 4.2 Select with Pagination

```csharp
public async Task<PagedList<{Module}Header>> {Module}Select(PagedListRequest request, CancellationToken cancellationToken)
{
    const string query = "{Module}_Select";
    var parameters = new DynamicParameters();
    
    // Process filters
    foreach (var filter in request.Filters)
    {
        if (filter.Value is { } strValue && !string.IsNullOrWhiteSpace(strValue))
        {
            switch (filter.Key)
            {
                // Date fields
                case "{Module}Date" or "CreatedDate" or "ModifiedDate" or "{Module}DateFrom" or "{Module}DateTo":
                {
                    if (DateTime.TryParse(strValue, out var dateValue))
                    {
                        parameters.Add($"@{filter.Key}", dateValue);
                    }
                    break;
                }
                // Numeric fields
                case "Total" or "SubTotal" or "Ppn" or "Amount":
                {
                    if (decimal.TryParse(strValue, out var decimalValue))
                    {
                        parameters.Add($"@{filter.Key}", decimalValue);
                    }
                    break;
                }
                // Operator fields
                case "TotalOperator" or "SubTotalOperator" or "PpnOperator" or "AmountOperator":
                {
                    parameters.Add($"@{filter.Key}", strValue);
                    break;
                }
                // Integer fields
                case "{Module}StatusId" or "RecId" or "CustomerId" or "SupplierId":
                {
                    if (int.TryParse(strValue, out var intValue))
                    {
                        parameters.Add($"@{filter.Key}", intValue);
                    }
                    break;
                }
                // Boolean fields
                case "IsPpn":
                {
                    if (bool.TryParse(strValue, out var boolValue))
                    {
                        parameters.Add($"@{filter.Key}", boolValue);
                    }
                    break;
                }
                // Default: string fields
                default:
                    parameters.Add($"@{filter.Key}", strValue);
                    break;
            }
        }
    }
    
    // Pagination & sorting
    parameters.Add("@PageNumber", request.PageNumber);
    parameters.Add("@PageSize", request.PageSize);
    parameters.Add("@SortOrder", request.IsSortAscending ? "ASC" : "DESC");
    parameters.Add("@SortColumn", string.IsNullOrEmpty(request.SortBy) ? "{Module}Id" : request.SortBy);

    // CRITICAL: Required for SQL Server arithmetic operations
    await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
    
    var command = new CommandDefinition(
        query,
        parameters,
        _dbTransaction,
        cancellationToken: cancellationToken,
        commandType: CommandType.StoredProcedure);

    var result = await _dbConnection.QueryMultipleAsync(command);
    var items = result.Read<{Module}Header>();
    var totalCount = result.ReadSingle<int>();
    
    return new PagedList<{Module}Header>(items, request.PageNumber, request.PageSize, totalCount);
}
```

### 4.3 Select By ID

```csharp
public async Task<{Module}Header> {Module}SelectById(int id, CancellationToken cancellationToken)
{
    const string query = "{Module}_SelectById";
    var parameters = new DynamicParameters();
    parameters.Add("@RecId", id);

    await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
    
    var command = new CommandDefinition(
        query,
        parameters,
        _dbTransaction,
        cancellationToken: cancellationToken,
        commandType: CommandType.StoredProcedure);

    var result = await _dbConnection.QuerySingleAsync<{Module}Header>(command);
    return result;
}
```

### 4.4 Insert Pattern

```csharp
public async Task<(int RecId, string {Module}Id)> {Module}Insert({Module}Header entity, CancellationToken cancellationToken)
{
    const string query = "{Module}_Insert";
    var parameters = new DynamicParameters();
    
    // Map all fields
    parameters.Add("@{Module}Date", entity.{Module}Date);
    parameters.Add("@CustomerId", entity.CustomerId);
    // ... other fields
    parameters.Add("@CreatedBy", entity.CreatedBy);
    
    // Output parameters
    parameters.Add("@RecId", dbType: DbType.Int32, direction: ParameterDirection.Output);
    parameters.Add("@{Module}Id", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

    await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
    
    var command = new CommandDefinition(
        query,
        parameters,
        _dbTransaction,
        cancellationToken: cancellationToken,
        commandType: CommandType.StoredProcedure);

    await _dbConnection.ExecuteAsync(command);
    
    var recId = parameters.Get<int>("@RecId");
    var moduleId = parameters.Get<string>("@{Module}Id");
    
    return (recId, moduleId);
}
```

### 4.5 Update Pattern

```csharp
public async Task {Module}Update({Module}Header entity, CancellationToken cancellationToken)
{
    const string query = "{Module}_Update";
    var parameters = new DynamicParameters();
    
    parameters.Add("@RecId", entity.RecId);
    parameters.Add("@{Module}Date", entity.{Module}Date);
    // ... all updateable fields
    parameters.Add("@ModifiedBy", entity.ModifiedBy);

    await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
    
    var command = new CommandDefinition(
        query,
        parameters,
        _dbTransaction,
        cancellationToken: cancellationToken,
        commandType: CommandType.StoredProcedure);

    await _dbConnection.ExecuteAsync(command);
}
```

### 4.6 Delete Pattern

```csharp
public async Task {Module}Delete(int recId, CancellationToken cancellationToken)
{
    const string query = "{Module}_Delete";
    var parameters = new DynamicParameters();
    parameters.Add("@RecId", recId);

    await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
    
    var command = new CommandDefinition(
        query,
        parameters,
        _dbTransaction,
        cancellationToken: cancellationToken,
        commandType: CommandType.StoredProcedure);

    await _dbConnection.ExecuteAsync(command);
}
```

### 4.7 Workflow Operations

```csharp
public async Task<{Module}Header> {Module}SubmitById(int recId, string username, CancellationToken cancellationToken)
{
    const string query = "{Module}_SubmitById";
    var parameters = new DynamicParameters();
    parameters.Add("@RecId", recId);
    parameters.Add("@Username", username);

    await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
    
    var command = new CommandDefinition(
        query,
        parameters,
        _dbTransaction,
        cancellationToken: cancellationToken,
        commandType: CommandType.StoredProcedure);

    var result = await _dbConnection.QuerySingleAsync<{Module}Header>(command);
    return result;
}

public async Task {Module}RecallById(int recId, string username, CancellationToken cancellationToken)
{
    const string query = "{Module}_RecallById";
    var parameters = new DynamicParameters();
    parameters.Add("@RecId", recId);
    parameters.Add("@Username", username);

    await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
    
    await _dbConnection.ExecuteAsync(
        query,
        parameters,
        _dbTransaction,
        commandType: CommandType.StoredProcedure);
}

public async Task {Module}ApproveById(int recId, string username, string notes, CancellationToken cancellationToken)
{
    const string query = "{Module}_ApproveById";
    var parameters = new DynamicParameters();
    parameters.Add("@RecId", recId);
    parameters.Add("@Username", username);
    parameters.Add("@Notes", notes);

    await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: _dbTransaction);
    
    await _dbConnection.ExecuteAsync(
        query,
        parameters,
        _dbTransaction,
        commandType: CommandType.StoredProcedure);
}
```

---

## 5. Service Layer

### 5.1 Constructor Pattern

```csharp
public partial class {Module}Service : I{Module}Service
{
    private readonly I{Module}Repo _{moduleLower}Repo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<{Module}Service> _logger;
    private readonly MapperlyMapper _mapper = new();  // NOT injected via DI

    public {Module}Service(
        I{Module}Repo {moduleLower}Repo,
        IDbTransaction dbTransaction,
        UserClaimDto userClaimDto,
        ILogger<{Module}Service> logger)
    {
        _{moduleLower}Repo = {moduleLower}Repo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }
```

### 5.2 Select with Pagination

```csharp
public async Task<ServiceResponse<PagedListVm<{Module}HeaderVm>>> {Module}Select(
    PagedListRequestVm request, 
    CancellationToken cancellationToken)
{
    LogRetrieving{Module}ListPagePageSize(request.PageNumber, request.PageSize);
    var entities = await _{moduleLower}Repo.{Module}Select(_mapper.MapToEntity(request), cancellationToken);
    LogRetrievedCount{Module}s(entities.TotalCount);
    return ServiceResponse<PagedListVm<{Module}HeaderVm>>.Success(
        _mapper.MapToVm(entities), 
        "{Module Name}s retrieved successfully.");
}
```

### 5.3 Select By ID

```csharp
public async Task<ServiceResponse<{Module}HeaderVm>> {Module}SelectById(
    int id, 
    CancellationToken cancellationToken)
{
    LogRetrieving{Module}ById(id);
    var entity = await _{moduleLower}Repo.{Module}SelectById(id, cancellationToken);
    return ServiceResponse<{Module}HeaderVm>.Success(
        _mapper.MapToVm(entity), 
        "{Module Name} retrieved successfully.");
}
```

### 5.4 Insert Pattern

```csharp
public async Task<ServiceResponse<(int RecId, string {Module}Id)>> {Module}Insert(
    {Module}Vm vm, 
    CancellationToken cancellationToken)
{
    LogCreatingNew{Module}ByUser(_userClaimDto.Username);
    
    var entity = _mapper.MapToEntity(vm);
    entity.Header.CreatedBy = _userClaimDto.Username;
    
    var result = await _{moduleLower}Repo.{Module}Insert(entity.Header, cancellationToken);

    // Insert lines if applicable
    foreach (var line in entity.Lines)
    {
        line.{Module}RecId = result.RecId;
        line.CreatedBy = _userClaimDto.Username;
        await _{moduleLower}Repo.{Module}LineInsert(line, cancellationToken);
    }
    
    _dbTransaction.Commit();  // ✅ MUST commit
    Log{Module}CreatedSuccessfully(result.RecId, result.{Module}Id);
    
    return ServiceResponse<(int RecId, string {Module}Id)>.Success(
        result, 
        "{Module Name} created successfully.", 
        StatusCodes.Status201Created);
}
```

### 5.5 Update Pattern

```csharp
public async Task<ServiceResponse> {Module}Update(
    {Module}HeaderVm vm, 
    CancellationToken cancellationToken)
{
    LogUpdating{Module}ByUser(vm.RecId, vm.{Module}Id, _userClaimDto.Username);
    
    var entity = _mapper.MapToEntity(vm);
    entity.ModifiedBy = _userClaimDto.Username;
    
    await _{moduleLower}Repo.{Module}Update(entity, cancellationToken);
    _dbTransaction.Commit();  // ✅ MUST commit
    
    Log{Module}UpdatedSuccessfully(vm.RecId);
    return ServiceResponse.Success("{Module Name} updated successfully.");
}
```

### 5.6 Delete Pattern

```csharp
public async Task<ServiceResponse> {Module}Delete(
    int recId, 
    CancellationToken cancellationToken)
{
    LogDeleting{Module}ByUser(recId, _userClaimDto.Username);
    await _{moduleLower}Repo.{Module}Delete(recId, cancellationToken);
    _dbTransaction.Commit();  // ✅ MUST commit
    Log{Module}DeletedSuccessfully(recId);
    return ServiceResponse.Success("{Module Name} deleted successfully.");
}
```

### 5.7 Workflow Operations

```csharp
public async Task<ServiceResponse> {Module}SubmitById(
    {Module}WorkflowActionVm action, 
    CancellationToken cancellationToken)
{
    LogSubmitting{Module}ByUser(action.RecId, _userClaimDto.Username);
    await _{moduleLower}Repo.{Module}SubmitById(action.RecId, _userClaimDto.Username, cancellationToken);
    _dbTransaction.Commit();
    Log{Module}SubmittedSuccessfully(action.RecId);
    return ServiceResponse.Success("{Module Name} submitted successfully.");
}

public async Task<ServiceResponse> {Module}RecallById(
    {Module}WorkflowActionVm action, 
    CancellationToken cancellationToken)
{
    LogRecalling{Module}ByUser(action.RecId, _userClaimDto.Username);
    await _{moduleLower}Repo.{Module}RecallById(action.RecId, _userClaimDto.Username, cancellationToken);
    _dbTransaction.Commit();
    Log{Module}RecalledSuccessfully(action.RecId);
    return ServiceResponse.Success("{Module Name} recalled successfully.");
}

public async Task<ServiceResponse> {Module}ApproveById(
    {Module}WorkflowActionVm action, 
    CancellationToken cancellationToken)
{
    LogApproving{Module}ByUser(action.RecId, _userClaimDto.Username);
    await _{moduleLower}Repo.{Module}ApproveById(
        action.RecId, 
        _userClaimDto.Username, 
        action.Notes ?? string.Empty, 
        cancellationToken);
    _dbTransaction.Commit();
    Log{Module}ApprovedSuccessfully(action.RecId);
    return ServiceResponse.Success("{Module Name} approved successfully.");
}

public async Task<ServiceResponse> {Module}RejectById(
    {Module}WorkflowActionVm action, 
    CancellationToken cancellationToken)
{
    LogRejecting{Module}ByUser(action.RecId, _userClaimDto.Username, action.Notes ?? string.Empty);
    await _{moduleLower}Repo.{Module}RejectById(
        action.RecId, 
        _userClaimDto.Username, 
        action.Notes ?? string.Empty, 
        cancellationToken);
    _dbTransaction.Commit();
    Log{Module}RejectedSuccessfully(action.RecId);
    return ServiceResponse.Success("{Module Name} rejected successfully.");
}
```

---

## 6. Controller Layer

### 6.1 Constructor Pattern

```csharp
[Authorize]
public class {Module}sController : Controller
{
    private readonly I{Module}Service _{moduleLower}Service;
    private readonly UserClaimDto _userClaimDto;  // If needed

    public {Module}sController(I{Module}Service {moduleLower}Service, UserClaimDto userClaimDto)
    {
        _{moduleLower}Service = {moduleLower}Service;
        _userClaimDto = userClaimDto;
    }
```

### 6.2 Index Action

```csharp
[Authorize(PermissionConstants.{Module}s.Index)]
public ActionResult Index()
{
    return View();
}
```

### 6.3 List Action (HTMX)

```csharp
[Authorize(PermissionConstants.{Module}s.Index)]
public async Task<IActionResult> List(
    [FromQuery] int page = 1,
    [FromQuery] string? sortBy = null,
    [FromQuery] bool sortAsc = true,
    [FromQuery(Name = "filters")] Dictionary<string, string>? filters = null,
    CancellationToken cancellationToken = default)
{
    var request = new PagedListRequestVm
    {
        PageNumber = page,
        PageSize = 10,
        SortBy = sortBy ?? string.Empty,
        IsSortAscending = sortAsc,
        Filters = filters ?? new Dictionary<string, string>()
    };

    var response = await _{moduleLower}Service.{Module}Select(request, cancellationToken);
    
    if (!response.IsSuccess || response.Data == null)
    {
        return PartialView("_Partials/_{Module}ListContainer", new PagedListVm<{Module}HeaderVm>());
    }

    var result = new PagedListVm<{Module}HeaderVm>
    {
        Items = response.Data.Items,
        PageNumber = response.Data.PageNumber,
        PageSize = response.Data.PageSize,
        TotalCount = response.Data.TotalCount
    };

    ViewBag.SortBy = sortBy;
    ViewBag.SortAsc = sortAsc;
    ViewBag.Filters = filters;

    return PartialView("_Partials/_{Module}ListContainer", result);
}
```

### 6.4 Details Action

```csharp
[Authorize(PermissionConstants.{Module}s.Details)]
public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
{
    var response = await _{moduleLower}Service.Get{Module}(id, cancellationToken);
    if (response is { IsSuccess: true }) 
        return View(response.Data);
    
    TempData["ErrorMessage"] = response.Message ?? "{Module Name} not found.";
    return RedirectToAction(nameof(Index));
}
```

### 6.5 Create Actions

```csharp
[Authorize(PermissionConstants.{Module}s.Create)]
public ActionResult Create()
{
    return View(new {Module}Vm());
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(PermissionConstants.{Module}s.Create)]
public async Task<IActionResult> Create(
    [FromForm] {Module}Vm collection, 
    CancellationToken cancellationToken)
{
    if (!ModelState.IsValid)
    {
        return View(collection);
    }

    var response = await _{moduleLower}Service.{Module}Insert(collection, cancellationToken);
    if (response.IsSuccess)
    {
        TempData["SuccessMessage"] = $"{Module Name} {response.Data.{Module}Id} created successfully.";
        return RedirectToAction("Edit", new { id = response.Data.RecId });
    }

    if (response.Message != null) 
        ModelState.AddModelError(string.Empty, response.Message);
    return View(collection);
}
```

### 6.6 Edit Actions

```csharp
[Authorize(PermissionConstants.{Module}s.Edit)]
public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
{
    var response = await _{moduleLower}Service.Get{Module}(id, cancellationToken);
    if (response is { IsSuccess: true })
    {
        // Only allow editing if status is Draft (0)
        if (response.Data == null || response.Data.Header.{Module}StatusId == 0) 
            return View(response.Data);
        
        TempData["ErrorMessage"] = "Only Draft {Module Name}s can be edited.";
        return RedirectToAction(nameof(Details), new { id });
    }
    TempData["ErrorMessage"] = response.Message ?? "{Module Name} not found.";
    return RedirectToAction(nameof(Index));
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(PermissionConstants.{Module}s.Edit)]
public async Task<IActionResult> Edit(
    [FromForm] {Module}HeaderVm collection, 
    CancellationToken cancellationToken)
{
    // Verify the entity is still in Draft status before allowing update
    var current = await _{moduleLower}Service.{Module}SelectById(collection.RecId, cancellationToken);
    
    if (current is { IsSuccess: true, Data: not null } && current.Data.{Module}StatusId != 0)
    {
        TempData["ErrorMessage"] = "Only Draft {Module Name}s can be edited.";
        return RedirectToAction(nameof(Details), new { id = collection.RecId });
    }
    
    if (!ModelState.IsValid)
    {
        var vm = await _{moduleLower}Service.Get{Module}(collection.RecId, cancellationToken);
        return View(vm.Data);
    }

    var response = await _{moduleLower}Service.{Module}Update(collection, cancellationToken);
    if (response.IsSuccess)
    {
        TempData["SuccessMessage"] = "{Module Name} updated successfully.";
        return RedirectToAction("Details", new { id = collection.RecId });
    }

    if (response.Message != null) 
        ModelState.AddModelError(string.Empty, response.Message);
    
    var viewModel = await _{moduleLower}Service.Get{Module}(collection.RecId, cancellationToken);
    return View(viewModel.Data);
}
```

### 6.7 Delete Actions

```csharp
[Authorize(PermissionConstants.{Module}s.Delete)]
public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
{
    var response = await _{moduleLower}Service.{Module}SelectById(id, cancellationToken);
    if (response is { IsSuccess: true }) 
        return View(response.Data);
    
    TempData["ErrorMessage"] = response.Message ?? "{Module Name} not found.";
    return RedirectToAction(nameof(Index));
}

[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
[Authorize(PermissionConstants.{Module}s.Delete)]
public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
{
    var response = await _{moduleLower}Service.{Module}Delete(id, cancellationToken);
    if (response.IsSuccess)
    {
        TempData["SuccessMessage"] = "{Module Name} deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    TempData["ErrorMessage"] = response.Message ?? "Failed to delete {Module Name}.";
    return RedirectToAction(nameof(Index));
}
```

### 6.8 Workflow Actions

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize]  // Custom authorization in action
public async Task<IActionResult> Submit(
    [FromForm] {Module}WorkflowActionVm action, 
    CancellationToken cancellationToken)
{
    // Get entity data before submit for creator verification
    var before = await _{moduleLower}Service.{Module}SelectById(action.RecId, cancellationToken);
    if (before.Data?.CreatedBy != User.Identity?.Name)
    {
        TempData["ErrorMessage"] = "You are not authorized to submit this {Module Name}.";
        return RedirectToAction(nameof(Details), new { id = action.RecId });
    }

    var response = await _{moduleLower}Service.{Module}SubmitById(action, cancellationToken);
    if (response.IsSuccess)
    {
        TempData["SuccessMessage"] = response.Message;
        return RedirectToAction(nameof(Details), new { id = action.RecId });
    }

    TempData["ErrorMessage"] = response.Message ?? "Failed to submit {Module Name}.";
    return RedirectToAction(nameof(Details), new { id = action.RecId });
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize]
public async Task<IActionResult> Recall(
    [FromForm] {Module}WorkflowActionVm action, 
    CancellationToken cancellationToken)
{
    var response = await _{moduleLower}Service.{Module}RecallById(action, cancellationToken);
    if (response.IsSuccess)
    {
        TempData["SuccessMessage"] = response.Message;
        return RedirectToAction(nameof(Details), new { id = action.RecId });
    }

    TempData["ErrorMessage"] = response.Message ?? "Failed to recall {Module Name}.";
    return RedirectToAction(nameof(Details), new { id = action.RecId });
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize]  // Check CurrentApprover in view
public async Task<IActionResult> Approve(
    [FromForm] {Module}WorkflowActionVm action, 
    CancellationToken cancellationToken)
{
    var response = await _{moduleLower}Service.{Module}ApproveById(action, cancellationToken);
    if (response.IsSuccess)
    {
        TempData["SuccessMessage"] = response.Message;
        return RedirectToAction(nameof(Details), new { id = action.RecId });
    }

    TempData["ErrorMessage"] = response.Message ?? "Failed to approve {Module Name}.";
    return RedirectToAction(nameof(Details), new { id = action.RecId });
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize]
public async Task<IActionResult> Reject(
    [FromForm] {Module}WorkflowActionVm action, 
    CancellationToken cancellationToken)
{
    var response = await _{moduleLower}Service.{Module}RejectById(action, cancellationToken);
    if (response.IsSuccess)
    {
        TempData["SuccessMessage"] = response.Message;
        return RedirectToAction(nameof(Details), new { id = action.RecId });
    }

    TempData["ErrorMessage"] = response.Message ?? "Failed to reject {Module Name}.";
    return RedirectToAction(nameof(Details), new { id = action.RecId });
}
```

---

## 7. Stored Procedures

### 7.1 Select with Pagination

```sql
CREATE PROCEDURE [dbo].[{Module}_Select]
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SortColumn NVARCHAR(50) = '{Module}Id',
    @SortOrder NVARCHAR(4) = 'DESC',
    @{Module}Id NVARCHAR(255) = NULL,
    @{Module}Date DATETIME = NULL,
    @{Module}DateFrom DATETIME = NULL,
    @{Module}DateTo DATETIME = NULL,
    @{Module}StatusId INT = NULL,
    @CreatedBy NVARCHAR(50) = NULL,
    @RecId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Temp table for filtering
    SELECT
        a.[{Module}Id],
        a.[{Module}Date],
        CONVERT(VARCHAR(11), a.[{Module}Date], 106) as [{Module}DateStr],
        a.[{Module}StatusId],
        s.[{Module}StatusName],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy],
        a.[RecId]
    INTO #FilteredData
    FROM [{Module}] a
    JOIN [{Module}Status] s ON s.{Module}StatusId = a.{Module}StatusId
    WHERE (@{Module}Id IS NULL OR a.{Module}Id LIKE '%' + @{Module}Id + '%')
      AND (@{Module}Date IS NULL OR a.{Module}Date = @{Module}Date)
      AND (@{Module}DateFrom IS NULL OR a.{Module}Date >= @{Module}DateFrom)
      AND (@{Module}DateTo IS NULL OR a.{Module}Date <= @{Module}DateTo)
      AND (@{Module}StatusId IS NULL OR a.{Module}StatusId = @{Module}StatusId)
      AND (@CreatedBy IS NULL OR a.CreatedBy LIKE '%' + @CreatedBy + '%')
      AND (@RecId IS NULL OR a.RecId = @RecId);

    -- Main result set with pagination
    SELECT *
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortOrder = 'ASC' THEN
            CASE @SortColumn
                WHEN '{Module}Id' THEN [{Module}Id]
                WHEN 'CreatedBy' THEN [CreatedBy]
            END
        END ASC,
        CASE WHEN @SortOrder = 'DESC' THEN
            CASE @SortColumn
                WHEN '{Module}Id' THEN [{Module}Id]
                WHEN 'CreatedBy' THEN [CreatedBy]
            END
        END DESC,
        CASE WHEN @SortOrder = 'ASC' THEN
            CASE @SortColumn
                WHEN '{Module}Date' THEN [{Module}Date]
                WHEN 'CreatedDate' THEN [CreatedDate]
                WHEN 'ModifiedDate' THEN [ModifiedDate]
            END
        END ASC,
        CASE WHEN @SortOrder = 'DESC' THEN
            CASE @SortColumn
                WHEN '{Module}Date' THEN [{Module}Date]
                WHEN 'CreatedDate' THEN [CreatedDate]
                WHEN 'ModifiedDate' THEN [ModifiedDate]
            END
        END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    -- Total count
    SELECT COUNT(*) FROM #FilteredData;

    DROP TABLE #FilteredData;
END
```

### 7.2 Select By ID

```sql
CREATE PROCEDURE [dbo].[{Module}_SelectById]
    @RecId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.[{Module}Id],
        a.[{Module}Date],
        CONVERT(VARCHAR(11), a.[{Module}Date], 106) as [{Module}DateStr],
        a.[{Module}StatusId],
        s.[{Module}StatusName],
        a.[WfTransId],
        w.[CurrentApprover],
        w.[CurrentIndex],
        a.[CreatedDate],
        a.[CreatedBy],
        a.[ModifiedDate],
        a.[ModifiedBy],
        a.[RecId]
    FROM [{Module}] a
    JOIN [{Module}Status] s ON s.{Module}StatusId = a.{Module}StatusId
    LEFT JOIN [WfTrans] w ON w.WfTransId = a.WfTransId
    WHERE a.RecId = @RecId;
END
```

### 7.3 Insert

```sql
CREATE PROCEDURE [dbo].[{Module}_Insert]
    @{Module}Date DATETIME,
    @{Module}StatusId INT = 0,  -- Default Draft
    @CreatedBy NVARCHAR(50),
    @RecId INT OUTPUT,
    @{Module}Id NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Generate ID
        DECLARE @Year NVARCHAR(4) = CAST(YEAR(@{Module}Date) AS NVARCHAR(4));
        DECLARE @MaxSeq INT;

        SELECT @MaxSeq = ISNULL(MAX(CAST(RIGHT({Module}Id, 4) AS INT)), 0)
        FROM [{Module}]
        WHERE {Module}Id LIKE '{MOD}-' + @Year + '-%';

        SET @MaxSeq = @MaxSeq + 1;
        SET @{Module}Id = '{MOD}-' + @Year + '-' + RIGHT('0000' + CAST(@MaxSeq AS NVARCHAR(4)), 4);

        -- Insert
        INSERT INTO [{Module}] (
            [{Module}Id],
            [{Module}Date],
            [{Module}StatusId],
            [CreatedDate],
            [CreatedBy],
            [ModifiedDate],
            [ModifiedBy]
        )
        VALUES (
            @{Module}Id,
            @{Module}Date,
            @{Module}StatusId,
            GETDATE(),
            @CreatedBy,
            GETDATE(),
            @CreatedBy
        );

        SET @RecId = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
```

### 7.4 Update

```sql
CREATE PROCEDURE [dbo].[{Module}_Update]
    @RecId INT,
    @{Module}Date DATETIME,
    @ModifiedBy NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [{Module}]
    SET
        [{Module}Date] = @{Module}Date,
        [ModifiedDate] = GETDATE(),
        [ModifiedBy] = @ModifiedBy
    WHERE [RecId] = @RecId;
END
```

### 7.5 Delete

```sql
CREATE PROCEDURE [dbo].[{Module}_Delete]
    @RecId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Delete lines first (if applicable)
        DELETE FROM [{Module}Line] WHERE {Module}RecId = @RecId;

        -- Delete header
        DELETE FROM [{Module}] WHERE RecId = @RecId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
```

### 7.6 Workflow Submit

```sql
CREATE PROCEDURE [dbo].[{Module}_SubmitById]
    @RecId INT,
    @Username NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @{Module}Id NVARCHAR(50);
        DECLARE @WfFormId INT = 1;  -- Workflow form ID for this module
        DECLARE @WfTransId INT;

        SELECT @{Module}Id = {Module}Id FROM [{Module}] WHERE RecId = @RecId;

        -- Create workflow transaction
        EXEC Wf_InitiateTrans
            @RefId = @{Module}Id,
            @WfFormId = @WfFormId,
            @Username = @Username,
            @WfTransId = @WfTransId OUTPUT;

        -- Update entity with workflow info
        UPDATE [{Module}]
        SET
            [{Module}StatusId] = 4,  -- Pending
            [WfTransId] = @WfTransId,
            [ModifiedDate] = GETDATE(),
            [ModifiedBy] = @Username
        WHERE [RecId] = @RecId;

        -- Return updated entity
        SELECT
            a.*,
            w.CurrentApprover,
            w.CurrentIndex
        FROM [{Module}] a
        LEFT JOIN WfTrans w ON w.WfTransId = a.WfTransId
        WHERE a.RecId = @RecId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
```

---

## 8. Logging Patterns

### 8.1 Service Logging Setup

```csharp
public partial class {Module}Service : I{Module}Service
{
    // ... constructor

    #region Logging

    [LoggerMessage(LogLevel.Information, "Retrieving {Module} list - Page: {page}, PageSize: {pageSize}")]
    partial void LogRetrieving{Module}ListPagePageSize(int page, int pageSize);

    [LoggerMessage(LogLevel.Information, "Retrieved {count} {Module}s")]
    partial void LogRetrievedCount{Module}s(int count);

    [LoggerMessage(LogLevel.Information, "Retrieving {Module} by Id: {id}")]
    partial void LogRetrieving{Module}ById(int id);

    [LoggerMessage(LogLevel.Information, "Creating new {Module} by user: {username}")]
    partial void LogCreatingNew{Module}ByUser(string username);

    [LoggerMessage(LogLevel.Information, "{Module} created successfully - RecId: {recId}, {Module}Id: {moduleId}")]
    partial void Log{Module}CreatedSuccessfully(int recId, string moduleId);

    [LoggerMessage(LogLevel.Information, "Updating {Module} {recId} ({moduleId}) by user: {username}")]
    partial void LogUpdating{Module}ByUser(int recId, string moduleId, string username);

    [LoggerMessage(LogLevel.Information, "{Module} {recId} updated successfully")]
    partial void Log{Module}UpdatedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Deleting {Module} {recId} by user: {username}")]
    partial void LogDeleting{Module}ByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "{Module} {recId} deleted successfully")]
    partial void Log{Module}DeletedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Submitting {Module} {recId} by user: {username}")]
    partial void LogSubmitting{Module}ByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "{Module} {recId} submitted successfully")]
    partial void Log{Module}SubmittedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Recalling {Module} {recId} by user: {username}")]
    partial void LogRecalling{Module}ByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "{Module} {recId} recalled successfully")]
    partial void Log{Module}RecalledSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Approving {Module} {recId} by user: {username}")]
    partial void LogApproving{Module}ByUser(int recId, string username);

    [LoggerMessage(LogLevel.Information, "{Module} {recId} approved successfully")]
    partial void Log{Module}ApprovedSuccessfully(int recId);

    [LoggerMessage(LogLevel.Information, "Rejecting {Module} {recId} by user: {username}, Notes: {notes}")]
    partial void LogRejecting{Module}ByUser(int recId, string username, string notes);

    [LoggerMessage(LogLevel.Information, "{Module} {recId} rejected successfully")]
    partial void Log{Module}RejectedSuccessfully(int recId);

    #endregion
}
```

---

## 9. Transaction Management

### 9.1 Critical Rules

1. **Always commit after mutations**:
```csharp
await _repo.InsertAsync(entity, ct);
_dbTransaction.Commit();  // ✅ REQUIRED
```

2. **Transaction is scoped/injected** - Do NOT create new transactions:
```csharp
// ✅ Correct
private readonly IDbTransaction _dbTransaction;  // Injected

// ❌ Wrong
using var transaction = _connection.BeginTransaction();
```

3. **Commit in service layer**, not repository:
```csharp
// ✅ Service layer commits
public async Task<ServiceResponse> Insert(Vm vm, CancellationToken ct)
{
    await _repo.Insert(entity, ct);
    _dbTransaction.Commit();  // ✅ Service commits
    return ServiceResponse.Success();
}

// ❌ Repository should NOT commit
public async Task Insert(Entity entity, CancellationToken ct)
{
    await _dbConnection.ExecuteAsync(...);
    // ❌ No commit here
}
```

4. **Read operations do NOT need commits**:
```csharp
// ✅ No commit needed for reads
public async Task<ServiceResponse<Vm>> SelectById(int id, CancellationToken ct)
{
    var entity = await _repo.SelectById(id, ct);
    return ServiceResponse<Vm>.Success(_mapper.MapToVm(entity));
    // No commit needed
}
```

---

## 10. Authorization

### 10.1 Permission Constants

```csharp
// In PermissionConstants.cs
public static class {Module}s
{
    public const string Index = "permissions.{moduleLower}s.index";
    public const string Details = "permissions.{moduleLower}s.details";
    public const string Create = "permissions.{moduleLower}s.create";
    public const string Edit = "permissions.{moduleLower}s.edit";
    public const string Delete = "permissions.{moduleLower}s.delete";
}
```

### 10.2 Controller Authorization

```csharp
[Authorize]  // Class level - all actions require auth
public class {Module}sController : Controller
{
    [Authorize(PermissionConstants.{Module}s.Index)]
    public ActionResult Index() { }

    [Authorize(PermissionConstants.{Module}s.Create)]
    public ActionResult Create() { }
}
```

### 10.3 Workflow Authorization

**In Views (Razor):**
```cshtml
@* ✅ Workflow buttons - check CurrentApprover *@
@if (Model.Header.{Module}StatusId == 4 && Model.Header.CurrentApprover == User.Identity.Name)
{
    <button type="submit" asp-action="Approve">Approve</button>
    <button type="submit" asp-action="Reject">Reject</button>
}

@* ✅ Submit button - check creator *@
@if (Model.Header.{Module}StatusId == 0 && Model.Header.CreatedBy == User.Identity.Name)
{
    <button type="submit" asp-action="Submit">Submit</button>
}

@* ❌ Wrong for workflow *@
@if (User.HasClaim("permission", PermissionConstants.{Module}s.Approve))
{
    @* Wrong - don't use permissions for workflow actions *@
}
```

**In Controller:**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize]  // ✅ Basic auth only, check CurrentApprover in action
public async Task<IActionResult> Approve([FromForm] {Module}WorkflowActionVm action, CancellationToken ct)
{
    var entity = await _{moduleLower}Service.{Module}SelectById(action.RecId, ct);
    
    // ✅ Check CurrentApprover
    if (entity.Data?.CurrentApprover != User.Identity?.Name)
    {
        TempData["ErrorMessage"] = "You are not authorized to approve this {Module}.";
        return RedirectToAction(nameof(Details), new { id = action.RecId });
    }
    
    var response = await _{moduleLower}Service.{Module}ApproveById(action, ct);
    // ...
}
```

---

## Summary Checklist

When creating a new module, ensure:

- [ ] Consistent abbreviation (2-3 letters)
- [ ] Domain entities follow standard field pattern
- [ ] View models include all validation attributes
- [ ] Repository methods follow naming convention
- [ ] Service methods use `ServiceResponse<T>` for data operations
- [ ] Transaction commits after all mutations
- [ ] Logging uses source generators (`LoggerMessage`)
- [ ] Controller actions follow Index/List/Details/Create/Edit/Delete pattern
- [ ] Stored procedures match naming convention
- [ ] Authorization checks `CurrentApprover` for workflow actions
- [ ] `SET ARITHABORT ON` before all Dapper calls
- [ ] `MapperlyMapper` instantiated, not injected

---

## 11. CSHTML Views (Razor)

### 11.1 View Structure & Layout

**Standard View Pattern:**

```cshtml
@model ResearchApps.Service.Vm.{Module}Vm

@{
    ViewBag.Title = "{Module Name}";
    ViewBag.pTitle = "{Page Title}";
    Layout = "_Layout";
    ViewBag.CurrentApprover = Model.Header.CurrentApprover; // For workflow modules
}

<div x-data="{moduleComponent}()" x-init="init()">
    @Html.AntiForgeryToken()
    
    <!-- Page content here -->
</div>

@section scripts {
    <script src="~/js/{module-folder}/{module}-{page}.js"></script>
}
```

### 11.2 Index View Pattern (List Page with HTMX)

**Index.cshtml** - Main page with card layout and HTMX container:

```cshtml
@{
    ViewBag.Title = "{Module Name}s";
    ViewBag.pTitle = "{Module Name}s";
    Layout = "_Layout";
}

@Html.AntiForgeryToken()

<div x-data="{module}Index()" x-init="init()">
    @* List container card *@
    <div class="card">
        <div class="card-header">
            <div class="row align-items-center">
                <div class="col-md-6">
                    <div class="d-flex gap-2">
                        @* Refresh button *@
                        <button type="button" class="btn btn-soft-secondary" @@click="fetchList()" title="Refresh">
                            <i class="ri-refresh-line" :class="{ 'spin': isLoading }"></i>
                        </button>
                        
                        @* Clear all filters button *@
                        <button type="button" class="btn btn-soft-warning" @@click="clearFilters()" title="Clear All Filters">
                            <i class="ri-filter-off-line me-1"></i> Clear Filters
                        </button>

                        @* Loading indicator *@
                        <div x-show.important="isLoading" class="d-flex align-items-center text-muted" x-cloak>
                            <div class="spinner-border spinner-border-sm text-primary me-2" role="status">
                                <span class="visually-hidden">Loading...</span>
                            </div>
                            <small>Loading...</small>
                        </div>
                    </div>
                </div>
                <div class="col-md-6 text-end">
                    @* Export to Excel button (optional) *@
                    <button type="button" class="btn btn-success me-2" @@click="exportToExcel()" :disabled="isExporting">
                        <span x-show="!isExporting">
                            <i class="ri-file-excel-2-line align-bottom me-1"></i> Export to Excel
                        </span>
                        <span x-show="isExporting" x-cloak>
                            <span class="spinner-border spinner-border-sm me-1" role="status"></span>
                            Exporting...
                        </span>
                    </button>
                    
                    @* Create button *@
                    <a href="/{Module}s/Create" class="btn btn-primary">
                        <i class="ri-add-line align-bottom me-1"></i> Create
                    </a>
                </div>
            </div>
        </div>
        <div class="card-body">
            @* HTMX container - loads _Partials/_{Module}ListContainer.cshtml *@
            <div id="{module}-list-container"
                 hx-get="/{Module}s/List"
                 hx-trigger="load"
                 :hx-vals="JSON.stringify({sortBy: sortBy, sortAsc: sortAsc})">
                @* Initial loading placeholder *@
                <div class="text-center py-5 row justify-content-center">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                    <span class="mt-3 text-muted">Loading...</span>
                </div>
            </div>
        </div>
    </div>
</div>

@section scripts {
    <script src="~/js/{module}s/{module}-index.js"></script>
}
```

**Controller List Action** - Returns partial view with paged data:

```csharp
// GET: {Module}s/List (HTMX partial)
[Authorize(PermissionConstants.{Module}s.Index)]
public async Task<IActionResult> List(
    [FromQuery] int page = 1,
    [FromQuery] string? sortBy = null,
    [FromQuery] bool sortAsc = true,
    [FromQuery(Name = "filters")] Dictionary<string, string>? filters = null,
    CancellationToken cancellationToken = default)
{
    var request = new PagedListRequestVm
    {
        PageNumber = page,
        PageSize = 10,
        SortBy = sortBy ?? string.Empty,
        IsSortAscending = sortAsc,
        Filters = filters ?? new Dictionary<string, string>()
    };

    var response = await _{module}Service.{Module}Select(request, cancellationToken);
    
    if (!response.IsSuccess || response.Data == null)
    {
        return PartialView("_Partials/_{Module}ListContainer", new PagedListVm<{Module}Vm>());
    }

    var result = new PagedListVm<{Module}Vm>
    {
        Items = response.Data.Items,
        PageNumber = response.Data.PageNumber,
        PageSize = response.Data.PageSize,
        TotalCount = response.Data.TotalCount
    };

    ViewBag.SortBy = sortBy;
    ViewBag.SortAsc = sortAsc;
    ViewBag.Filters = filters;

    return PartialView("_Partials/_{Module}ListContainer", result);
}
```

**_Partials/_{Module}ListContainer.cshtml** - Table structure with sortable headers and filter footer:

```cshtml
@model ResearchApps.Service.Vm.Common.PagedListVm<ResearchApps.Service.Vm.{Module}Vm>

<table class="table table-bordered table-hover align-middle mb-0" style="width: 100%;">
    <thead class="table-light">
    <tr>
        @* Sortable header columns with Alpine.js click handlers *@
        <th class="sortable-header" style="width: 140px; cursor: pointer;" 
            @@click="sort('{Module}Id')" data-column="{Module}Id">
            <div class="d-flex align-items-center justify-content-center">
                <span>{Module} ID</span>
                <template x-if="sortBy === '{Module}Id'">
                    <i :class="sortAsc ? 'ri-arrow-up-s-line' : 'ri-arrow-down-s-line'" class="ms-1"></i>
                </template>
            </div>
        </th>
        <th class="sortable-header" style="min-width: 250px; cursor: pointer;"
            @@click="sort('{Field}Name')" data-column="{Field}Name">
            <div class="d-flex align-items-center">
                <span>{Field Label}</span>
                <template x-if="sortBy === '{Field}Name'">
                    <i :class="sortAsc ? 'ri-arrow-up-s-line' : 'ri-arrow-down-s-line'" class="ms-1"></i>
                </template>
            </div>
        </th>
        @* Add more sortable columns as needed *@
        <th class="text-center" style="width: 80px;">Actions</th>
    </tr>
    </thead>
    <tbody id="{module}-table-body">
        @* Embed list partial for table rows *@
        @await Html.PartialAsync("_Partials/_{Module}ListPartial", Model)
    </tbody>
    <tfoot class="table-light">
    <tr>
        @* Filter inputs for each column *@
        <td>
            <input type="text" class="form-control form-control-sm" placeholder="Search ID..."
                   x-model="filters.{Module}Id" @@input.debounce.500ms="fetchList()">
        </td>
        <td>
            <input type="text" class="form-control form-control-sm" placeholder="Search name..."
                   x-model="filters.{Field}Name" @@input.debounce.500ms="fetchList()">
        </td>
        @* Add more filter inputs as needed *@
        <td class="text-center">
            <button type="button" class="btn btn-sm btn-outline-secondary w-100" 
                    @@click="clearFilters()" title="Clear All Filters">
                <i class="ri-filter-off-line"></i>
            </button>
        </td>
    </tr>
    </tfoot>
</table>

<div class="mt-3">
    @* Embed pagination partial *@
    @await Html.PartialAsync("_Partials/_{Module}PaginationPartial", Model)
</div>
```

**_Partials/_{Module}ListPartial.cshtml** - Table rows (tbody content):

```cshtml
@model ResearchApps.Service.Vm.Common.PagedListVm<ResearchApps.Service.Vm.{Module}Vm>

@if (Model.Items.Any())
{
    foreach (var item in Model.Items)
    {
        <tr>
            <td class="text-center fw-semibold">@item.{Module}Id</td>
            <td>@item.{Field}Name</td>
            @* Add more data columns *@
            <td class="text-center">
                @* Actions dropdown *@
                <div class="dropdown d-inline-block">
                    <button class="btn btn-soft-secondary btn-sm dropdown" type="button" 
                            data-bs-toggle="dropdown" aria-expanded="false">
                        <i class="ri-more-fill align-middle"></i>
                    </button>
                    <ul class="dropdown-menu dropdown-menu-end">
                        <li>
                            <a href="/{Module}s/Details/@item.RecId" class="dropdown-item">
                                <i class="ri-eye-fill align-bottom me-2 text-muted"></i> View
                            </a>
                        </li>
                        <li>
                            <a href="/{Module}s/Edit/@item.RecId" class="dropdown-item">
                                <i class="ri-pencil-fill align-bottom me-2 text-muted"></i> Edit
                            </a>
                        </li>
                        <li>
                            <a href="/{Module}s/Delete/@item.RecId" class="dropdown-item">
                                <i class="ri-delete-bin-fill align-bottom me-2 text-muted"></i> Delete
                            </a>
                        </li>
                    </ul>
                </div>
            </td>
        </tr>
    }
}
else
{
    <tr>
        <td colspan="9" class="text-center text-muted py-4">
            <i class="ri-inbox-line fs-2"></i>
            <p class="mb-0 mt-2">No {module}s found</p>
        </td>
    </tr>
}
```

**_Partials/_{Module}PaginationPartial.cshtml** - Pagination controls:

```cshtml
@model ResearchApps.Service.Vm.Common.PagedListVm<ResearchApps.Service.Vm.{Module}Vm>

@{
    var currentPage = Model.PageNumber;
    var totalPages = Model.TotalPages;
    var hasNextPage = Model.HasNextPage;
    var hasPrevPage = Model.HasPreviousPage;
    var startRecord = Model.TotalCount > 0 ? (Model.PageNumber - 1) * Model.PageSize + 1 : 0;
    var endRecord = Math.Min(Model.PageNumber * Model.PageSize, Model.TotalCount);
}

<div class="d-flex justify-content-between align-items-center">
    <div class="text-muted">
        Showing @startRecord to @endRecord of @Model.TotalCount entries
    </div>
    <nav aria-label="Page navigation">
        <ul class="pagination pagination-sm mb-0">
            <li class="page-item @(hasPrevPage ? "" : "disabled")">
                <a class="page-link" href="javascript:void(0);"
                   @@click="fetchList(@(currentPage - 1))"
                   @(hasPrevPage ? "" : "aria-disabled=true")>
                    <i class="ri-arrow-left-s-line"></i>
                </a>
            </li>
            @for (var i = Math.Max(1, currentPage - 2); i <= Math.Min(totalPages, currentPage + 2); i++)
            {
                <li class="page-item @(i == currentPage ? "active" : "")">
                    <a class="page-link" href="javascript:void(0);"
                       @@click="fetchList(@i)">
                        @i
                    </a>
                </li>
            }
            <li class="page-item @(hasNextPage ? "" : "disabled")">
                <a class="page-link" href="javascript:void(0);"
                   @@click="fetchList(@(currentPage + 1))"
                   @(hasNextPage ? "" : "aria-disabled=true")>
                    <i class="ri-arrow-right-s-line"></i>
                </a>
            </li>
        </ul>
    </nav>
</div>
```

### 11.3 Details View Pattern

```cshtml
@using ResearchApps.Common.Constants
@model ResearchApps.Service.Vm.{Module}Vm

@{
    ViewBag.Title = "{Module Name} Details";
    ViewBag.pTitle = $"{Module Name} {Model.Header.{Module}Id}";
    Layout = "_Layout";
    ViewBag.CurrentApprover = Model.Header.CurrentApprover;
}

<div x-data="{module}Details('@Model.Header.{Module}Id', @Model.Header.RecId)" x-init="init()">
    @Html.AntiForgeryToken()
    
    <div id="{module}Details" class="card">
        <div class="card-header">
            <div class="row">
                <div class="col d-flex justify-content-between align-items-center">
                    <!-- Tabs -->
                    <ul class="nav nav-tabs-custom rounded card-header-tabs border-bottom-0" role="tablist">
                        <li class="nav-item" role="presentation">
                            <a class="nav-link active" data-bs-toggle="tab" href="#overview">Overview</a>
                        </li>
                        <li class="nav-item" role="presentation">
                            <a class="nav-link" data-bs-toggle="tab" href="#lines">Lines</a>
                        </li>
                        <li class="nav-item" role="presentation">
                            <a class="nav-link" data-bs-toggle="tab" href="#workflow">Workflow</a>
                        </li>
                        <li class="nav-item" role="presentation">
                            <a class="nav-link" data-bs-toggle="tab" href="#log">Log</a>
                        </li>
                    </ul>
                    
                    <!-- Action Buttons -->
                    <div class="position-absolute top-0 end-0">
                        <span x-show="isLoading" x-cloak class="spinner-border spinner-border-sm text-primary"></span>
                        
                        @await Html.PartialAsync("_Partials/_WorkflowButtons", Model.Header)
                        
                        @if (Model.Header.{Module}StatusId == {Module}StatusConstants.Draft && User.Identity?.Name == Model.Header.CreatedBy)
                        {
                            <a href="/{Module}s/Edit/@Model.Header.RecId" class="btn btn-sm btn-success">
                                <i class="ri-edit-line"></i> Edit
                            </a>
                        }
                        
                        @if (Model.Header.{Module}StatusId == {Module}StatusConstants.Draft && User.Identity?.Name == Model.Header.CreatedBy)
                        {
                            <a href="/{Module}s/Delete/@Model.Header.RecId" class="btn btn-sm btn-danger">
                                <i class="ri-delete-bin-line"></i> Delete
                            </a>
                        }
                    </div>
                </div>
            </div>
        </div>
        <div class="card-body">
            <div class="tab-content">
                <!-- Overview Tab -->
                <div class="tab-pane active" id="overview" role="tabpanel">
                    <div class="row">
                        <div class="col-md-6">
                            <table class="table table-borderless">
                                <tr>
                                    <td class="fw-bold" style="width: 150px;">{Module} No</td>
                                    <td>@Model.Header.{Module}Id</td>
                                </tr>
                                <!-- More fields... -->
                            </table>
                        </div>
                        <div class="col-md-6">
                            <!-- Right column fields -->
                        </div>
                    </div>
                </div>
                
                <!-- Lines Tab -->
                <div class="tab-pane" id="lines" role="tabpanel">
                    <div class="table-responsive">
                        <table class="table table-bordered table-striped align-middle">
                            <thead class="table-light">
                                <tr>
                                    <th class="text-center" style="width: 50px;">#</th>
                                    <th>Item</th>
                                    <th class="text-end" style="width: 100px;">Qty</th>
                                    <th class="text-end" style="width: 120px;">Price</th>
                                    <th class="text-end" style="width: 120px;">Amount</th>
                                </tr>
                            </thead>
                            <tbody>
                                @{ int lineNumber = 0; }
                                @foreach (var line in Model.Lines)
                                {
                                    lineNumber++;
                                    <tr>
                                        <td class="text-center">@lineNumber</td>
                                        <td>@line.ItemName</td>
                                        <td class="text-end">@line.Qty.ToString("N0")</td>
                                        <td class="text-end">@line.Price.ToString("N0")</td>
                                        <td class="text-end">@line.Amount.ToString("N0")</td>
                                    </tr>
                                }
                            </tbody>
                        </table>
                    </div>
                </div>
                
                <!-- Workflow Tab -->
                <div class="tab-pane" id="workflow" role="tabpanel">
                    <div id="workflowHistoryContainer" 
                         hx-get="@Url.Action("WorkflowHistory", "{Module}s", new { refId = Model.Header.{Module}Id, wfFormId = 1 })"
                         hx-trigger="load"
                         hx-swap="innerHTML">
                        <div class="text-center py-4">
                            <div class="spinner-border text-primary" role="status">
                                <span class="visually-hidden">Loading...</span>
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Log Tab -->
                <div class="tab-pane" id="log" role="tabpanel">
                    <div class="row">
                        <div class="col-md-6">
                            <table class="table table-borderless">
                                <tr>
                                    <td class="fw-bold" style="width: 150px;">Created Date</td>
                                    <td>@Model.Header.CreatedDate.ToString("dd MMM yyyy HH:mm:ss")</td>
                                </tr>
                                <tr>
                                    <td class="fw-bold">Created By</td>
                                    <td>@Model.Header.CreatedBy</td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@section scripts {
    <script src="~/js/{module}s/{module}-details.js"></script>
}
```

### 11.4 Partial View Pattern (_ListContainer)

```cshtml
@model ResearchApps.Service.Vm.Common.PagedListVm<ResearchApps.Service.Vm.{Module}HeaderVm>

<table class="table table-bordered table-hover align-middle mb-0" style="width: 100%;">
    <thead class="table-light">
    <tr>
        <th class="sortable-header" style="width: 140px; cursor: pointer;" 
            @@click="sort('{Module}Id')" data-column="{Module}Id">
            <div class="d-flex align-items-center justify-content-center">
                <span>{Module} ID</span>
                <template x-if="sortBy === '{Module}Id'">
                    <i :class="sortAsc ? 'ri-arrow-up-s-line' : 'ri-arrow-down-s-line'" class="ms-1"></i>
                </template>
            </div>
        </th>
        <!-- More columns... -->
    </tr>
    </thead>
    <tbody id="{module}-table-body">
    @await Html.PartialAsync("_Partials/_{Module}ListPartial", Model)
    </tbody>
    <tfoot class="table-light">
    <tr>
        <td>
            <input type="text" class="form-control form-control-sm" placeholder="Search ID..."
                   x-model="filters.{Module}Id" @@input.debounce.500ms="fetchList()">
        </td>
        <!-- More filter inputs... -->
    </tr>
    </tfoot>
</table>

<!-- Pagination -->
@await Html.PartialAsync("_Partials/_{Module}PaginationPartial", Model)
```

### 11.5 Alpine.js Directives Usage

**Syntax Rules:**

```cshtml
<!-- ✅ Correct - Use @@ to escape @ in Razor -->
<button @@click="save()">Save</button>
<div @@click.prevent="handleClick()">Click me</div>
<input @@input.debounce.500ms="search()" />
<div x-show.important="isVisible" x-cloak>Content</div>

<!-- ✅ Binding attributes -->
<div :class="{ 'active': isActive }">
<button :disabled="isLoading">
<span x-text="message"></span>

<!-- ✅ Conditionals -->
<template x-if="showModal">
    <!-- Use template for x-if -->
</template>

<div x-show="isVisible" x-cloak>
    <!-- Use div for x-show -->
</div>

<!-- ❌ Wrong - Single @ will cause Razor errors -->
<button @click="save()">Save</button>
```

### 11.6 Form Elements

**Input Fields:**

```cshtml
<div class="form-group">
    <label class="form-label">Field Name <span class="text-danger">*</span></label>
    <input type="text" 
           name="FieldName"
           class="form-control"
           value="@Model.FieldName"
           placeholder="Enter field name"
           maxlength="50"
           required />
</div>
```

**Readonly Fields (Greyed Out):**

```cshtml
<!-- ✅ CORRECT - Use bg-light for readonly/disabled fields -->
<div class="form-group">
    <label class="form-label">Unit</label>
    <input type="text" 
           class="form-control bg-light"
           x-model="lineModal.data.unitName"
           readonly />
</div>

<div class="form-group">
    <label class="form-label">Amount</label>
    <input type="text" 
           class="form-control bg-light"
           :value="formatNumber(lineModal.data.amount)"
           readonly />
</div>

<!-- ❌ WRONG - Missing bg-light class -->
<input type="text" class="form-control" readonly />
```

**Key Points:**
- ✅ Always add `bg-light` to readonly or disabled inputs
- ✅ Provides visual feedback that field is not editable
- ✅ Consistent with Bootstrap's disabled input styling

**Select with TomSelect:**

```cshtml
<div class="form-group">
    <label class="form-label">Customer <span class="text-danger">*</span></label>
    <select id="Header_CustomerId" 
            name="CustomerId"
            x-ref="customerSelect"
            data-tomselect
            data-url="/api/Customers/cbo"
            required>
        <option value="">Select Customer</option>
    </select>
</div>
```

**Date Picker with Flatpickr:**

```cshtml
<div class="form-group">
    <label class="form-label">Date <span class="text-danger">*</span></label>
    <div class="input-group">
        <input type="text" 
               class="form-control" 
               id="Header_{Module}Date"
               name="{Module}Date"
               x-ref="{module}DatePicker"
               data-flatpickr
               required />
        <span class="input-group-text"><i class="ri-calendar-line"></i></span>
    </div>
</div>
```

### 11.7 Button Patterns

```cshtml
<!-- Primary Action -->
<button type="submit" class="btn btn-primary" :disabled="isLoading">
    <span x-show="!isLoading">
        <i class="ri-save-line me-1"></i> Save
    </span>
    <span x-show="isLoading" x-cloak>
        <span class="spinner-border spinner-border-sm me-1" role="status"></span>
        Saving...
    </span>
</button>

<!-- Secondary Action -->
<button type="button" class="btn btn-secondary" @@click="cancel()">
    <i class="ri-close-line me-1"></i> Cancel
</button>

<!-- Danger Action -->
<button type="button" class="btn btn-danger" @@click="deleteItem()">
    <i class="ri-delete-bin-line me-1"></i> Delete
</button>

<!-- Icon Only Button -->
<button type="button" class="btn btn-soft-secondary" @@click="refresh()" title="Refresh">
    <i class="ri-refresh-line" :class="{ 'spin': isLoading }"></i>
</button>
```

### 11.8 Modal Pattern

```cshtml
<div class="modal fade" 
     :class="{ 'show d-block': showModal }"
     x-show="showModal"
     x-cloak
     @@click.self="showModal = false"
     @@keydown.escape.window="showModal = false">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">
                    <i class="ri-information-line me-2"></i>Modal Title
                </h5>
                <button type="button" class="btn-close" @@click="showModal = false"></button>
            </div>
            <div class="modal-body">
                <!-- Modal content -->
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" @@click="showModal = false">Cancel</button>
                <button type="button" 
                        class="btn btn-primary" 
                        :disabled="isProcessing"
                        @@click="confirm()">
                    <span x-show="!isProcessing">Confirm</span>
                    <span x-show="isProcessing" x-cloak>
                        <span class="spinner-border spinner-border-sm me-1"></span>
                        Processing...
                    </span>
                </button>
            </div>
        </div>
    </div>
</div>
<div class="modal-backdrop fade show" x-show="showModal" x-cloak></div>
```

### 11.9 HTMX Integration

```cshtml
<!-- Load content on page load -->
<div id="content-container"
     hx-get="/Module/Action"
     hx-trigger="load"
     hx-swap="innerHTML">
    <!-- Loading placeholder -->
</div>

<!-- Load with parameters -->
<div id="list-container"
     hx-get="/Module/List"
     hx-trigger="load"
     :hx-vals="JSON.stringify({page: currentPage, sortBy: sortBy})">
</div>

<!-- Button trigger -->
<button type="button"
        hx-post="/Module/Action"
        hx-target="#result"
        hx-swap="innerHTML"
        class="btn btn-primary">
    Submit
</button>
```

### 11.10 Status Badges

```cshtml
@{
    string badgeClass = Model.{Module}StatusId switch
    {
        0 => "badge bg-secondary", // Draft
        1 => "badge bg-info",      // Open/Active
        4 => "badge bg-warning",   // Pending
        5 => "badge bg-success",   // Approved/Completed
        6 => "badge bg-danger",    // Rejected/Cancelled
        _ => "badge bg-secondary"
    };
}

<span class="@badgeClass">@Model.{Module}StatusName</span>
```

### 11.11 Number Formatting

```cshtml
<!-- Integer with thousand separators -->
@Model.Qty.ToString("N0")

<!-- Decimal with 2 places -->
@Model.Price.ToString("N2")

<!-- Currency (without symbol) -->
@Model.Total.ToString("N0")

<!-- Date formats -->
@Model.{Module}Date.ToString("dd MMM yyyy")         <!-- 16 Jan 2026 -->
@Model.CreatedDate.ToString("dd MMM yyyy HH:mm:ss") <!-- 16 Jan 2026 14:30:45 -->
@Model.{Module}Date.ToString("yyyy-MM-dd")          <!-- 2026-01-16 (for inputs) -->
```

---

## 12. JavaScript Patterns

### 12.1 File Organization

```
wwwroot/
├── js/
│   ├── components/              # Reusable components
│   │   ├── workflow-component.js
│   │   └── alert-modal.js
│   ├── {module}s/              # Module-specific scripts
│   │   ├── {module}-index.js
│   │   ├── {module}-details.js
│   │   ├── {module}-create.js
│   │   └── {module}-edit.js
│   └── assets/js/
│       ├── alpine-components.js  # Global Alpine components
│       └── helper.js             # Utility functions
```

### 12.2 Index Page Component Pattern

```javascript
/**
 * {Module Name} Index Page Component
 * Handles listing, filtering, sorting, and pagination
 */
function {module}Index() {
    return {
        // State
        isLoading: false,
        isExporting: false,
        sortBy: '{Module}Id',
        sortAsc: false,
        
        // Filters
        filters: {
            {Module}Id: '',
            {Module}DateFrom: '',
            {Module}DateTo: '',
            {Module}StatusId: ''
        },
        
        // Component instances
        dateRangePicker: null,
        
        /**
         * Initialize component
         */
        init() {
            this.setupHtmxListeners();
        },
        
        /**
         * Setup HTMX event listeners
         */
        setupHtmxListeners() {
            // Before request
            document.body.addEventListener('htmx:beforeRequest', (e) => {
                if (e.detail.target?.id === '{module}-list-container') {
                    this.isLoading = true;
                }
            });
            
            // After swap
            document.body.addEventListener('htmx:afterSwap', (e) => {
                if (e.detail.target?.id === '{module}-list-container') {
                    this.isLoading = false;
                    this.initializeDatePicker();
                }
            });
            
            // Response errors (4xx, 5xx)
            document.body.addEventListener('htmx:responseError', (e) => {
                if (e.detail.target?.id === '{module}-list-container') {
                    this.isLoading = false;
                    this.showErrorMessage(e);
                }
            });
            
            // Network errors
            document.body.addEventListener('htmx:sendError', (e) => {
                if (e.detail.target?.id === '{module}-list-container') {
                    this.isLoading = false;
                    this.showNetworkError(e);
                }
            });
        },
        
        /**
         * Sort by column
         */
        sort(column) {
            if (this.sortBy === column) {
                this.sortAsc = !this.sortAsc;
            } else {
                this.sortBy = column;
                this.sortAsc = true;
            }
            this.fetchList();
        },
        
        /**
         * Clear all filters
         */
        clearFilters() {
            this.filters = {
                {Module}Id: '',
                {Module}DateFrom: '',
                {Module}DateTo: '',
                {Module}StatusId: ''
            };
            if (this.dateRangePicker) {
                this.dateRangePicker.clear();
            }
            this.sortBy = '{Module}Id';
            this.sortAsc = false;
            this.fetchList();
        },
        
        /**
         * Fetch list data
         */
        fetchList(page = 1) {
            const container = document.getElementById('{module}-list-container');
            if (!container) return;
            
            const params = new URLSearchParams();
            params.set('page', page);
            
            // Add filters
            Object.entries(this.filters).forEach(([key, value]) => {
                if (value && value.trim() !== '') {
                    params.set(`filters[${key}]`, value);
                }
            });
            
            // Add sort parameters
            if (this.sortBy) {
                params.set('sortBy', this.sortBy);
                params.set('sortAsc', this.sortAsc);
            }
            
            const url = `/{Module}s/List?${params.toString()}`;
            
            htmx.ajax('GET', url, {
                target: '#{module}-list-container',
                swap: 'innerHTML'
            });
        },
        
        /**
         * Initialize date range picker
         */
        initializeDatePicker() {
            if (this.dateRangePicker) {
                this.dateRangePicker.destroy();
            }
            
            const dateInput = document.querySelector('#{module}-date-range');
            if (!dateInput) return;
            
            this.dateRangePicker = flatpickr(dateInput, {
                wrap: true,
                mode: 'range',
                dateFormat: 'Y-m-d',
                onChange: (selectedDates) => {
                    if (selectedDates.length === 2) {
                        this.filters.{Module}DateFrom = this.formatDateISO(selectedDates[0]);
                        this.filters.{Module}DateTo = this.formatDateISO(selectedDates[1]);
                        this.fetchList();
                    } else if (selectedDates.length === 0) {
                        this.filters.{Module}DateFrom = '';
                        this.filters.{Module}DateTo = '';
                        this.fetchList();
                    }
                }
            });
        },
        
        /**
         * Format date to ISO string (YYYY-MM-DD)
         */
        formatDateISO(date) {
            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');
            return `${year}-${month}-${day}`;
        },
        
        /**
         * Export to Excel
         */
        async exportToExcel() {
            this.isExporting = true;
            try {
                const params = new URLSearchParams();
                Object.entries(this.filters).forEach(([key, value]) => {
                    if (value && value.trim() !== '') {
                        params.set(`filters[${key}]`, value);
                    }
                });
                
                const url = `/{Module}s/Export?${params.toString()}`;
                window.location.href = url;
            } catch (error) {
                console.error('Export error:', error);
                alert('Failed to export data.');
            } finally {
                setTimeout(() => { this.isExporting = false; }, 1000);
            }
        },
        
        /**
         * Show error message
         */
        showErrorMessage(event) {
            const status = event.detail.xhr.status;
            const statusText = event.detail.xhr.statusText || 'Error';
            event.detail.target.innerHTML = `
                <div class="text-center py-5">
                    <div class="mb-3">
                        <i class="ri-error-warning-line text-danger" style="font-size: 3rem;"></i>
                    </div>
                    <h5 class="text-danger">Failed to Load Data</h5>
                    <p class="text-muted">Server returned ${status} ${statusText}</p>
                    <button type="button" class="btn btn-primary" onclick="location.reload()">
                        <i class="ri-refresh-line me-1"></i> Reload Page
                    </button>
                </div>
            `;
        },
        
        /**
         * Show network error
         */
        showNetworkError(event) {
            event.detail.target.innerHTML = `
                <div class="text-center py-5">
                    <div class="mb-3">
                        <i class="ri-wifi-off-line text-warning" style="font-size: 3rem;"></i>
                    </div>
                    <h5 class="text-warning">Network Error</h5>
                    <p class="text-muted">Unable to connect to the server.</p>
                    <button type="button" class="btn btn-primary" onclick="location.reload()">
                        <i class="ri-refresh-line me-1"></i> Retry
                    </button>
                </div>
            `;
        }
    };
}
```

### 12.3 Edit Page Component Pattern

```javascript
/**
 * {Module Name} Edit Page Component
 * Handles header editing, line item management, and workflow actions
 * 
 * @param {Array} initialLines - Initial line items from server
 * @param {Object} config - Configuration object
 * @param {number} config.recId - Record ID
 * @param {string} config.{module}Id - {Module} ID
 * @param {string} config.{module}Date - Order/transaction date (YYYY-MM-DD)
 * @param {number} config.customerId - Customer/Supplier ID
 * @param {string} config.customerName - Customer/Supplier name
 */
function {module}Edit(initialLines = [], config = {}) {
    return {
        // Configuration
        config: {
            recId: 0,
            {module}Id: '',
            {module}Date: '',
            customerId: 0,
            customerName: '',
            ...config
        },
        
        // Header state
        header: {
            recId: config.recId || 0,
            {module}Id: config.{module}Id || '',
            notes: config.notes || ''
        },
        isHeaderSaving: false,
        
        // Lines state
        lines: initialLines || [],
        
        // Line modal state
        lineModal: {
            show: false,
            mode: 'add', // 'add' or 'edit'
            data: {
                recId: 0,
                {module}LineId: 0,
                itemId: '',
                itemName: '',
                qty: 1,
                price: 0,
                amount: 0,
                notes: ''
            },
            errors: {},
            isSaving: false
        },
        
        // Delete modal state
        deleteModal: {
            show: false,
            line: null,
            isDeleting: false
        },
        
        // Component instances
        customerSelect: null,
        lineItemSelect: null,
        {module}DatePicker: null,
        
        /**
         * Initialize component
         */
        init() {
            this.header.recId = this.config.recId;
            this.header.{module}Id = this.config.{module}Id;
            this.header.notes = this.config.notes;
            this.initHeaderComponents();
        },
        
        /**
         * Initialize header form components (TomSelect, Flatpickr)
         */
        initHeaderComponents() {
            // Initialize date picker
            this.{module}DatePicker = initFlatpickr('#Header_{Module}Date', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: this.config.{module}Date
            });
            
            // Initialize Customer/Supplier TomSelect
            this.customerSelect = initTomSelect('#Header_CustomerId', {
                url: '/api/Customers/cbo', // or /api/Suppliers/cbo
                placeholder: 'Select Customer'
            });
            
            // Pre-select current customer
            if (this.customerSelect && this.config.customerId) {
                this.customerSelect.addOption({
                    value: String(this.config.customerId),
                    text: this.config.customerName
                });
                this.customerSelect.setValue(String(this.config.customerId));
            }
        },
        
        /**
         * Initialize line item select in modal
         */
        initLineItemSelect() {
            // Destroy existing instance if present
            if (this.lineItemSelect) {
                this.lineItemSelect.destroy();
            }
            
            // Initialize Item TomSelect for line modal
            this.lineItemSelect = initTomSelect('#lineItemId', {
                url: '/api/Items/cbo',
                placeholder: 'Select Item',
                maxOptions: 100,
                onChange: (value) => {
                    if (value) {
                        this.loadItemDetails(value);
                    }
                }
            });
            
            // Pre-select item if in edit mode
            if (this.lineModal.mode === 'edit' && this.lineModal.data.itemId) {
                this.lineItemSelect.addOption({
                    value: String(this.lineModal.data.itemId),
                    text: this.lineModal.data.itemName
                });
                this.lineItemSelect.setValue(String(this.lineModal.data.itemId));
            }
        },
        
        // ... line management methods (showLineModal, saveLine, deleteLine, etc.)
    };
}
```

**Corresponding Edit.cshtml:**

```cshtml
@model ResearchApps.Service.Vm.{Module}Vm

@{
    ViewBag.Title = "Edit {Module Name}";
    ViewBag.pTitle = $"Edit {Module Name} No. {Model.Header.{Module}Id}";
    Layout = "_Layout";
    
    // ⚠️ CRITICAL: Serialize lines for Alpine.js with CAMELCASE properties
    // Backend C# uses PascalCase, but JavaScript expects camelCase
    var linesJson = System.Text.Json.JsonSerializer.Serialize(Model.Lines.Select(l => new {
        recId = l.RecId,
        {module}LineId = l.{Module}LineId,
        {module}Id = l.{Module}Id,
        itemId = l.ItemId,
        itemName = l.ItemName,
        qty = l.Qty,
        unitId = l.UnitId,
        unitName = l.UnitName,
        price = l.Price,
        amount = l.Amount,
        notes = l.Notes ?? "",
        createdDate = l.CreatedDate,
        createdBy = l.CreatedBy,
        modifiedDate = l.ModifiedDate,
        modifiedBy = l.ModifiedBy
    }));
    
    // Config object for {module}Edit component (also camelCase)
    var configJson = System.Text.Json.JsonSerializer.Serialize(new {
        recId = Model.Header.RecId,
        {module}Id = Model.Header.{Module}Id,
        {module}Date = Model.Header.{Module}Date.ToString("yyyy-MM-dd"),
        customerId = Model.Header.CustomerId,
        customerName = Model.Header.CustomerName,
        notes = Model.Header.Notes ?? ""
    });
}

// ⚠️ CRITICAL: Use @Html.Raw() to prevent HTML encoding of JSON
<div x-data="{ ...{module}Edit(@Html.Raw(linesJson), @Html.Raw(configJson)), workflow: createWorkflowComponent({ recId: @Model.Header.RecId, refId: '@Model.Header.{Module}Id', baseUrl: '/{Module}s' }) }" x-init="init()">
    @Html.AntiForgeryToken()
    
    <!-- Header form -->
    <form id="{module}Form" method="post">
        <!-- Customer/Supplier select with x-ref and data attributes -->
        <select id="Header_CustomerId" 
                name="CustomerId"
                x-ref="customerSelect"
                data-tomselect
                data-url="/api/Customers/cbo"
                required>
            <option value="">Select Customer</option>
        </select>
        
        <!-- Date picker with x-ref and data-flatpickr -->
        <input type="text" 
               id="Header_{Module}Date"
               name="{Module}Date"
               x-ref="{module}DatePicker"
               data-flatpickr
               required />
    </form>
    
    <!-- Line items table and modals -->
    
    <!-- Add/Edit Line Modal -->
    <div class="modal fade" 
         :class="{ 'show d-block': lineModal.show }" 
         x-show="lineModal.show" 
         tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">
                        <span x-text="lineModal.mode === 'add' ? 'Add Line Item' : 'Edit Line Item'"></span>
                    </h5>
                    <button type="button" class="btn-close" @click="closeLineModal()"></button>
                </div>
                <div class="modal-body">
                    <!-- Item Select - Must have ID for initTomSelect('#lineItemId') -->
                    <label class="form-label">Item <span class="text-danger">*</span></label>
                    <select id="lineItemId" class="form-control" required>
                        <option value="">Select Item</option>
                    </select>
                    
                    <!-- Numeric inputs - Stay formatted while typing -->
                    <label class="form-label">Quantity <span class="text-danger">*</span></label>
                    <input type="text" 
                           class="form-control text-end" 
                           inputmode="numeric"
                           :value="formatNumber(lineModal.data.qty)"
                           @input="lineModal.data.qty = parseNumber($event.target.value); calculateLineAmount();"
                           @blur="$event.target.value = formatNumber(lineModal.data.qty)"
                           @keypress="if(!/[0-9]/.test($event.key) && $event.key !== 'Backspace' && $event.key !== 'Delete') $event.preventDefault()"
                           required />
                </div>
            </div>
        </div>
    </div>
</div>

@section scripts {
    <script src="~/js/components/workflow-component.js"></script>
    <script src="~/js/{module}s/{module}-edit.js"></script>
}
```

**Key Points:**
- ✅ **JSDoc documentation** - Complete parameter descriptions
- ✅ **Direct function call** - No wrapper, called directly in x-data
- ✅ **Inline JSON serialization** - `@linesJson`, `@configJson` embedded in view
- ✅ **x-ref for component instances** - Used to reference DOM elements
- ✅ **data-tomselect + data-url** - Required for global `initTomSelect()` helper
- ✅ **data-flatpickr** - Required for global `initFlatpickr()` helper
- ✅ **Spread operator** - `...config` merges with defaults
- ✅ **Preselection pattern** - `addOption()` then `setValue()` for dropdowns
- ✅ **Component lifecycle** - `init()` → `initHeaderComponents()` → `initLineItemSelect()`
        
        /**
         * Initialize component
         */
        init() {
            this.initHeaderComponents();
        },
        
        /**
         * Initialize header form components
         */
        initHeaderComponents() {
            // Initialize date picker
            this.datePicker = initFlatpickr('#Header_{Module}Date', {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                defaultDate: this.config.{module}Date
            });
            
            // Initialize TomSelect for dropdown
            this.customerSelect = initTomSelect('#Header_CustomerId', {
                url: '/api/Customers/cbo',
                placeholder: 'Select Customer'
            });
            
            // Pre-select current value
            if (this.customerSelect && this.config.customerId) {
                this.customerSelect.addOption({
                    value: String(this.config.customerId),
                    text: this.config.customerName
                });
                this.customerSelect.setValue(String(this.config.customerId));
            }
        },
        
        /**
         * Save header
         */
        async saveHeader() {
            this.isHeaderSaving = true;
            
            const form = document.getElementById('{module}Form');
            const formData = new FormData(form);
            
            try {
                const response = await fetch('/{Module}s/Edit', {
                    method: 'POST',
                    body: formData
                });
                
                if (response.redirected) {
                    window.location.href = response.url;
                } else if (response.ok) {
                    window.location.href = `/{Module}s/Details/${this.header.recId}`;
                } else {
                    const errorMessage = await this.parseErrorResponse(response);
                    alert('Error saving:\n\n' + errorMessage);
                }
            } catch (error) {
                console.error('Save error:', error);
                alert('An error occurred while saving.');
            } finally {
                this.isHeaderSaving = false;
            }
        },
        
        /**
         * Parse error response
         */
        async parseErrorResponse(response) {
            try {
                const text = await response.text();
                // Try to extract error message from HTML
                const parser = new DOMParser();
                const doc = parser.parseFromString(text, 'text/html');
                const errorDiv = doc.querySelector('.validation-summary-errors');
                if (errorDiv) {
                    return errorDiv.textContent.trim();
                }
                return `Server error: ${response.status} ${response.statusText}`;
            } catch (e) {
                return `Server error: ${response.status} ${response.statusText}`;
            }
        }
    };
}
```

### 12.4 Number Input Formatting in Modals

For numeric inputs in modals (line items, prices, quantities), use **formatted text inputs** instead of `type="number"` to provide proper thousand separators and decimal formatting:

**Pattern:**
- Use `type="text"` with `inputmode="numeric"` for mobile keyboard
- Display formatted value with `formatNumber()`
- Parse input on change with `parseNumber()`
- Reformat on blur for consistent display
- Prevent non-numeric keypresses

**CSHTML Implementation:**

```cshtml
<!-- ❌ WRONG - type="number" does not support formatting -->
<input type="number" 
       class="form-control" 
       x-model="lineModal.data.qty"
       step="0.01" />

<!-- ✅ CORRECT - Formatted text input that stays formatted while typing -->
<label class="form-label">Quantity <span class="text-danger">*</span></label>
<input type="text" 
       class="form-control text-end" 
       inputmode="numeric"
       :value="formatNumber(lineModal.data.qty)"
       @input="lineModal.data.qty = parseNumber($event.target.value); calculateLineAmount();"
       @blur="$event.target.value = formatNumber(lineModal.data.qty)"
       @keypress="if(!/[0-9]/.test($event.key) && $event.key !== 'Backspace' && $event.key !== 'Delete') $event.preventDefault()"
       required />

<!-- Price field follows same pattern -->
<label class="form-label">Price <span class="text-danger">*</span></label>
<input type="text" 
       class="form-control text-end" 
       inputmode="numeric"
       :value="formatNumber(lineModal.data.price)"
       @input="lineModal.data.price = parseNumber($event.target.value); calculateLineAmount();"
       @blur="$event.target.value = formatNumber(lineModal.data.price)"
       @keypress="if(!/[0-9]/.test($event.key) && $event.key !== 'Backspace' && $event.key !== 'Delete') $event.preventDefault()"
       required />

<!-- Amount (read-only, formatted display) -->
<label class="form-label">Amount</label>
<input type="text" 
       class="form-control bg-light" 
       :value="formatNumber(lineModal.data.amount)"
       readonly />
```

**Key Elements:**
- `type="text"` - Allows custom formatting display
- `inputmode="numeric"` - Shows numeric keyboard on mobile
- `class="text-end"` - Right-align numbers for readability
- `class="bg-light"` - Grey out readonly fields for visual feedback
- `:value="formatNumber(lineModal.data.qty)"` - Always display formatted value (even while typing)
- `@input="lineModal.data.qty = parseNumber($event.target.value); calculateLineAmount();"` - Update raw value and recalculate
- `@blur="$event.target.value = formatNumber(lineModal.data.qty)"` - Ensure formatting on blur
- `@keypress` - Prevent non-numeric characters from being entered

**Global Helper Functions:**

These functions are available globally from `alpine-components.js` and can be called directly in Alpine expressions **without** adding them as component methods:

```javascript
// Format number with thousand separators and 2 decimals
function formatNumber(value) {
    if (value === null || value === undefined || value === '') return '';
    const num = parseFloat(String(value).replace(/,/g, ''));
    if (isNaN(num)) return value;
    return num.toLocaleString('en-US');
}

// Parse formatted string back to number
function parseNumber(value) {
    if (value === null || value === undefined || value === '') return '';
    // Remove thousand separators and return plain number string
    return String(value).replace(/,/g, '');
}
```

**Usage in Alpine Expressions:**
```cshtml
<!-- ✅ CORRECT - Call global functions directly -->
:value="formatNumber(lineModal.data.qty)"
@input="lineModal.data.qty = parseNumber($event.target.value)"

<!-- ❌ WRONG - Do NOT add wrapper methods in component -->
// Don't do this in your component:
formatNumber(value) {
    return formatNumber(value);  // Unnecessary wrapper
}
```

**Why Not `type="number"`?**
- ❌ Cannot display thousand separators (e.g., "1,234.56")
- ❌ Browser controls (up/down arrows) can confuse users
- ❌ Mobile keyboards may show unnecessary symbols
- ❌ Inconsistent behavior across browsers
- ✅ Text input with `inputmode="numeric"` gives full control

### 12.5 Details Page Component Pattern

```javascript
/**
 * {Module Name} Details Page Component
 */
function {module}Details({module}Id, recId) {
    return {
        workflow: null,
        isLoading: false,
        
        lineDetailModal: {
            show: false,
            line: null
        },

        /**
         * Initialize component
         */
        init() {
            // Initialize workflow component
            this.workflow = createWorkflowComponent({
                recId: recId,
                refId: {module}Id,
                baseUrl: '/{Module}s',
                redirectUrl: `/{Module}s/Details/${recId}`
            });

            // Watch workflow processing state
            this.$watch('workflow.modal.isProcessing', value => {
                this.isLoading = value;
            });
        },

        /**
         * Show line detail modal
         */
        showLineDetailModal(lineJson) {
            try {
                const line = typeof lineJson === 'string' ? JSON.parse(lineJson) : lineJson;
                this.lineDetailModal.line = line;
                this.lineDetailModal.show = true;
            } catch (error) {
                console.error('Failed to parse line data:', error);
            }
        }
    };
}
```

### 12.5 Utility Functions

**TomSelect Initialization:**

```javascript
/**
 * Initialize TomSelect dropdown
 * @param {string} selector - Element selector
 * @param {Object} options - Configuration options
 * @returns {TomSelect} TomSelect instance
 */
function initTomSelect(selector, options = {}) {
    const element = document.querySelector(selector);
    if (!element) return null;
    
    const defaults = {
        valueField: 'value',
        labelField: 'text',
        searchField: 'text',
        placeholder: 'Select...',
        maxOptions: 100,
        load: function(query, callback) {
            if (!options.url) {
                callback();
                return;
            }
            
            fetch(`${options.url}?search=${encodeURIComponent(query)}`)
                .then(response => response.json())
                .then(json => {
                    callback(json);
                })
                .catch(() => {
                    callback();
                });
        }
    };
    
    return new TomSelect(element, { ...defaults, ...options });
}
```

**Flatpickr Initialization:**

```javascript
/**
 * Initialize Flatpickr date picker
 * @param {string} selector - Element selector
 * @param {Object} options - Configuration options
 * @returns {flatpickr} Flatpickr instance
 */
function initFlatpickr(selector, options = {}) {
    const element = document.querySelector(selector);
    if (!element) return null;
    
    const defaults = {
        dateFormat: 'Y-m-d',
        altInput: true,
        altFormat: 'd M Y',
        allowInput: false
    };
    
    return flatpickr(element, { ...defaults, ...options });
}
```

### 12.6 JSDoc Comments

```javascript
/**
 * Function description
 * 
 * @param {string} param1 - Description of param1
 * @param {number} param2 - Description of param2
 * @param {Object} options - Options object
 * @param {boolean} [options.optional] - Optional parameter
 * @returns {Promise<Object>} Description of return value
 * @throws {Error} When something goes wrong
 * 
 * @example
 * const result = await myFunction('test', 123, { optional: true });
 */
async function myFunction(param1, param2, options = {}) {
    // Implementation
}
```

### 12.7 Naming Conventions

```javascript
// ✅ camelCase for variables and functions
let isLoading = false;
const userId = 123;
function fetchData() {}

// ✅ PascalCase for classes and constructors
class UserManager {}
function CustomerOrder() {}

// ✅ UPPER_SNAKE_CASE for constants
const API_BASE_URL = '/api';
const MAX_RETRY_COUNT = 3;

// ✅ Descriptive names
// Good
const userList = [];
const isFormValid = true;
function validateEmail(email) {}

// ❌ Bad
const ul = [];
const flag = true;
function ve(e) {}
```

### 12.8 Error Handling

```javascript
/**
 * Fetch data with error handling
 */
async function fetchData(url) {
    try {
        const response = await fetch(url);
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const data = await response.json();
        return { success: true, data };
        
    } catch (error) {
        console.error('Fetch error:', error);
        return { 
            success: false, 
            error: error.message || 'An error occurred' 
        };
    }
}
```

### 12.9 Async/Await Pattern

```javascript
// ✅ Use async/await for asynchronous operations
async function saveData() {
    this.isLoading = true;
    
    try {
        const response = await fetch('/api/save', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(this.data)
        });
        
        if (response.ok) {
            alert('Saved successfully');
        } else {
            throw new Error('Save failed');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('An error occurred');
    } finally {
        this.isLoading = false;
    }
}

// ❌ Avoid promise chains when async/await is clearer
function saveData() {
    this.isLoading = true;
    
    fetch('/api/save', { method: 'POST' })
        .then(response => response.json())
        .then(data => {
            alert('Saved');
        })
        .catch(error => {
            alert('Error');
        })
        .finally(() => {
            this.isLoading = false;
        });
}
```

### 12.10 Component Composition

```javascript
/**
 * Create reusable workflow component
 */
function createWorkflowComponent(config) {
    const { recId, refId, baseUrl = '/Module', customActionUrls = {} } = config;

    const actionUrls = {
        'submit': `${baseUrl}/Submit`,
        'approve': `${baseUrl}/Approve`,
        'reject': `${baseUrl}/Reject`,
        'recall': `${baseUrl}/Recall`,
        ...customActionUrls
    };

    return {
        modal: {
            show: false,
            action: null,
            notes: '',
            isProcessing: false
        },

        showModal(action) {
            this.modal.show = true;
            this.modal.action = action;
            this.modal.notes = '';
        },

        closeModal() {
            if (!this.modal.isProcessing) {
                this.modal.show = false;
                this.modal.action = null;
                this.modal.notes = '';
            }
        },

        async execute() {
            // Validation
            if (this.modal.action === 'reject' && !this.modal.notes?.trim()) {
                alert('Please enter a reason for rejection.');
                return;
            }

            this.modal.isProcessing = true;

            try {
                const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                const body = new URLSearchParams();
                body.append('RecId', recId);
                body.append('RefId', refId);
                body.append('Notes', this.modal.notes);
                body.append('__RequestVerificationToken', token);

                const response = await fetch(actionUrls[this.modal.action], {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    body: body.toString()
                });

                if (response.ok || response.redirected) {
                    window.location.href = response.redirected 
                        ? response.url 
                        : `${baseUrl}/Details/${recId}`;
                } else {
                    alert('An error occurred. Please try again.');
                }
            } catch (error) {
                console.error('Workflow action failed:', error);
                alert('An error occurred. Please try again.');
            } finally {
                this.modal.isProcessing = false;
            }
        }
    };
}
```

---

## 11. Error Handling Patterns

### 11.1 Inline Validation Errors (Preferred for Forms)

**Use inline validation for form fields to provide immediate, contextual feedback without blocking the UI.**

#### JavaScript Pattern

```javascript
// Line modal state must include errors object
lineModal: {
    show: false,
    mode: 'add',
    data: {
        itemId: 0,
        qty: 1,
        price: 0,
        // ... other fields
    },
    errors: {},  // ✅ Required for inline validation
    isSaving: false
}

// Clear errors when opening modal
showLineModal(mode, line = null) {
    this.lineModal.mode = mode;
    this.lineModal.errors = {};  // ✅ Always clear errors
    this.lineModal.show = true;
    // ... rest of logic
}

// Validation sets errors object instead of alert()
validateLine() {
    this.lineModal.errors = {};  // ✅ Clear previous errors
    
    if (!this.lineModal.data.itemId) {
        this.lineModal.errors.itemId = 'Please select an item';
    }
    
    if (!this.lineModal.data.qty || this.lineModal.data.qty <= 0) {
        this.lineModal.errors.qty = 'Quantity must be greater than 0';
    } else if (this.lineModal.data.qty > this.lineModal.data.outstandingQty) {
        // Chained validation - use else-if for related checks
        this.lineModal.errors.qty = `Quantity cannot exceed outstanding quantity of ${this.lineModal.data.outstandingQty}`;
    }
    
    if (this.lineModal.data.price < 0) {
        this.lineModal.errors.price = 'Price cannot be negative';
    }
    
    return Object.keys(this.lineModal.errors).length === 0;
}
```

#### Razor View Pattern

Display errors inline below each field using Alpine.js directives:

```cshtml
<div class="col-6">
    <label class="form-label">Item <span class="text-danger">*</span></label>
    <select id="lineItemId" 
            class="form-select"
            :class="{ 'is-invalid': lineModal.errors.itemId }"
            required>
        <!-- options -->
    </select>
    <small class="text-danger" 
           x-show="lineModal.errors.itemId" 
           x-text="lineModal.errors.itemId"></small>
</div>

<div class="col-4">
    <label class="form-label">Quantity <span class="text-danger">*</span></label>
    <input type="text" 
           class="form-control text-end"
           :class="{ 'is-invalid': lineModal.errors.qty }"
           x-model="lineModal.data.qty"
           @@input="calculateLineAmount()"
           required />
    <small class="text-danger" 
           x-show="lineModal.errors.qty" 
           x-text="lineModal.errors.qty"></small>
</div>

<div class="col-4">
    <label class="form-label">Price <span class="text-danger">*</span></label>
    <input type="text" 
           class="form-control text-end"
           :class="{ 'is-invalid': lineModal.errors.price }"
           x-model="lineModal.data.price"
           @@input="calculateLineAmount()"
           required />
    <small class="text-danger" 
           x-show="lineModal.errors.price" 
           x-text="lineModal.errors.price"></small>
</div>
```

**Key Points:**
- Use `x-show` to conditionally display error messages (preserves space when hidden)
- Use `x-text` to bind error message text
- Optionally add `:class="{ 'is-invalid': lineModal.errors.fieldName }"` to highlight invalid fields
- Always place error message immediately below the input field

### 11.2 Server-Side Error Parsing (For Async Operations)

**Always use structured error parsing with detailed messages. Never use simple alert() without parsing the server response.**

#### parseErrorResponse Method

All edit pages must include this method to parse server responses:

```javascript
/**
 * Parse error response from server (HTML or JSON)
 */
async parseErrorResponse(response) {
    const contentType = response.headers.get('content-type');
    
    if (contentType && contentType.includes('application/json')) {
        const result = await response.json();
        return result.message || result.title || JSON.stringify(result);
    }
    
    if (contentType && contentType.includes('text/html')) {
        const text = await response.text();
        const parser = new DOMParser();
        const doc = parser.parseFromString(text, 'text/html');
        
        let errors = [];
        
        // Check for validation summary
        const validationSummary = doc.querySelector('.validation-summary-errors ul');
        if (validationSummary) {
            errors = Array.from(validationSummary.querySelectorAll('li'))
                .map(li => li.textContent.trim());
        }
        
        // Check for field-level validation errors
        const fieldErrors = doc.querySelectorAll('.field-validation-error, .invalid-feedback');
        if (fieldErrors.length > 0) {
            errors = errors.concat(
                Array.from(fieldErrors)
                    .map(el => el.textContent.trim())
                    .filter(text => text.length > 0)
            );
        }
        
        // Check for span.text-danger
        const dangerSpans = doc.querySelectorAll('span.text-danger');
        if (dangerSpans.length > 0) {
            errors = errors.concat(
                Array.from(dangerSpans)
                    .map(el => el.textContent.trim())
                    .filter(text => text.length > 0)
            );
        }
        
        if (errors.length > 0) {
            errors = [...new Set(errors)];
            return `Validation errors:\n\n${errors.map(e => `• ${e}`).join('\n')}`;
        }
        
        console.error('Full HTML response:', text);
        return `Server returned ${response.status} error.\n\nNo specific validation errors found.\nCheck browser console for full response.`;
    }
    
    const text = await response.text();
    return text || `Server returned ${response.status}: ${response.statusText}`;
}
```

#### Usage in CRUD Operations

**✅ Use Inline Validation for Form Fields:**
```javascript
async saveLine() {
    if (!this.validateLine()) return;  // Shows inline errors, doesn't proceed if invalid
    
    this.lineModal.isSaving = true;
    // ... save logic
}
```

**✅ Use Parsed Error Messages for Server Errors:**
```javascript
async saveLine() {
    if (!this.validateLine()) return;  // Shows inline errors first
    
    this.lineModal.isSaving = true;
    
    try {
        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            const result = await response.json();
            // ✅ Don't check result.isSuccess - it's private
            // If response.ok, treat as success
            if (this.lineModal.mode === 'add') {
                this.lines.push(result.data);
            } else {
                const index = this.lines.findIndex(l => l.recId === result.data.recId);
                if (index !== -1) {
                    this.lines[index] = result.data;
                }
            }
            this.closeLineModal();
        } else {
            const errorMessage = await this.parseErrorResponse(response);
            console.error('Save line error:', errorMessage);
            alert('Error saving line item:\n\n' + errorMessage);
        }
    } catch (error) {
        console.error('Save line failed:', error);
        alert('An error occurred while saving the line item:\n\n' + error.message);
    } finally {
        this.lineModal.isSaving = false;
    }
}
```

**Delete Operation:**
```javascript
async confirmDelete() {
    this.deleteModal.isDeleting = true;
    
    try {
        // ✅ Use the correct ID - check what the API/stored procedure expects
        // Some entities use recId, others use specific IDs like poLineId
        const response = await fetch(`/api/{Entity}/${this.deleteModal.line.{entityId}}`, {
            method: 'DELETE',
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        });

        if (response.ok) {
            // ✅ Success - update local state
            this.lines = this.lines.filter(l => l.recId !== this.deleteModal.line.recId);
            this.deleteModal.show = false;
            this.deleteModal.line = null;
        } else {
            const errorMessage = await this.parseErrorResponse(response);
            console.error('Delete line error:', errorMessage);
            alert('Error deleting line item:\n\n' + errorMessage);
        }
    } catch (error) {
        console.error('Delete line failed:', error);
        alert('An error occurred while deleting the line item:\n\n' + error.message);
    } finally {
        this.deleteModal.isDeleting = false;
    }
}
```

### 11.3 Error Message Best Practices

**✅ Inline Validation (Preferred for Forms):**
- Use `lineModal.errors = {}` object for field-level validation
- Display errors inline below each field
- No UI blocking - user can see all errors at once
- Clear errors when opening modal and at start of validation
- Use `else-if` for related field validations (e.g., qty > 0 AND qty <= outstanding)

**✅ Alert with Parsed Errors (For Server Operations):**
- Always parse server responses for detailed validation errors
- Include context in error message (what operation failed)
- Log full error details to console for debugging
- Format multiple errors as bullet points
- Show specific field-level errors when available

**❌ Avoid:**
```javascript
// Don't check result.isSuccess - it's private
if (response.ok) {
    const result = await response.json();
    if (result.isSuccess && result.data) {  // ❌ isSuccess is private
        // ...
    }
}

// Don't use alert() for form validation - use inline errors instead
if (!this.lineModal.data.qty) {
    alert('Quantity is required');  // ❌ Blocks UI, poor UX
    return false;
}
    return false;
}

// Don't use generic messages without parsing server responses
alert('Failed to save line item. Please try again.');  // ❌ No details

// Don't ignore server response content
catch (error) {
    alert('An error occurred.');  // ❌ Loses error details
}

// Don't lose error context
alert(error);  // ❌ Shows "[object Object]"
```

**✅ Correct:**
```javascript
// For form validation - use inline errors
validateLine() {
    this.lineModal.errors = {};
    
    if (!this.lineModal.data.qty) {
        this.lineModal.errors.qty = 'Quantity is required';  // ✅ Inline error
    }
    
    return Object.keys(this.lineModal.errors).length === 0;
}

// For server errors - parse the response for detailed errors
const errorMessage = await this.parseErrorResponse(response);
console.error('Save line error:', errorMessage);
alert('Error saving line item:\n\n' + errorMessage);

// For exceptions, show the error message
catch (error) {
    console.error('Save line failed:', error);
    alert('An error occurred while saving the line item:\n\n' + error.message);
}
```

---

