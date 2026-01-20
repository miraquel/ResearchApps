CREATE PROCEDURE [dbo].[WfTrans_SelectByRefId]
    @RefId NVARCHAR(20),
    @WfFormId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        wt.WfTransId,
        wt.WfId,
        wt.WfFormId,
        wf.FormName,
        wt.RefId,
        wt.[Index],
        wt.UserId,
        wsa.WfStatusActionId,
        wsa.WfStatusActionName,
        wt.ActionDate,
        wt.CreatedDate,
        wt.Notes
    FROM WfTrans wt
    INNER JOIN WfStatusAction wsa ON wsa.WfStatusActionId = wt.WfStatusActionId
    INNER JOIN WfForm wf ON wf.WfFormId = wt.WfFormId
    WHERE wt.RefId = @RefId 
        AND wt.WfFormId = @WfFormId
        --AND wt.ActionDate > '1900-01-01' -- Only show records that have been actioned
    ORDER BY wt.[Index], wt.ActionDate DESC
END

GO

