CREATE PROCEDURE [dbo].[Wf_SelectById]
@WfId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT a.[WfId]
		,a.[WfFormId]
		,a.[Index]
		,a.[UserId]
	FROM [Wf] a
	WHERE a.WfId = @WfId
END
GO
