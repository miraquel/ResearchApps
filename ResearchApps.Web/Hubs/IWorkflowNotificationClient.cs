namespace ResearchApps.Web.Hubs;

public interface IWorkflowNotificationClient
{
    Task ReceivePendingApproval(WorkflowNotification notification);
    Task ReceiveRejected(WorkflowNotification notification);
    Task ReceiveNotification(GenericNotification notification);
    Task EntityStatusChanged(WorkflowNotification notification);
}
