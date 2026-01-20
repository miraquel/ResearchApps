CREATE PROCEDURE [dbo].[Co_Workflow]
    @RecId INT = 29
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @RefId nvarchar(20)

	SELECT @RefId=CoId FROM Co WHERE RecId = @RecId

    SELECT a.[Index], a.UserId, a.WfStatusActionId, s.WfStatusActionName, a.ActionDate, a.Notes 
	FROM [WfTrans] a
	JOIN [WfStatusAction] s ON s.WfStatusActionId = a.WfStatusActionId
	WHERE WfFormId = 2 AND RefId = @RefId
		AND a.WfStatusActionId <> 3
	ORDER BY [Index]

END

GO

