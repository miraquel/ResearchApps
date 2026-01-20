using ResearchApps.Domain;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface ICustomerOrderService
{
    // Co_Select
    Task<ServiceResponse<PagedListVm<CustomerOrderHeaderVm>>> CoSelect(PagedListRequestVm request, CancellationToken cancellationToken);
    
    // Co_SelectById
    Task<ServiceResponse<CustomerOrderHeaderVm>> CoSelectById(int id, CancellationToken cancellationToken);
    
    // Co_Insert
    Task<ServiceResponse<(int RecId, string CoId)>> CoInsert(CustomerOrderVm customerOrderHeader,
        CancellationToken cancellationToken);
    
    // Co_Update
    Task<ServiceResponse> CoUpdate(CustomerOrderHeaderVm customerOrderHeader, CancellationToken cancellationToken);
    
    // Co_Delete
    Task<ServiceResponse> CoDelete(int recId, CancellationToken cancellationToken);
    
    // Workflow Actions
    Task<ServiceResponse> CoSubmitById(CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken);
    Task<ServiceResponse> CoRecallById(CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken);
    Task<ServiceResponse> CoRejectById(CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken);
    Task<ServiceResponse> CoCloseByNo(CustomerOrderWorkflowActionVm coId, CancellationToken cancellationToken);
    Task<ServiceResponse> CoApproveById(CustomerOrderWorkflowActionVm action, CancellationToken cancellationToken);
    
    // Outstanding
    Task<ServiceResponse<IEnumerable<CustomerOrderHeaderOutstandingVm>>> CoHdOsSelect(int customerId, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<CustomerOrderOutstandingVm>>> CoOsSelect(int customerId, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<CustomerOrderOutstandingVm>>> CoOsById(int coRecId, CancellationToken cancellationToken);
    Task<ServiceResponse<CustomerOrderOutstandingVm>> CoOsByCoLineId(int coLineId, CancellationToken cancellationToken);
    
    // Lines
    Task<ServiceResponse<IEnumerable<CustomerOrderLineVm>>> CoLineSelectByCo(int coRecId, CancellationToken cancellationToken);
    Task<ServiceResponse<CustomerOrderLineVm>> CoLineSelectById(int coLineId, CancellationToken cancellationToken);
    Task<ServiceResponse<string>> CoLineInsert(CustomerOrderLineVm customerOrderLine,
        CancellationToken cancellationToken);
    Task<ServiceResponse> CoLineUpdate(CustomerOrderLineVm customerOrderLine, CancellationToken cancellationToken);
    Task<ServiceResponse> CoLineDelete(int coLineId, CancellationToken cancellationToken);
    
    // Lookups
    Task<ServiceResponse<IEnumerable<CustomerOrderTypeVm>>> CoTypeCbo(CancellationToken cancellationToken);

    // Composite ViewModel
    Task<ServiceResponse<CustomerOrderVm>> GetCustomerOrder(int recId, CancellationToken cancellationToken);
    
    // Reports
    Task<IEnumerable<CoSummaryReportItem>> GetCoSummaryReportData(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    Task<IEnumerable<CoDetailReportItem>> GetCoDetailReportData(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    
    // Export
    Task<IEnumerable<CustomerOrderHeader>> GetCoExportData(PagedListRequestVm request, CancellationToken cancellationToken);
    
    // Workflow
    Task<ServiceResponse<IEnumerable<WfTransHistoryVm>>> GetWfHistory(string refId, int wfFormId, CancellationToken cancellationToken);
}
