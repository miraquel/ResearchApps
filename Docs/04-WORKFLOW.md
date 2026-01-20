# Workflow System

## Overview

Purchase Requests (PR) and Customer Orders (CO) use multi-level approval workflows with:
- Sequential approver chain via `WfTrans` table
- `CurrentApprover` field tracking next approver
- User-based (not permission-based) authorization for workflow actions

## Workflow States

```
┌─────────┐    Submit    ┌──────────────┐   Approve   ┌──────────┐
│  Draft  │─────────────→│   Pending    │────────────→│ Approved │
│  (0)    │              │  Approval(4) │             │   (5)    │
└─────────┘              └──────────────┘             └──────────┘
     ↑                         │  │
     │         Recall          │  │  Reject
     └─────────────────────────┘  │
                                  ↓
                           ┌──────────┐
                           │ Rejected │
                           │   (6)    │
                           └──────────┘
```

## User-Based Authorization (Critical Pattern)

Workflow buttons use **user-based** checks, not permissions:

```cshtml
@* In Views/Prs/Details.cshtml *@
@{
    var currentUserName = User.Identity?.Name ?? "";
    var isCreator = Model.CreatedBy == currentUserName;
    var isCurrentApprover = !string.IsNullOrEmpty(Model.CurrentApprover) 
        && Model.CurrentApprover == currentUserName;
}

@* Submit - only creator in Draft status *@
@if (Model.PrStatusId == 0 && isCreator)
{
    <button type="submit">Submit</button>
}

@* Approve/Reject - only current approver in Pending status *@
@if (Model.PrStatusId == 4 && isCurrentApprover)
{
    <button>Approve</button>
    <button>Reject</button>
}

@* Recall - only creator can recall pending PR *@
@if (Model.PrStatusId == 4 && isCreator)
{
    <button>Recall</button>
}
```

**CRUD operations** still use permission-based authorization:
```csharp
[Authorize(PermissionConstants.Prs.Edit)]
public async Task<IActionResult> Edit(int id) { ... }
```

## Controller Workflow Actions

```csharp
// In CustomerOrdersController.cs
[HttpPost]
public async Task<IActionResult> Approve(int id)
{
    var coBefore = await _customerOrderService.CoSelectById(id, ct);
    
    // Verify current user is the approver
    if (coBefore.Data?.CurrentApprover != User.Identity?.Name)
    {
        return Forbid();  // Not the current approver
    }
    
    await _customerOrderService.CoApproveById(action, ct);
    
    // Send notification to next approver
    var coAfter = await _customerOrderService.CoSelectById(id, ct);
    await _coNotificationService.NotifyCoApproved(
        coAfter.Data?.CoId,
        id,
        User.Identity?.Name,
        coAfter.Data?.CurrentApprover);
}
```

## Key Workflow Fields

| Field | Purpose |
|-------|---------|
| `CurrentApprover` | Username of person who should approve next |
| `CurrentIndex` | Position in approval chain |
| `WfTransId` | Links to workflow transaction record |
| `CreatedBy` | Original creator (can submit/recall) |

## Workflow Service Methods

```csharp
public interface IPrService
{
    // Submit PR for approval (moves to Pending)
    Task<ServiceResponse<PrVm>> PrSubmitById(int id, CancellationToken ct);
    
    // Approve PR (moves to next approver or Approved)
    Task<ServiceResponse> PrApproveById(PrWorkflowActionVm action, CancellationToken ct);
    
    // Reject PR (moves to Rejected)
    Task<ServiceResponse> PrRejectById(PrWorkflowActionVm action, CancellationToken ct);
    
    // Recall PR (moves back to Draft)
    Task<ServiceResponse> PrRecallById(int id, CancellationToken ct);
}
```

## Workflow Action ViewModel

```csharp
public class PrWorkflowActionVm
{
    public int RecId { get; set; }
    public string? Notes { get; set; }  // Optional reason/comments
}
```

## Notification Integration

Workflow actions trigger real-time notifications via SignalR:

```csharp
// After approval
await _prNotificationService.NotifyPrApproved(
    prId: pr.PrId,
    recId: pr.RecId,
    approvedBy: currentUser,
    nextApprover: pr.CurrentApprover,
    isFullyApproved: pr.PrStatusId == 5);
```

See [05-SIGNALR.md](./05-SIGNALR.md) for notification details.
