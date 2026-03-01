CREATE PROCEDURE [dbo].[WfForm_SelectById]
@WfFormId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT a.[WfFormId]
		,a.[FormName]
		,a.[Initial]
	FROM [WfForm] a
	WHERE a.WfFormId = @WfFormId
END
GO

