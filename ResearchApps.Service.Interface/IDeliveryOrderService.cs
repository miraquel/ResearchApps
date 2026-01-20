using ResearchApps.Domain;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service.Interface;

public interface IDeliveryOrderService
{
    // Do_Select
    Task<ServiceResponse<PagedListVm<DeliveryOrderHeaderVm>>> DoSelect(PagedListRequestVm request, CancellationToken cancellationToken);
    
    // Do_SelectById
    Task<ServiceResponse<DeliveryOrderHeaderVm>> DoSelectById(int id, CancellationToken cancellationToken);
    
    // Do_Insert
    Task<ServiceResponse<(int RecId, string DoId)>> DoInsert(DeliveryOrderVm deliveryOrderHeader,
        CancellationToken cancellationToken);
    
    // Do_Update
    Task<ServiceResponse> DoUpdate(DeliveryOrderHeaderVm deliveryOrderHeader, CancellationToken cancellationToken);
    
    // Do_Delete
    Task<ServiceResponse> DoDelete(int recId, CancellationToken cancellationToken);
    
    // Outstanding
    Task<ServiceResponse<IEnumerable<DeliveryOrderHeaderOutstandingVm>>> DoHdOsSelect(int customerId, CancellationToken cancellationToken);
    Task<ServiceResponse<IEnumerable<DeliveryOrderOutstandingVm>>> DoOsSelect(int customerId, CancellationToken cancellationToken);
    Task<ServiceResponse<DeliveryOrderOutstandingVm>> DoOsByDoLineId(int doLineId, CancellationToken cancellationToken);
    
    // Lines
    Task<ServiceResponse<IEnumerable<DeliveryOrderLineVm>>> DoLineSelectByDo(int doRecId, CancellationToken cancellationToken);
    Task<ServiceResponse<DeliveryOrderLineVm>> DoLineSelectById(int doLineId, CancellationToken cancellationToken);
    Task<ServiceResponse<string>> DoLineInsert(DeliveryOrderLineVm deliveryOrderLine,
        CancellationToken cancellationToken);
    Task<ServiceResponse> DoLineUpdate(DeliveryOrderLineVm deliveryOrderLine, CancellationToken cancellationToken);
    Task<ServiceResponse> DoLineDelete(int doLineId, CancellationToken cancellationToken);
    
    // Lookups

    // Composite ViewModel
    Task<ServiceResponse<DeliveryOrderVm>> GetDeliveryOrderViewModel(int recId, CancellationToken cancellationToken);
    
    // Export
    Task<IEnumerable<DeliveryOrderHeader>> GetDoExportData(PagedListRequestVm request, CancellationToken cancellationToken);
    
    // Workflow
    Task<ServiceResponse<IEnumerable<WfTransHistoryVm>>> GetWfHistory(string refId, int wfFormId, CancellationToken cancellationToken);
}
