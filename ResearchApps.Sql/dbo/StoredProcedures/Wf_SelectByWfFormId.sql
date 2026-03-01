CREATE PROCEDURE [dbo].[Wf_SelectByWfFormId]
@WfFormId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT a.[WfId]
		,a.[WfFormId]
		,a.[Index]
		,a.[UserId]
	FROM [Wf] a
	WHERE a.WfFormId = @WfFormId
	ORDER BY a.[Index] ASC
END
GO
