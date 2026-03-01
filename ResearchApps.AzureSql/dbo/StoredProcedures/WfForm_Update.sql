CREATE PROCEDURE [dbo].[WfForm_Update]
@WfFormId int,
@FormName nvarchar(50),
@Initial nvarchar(20)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [WfForm]
	SET FormName = @FormName
		,Initial = @Initial
	WHERE [WfFormId] = @WfFormId

	SELECT a.[WfFormId]
		,a.[FormName]
		,a.[Initial]
	FROM [WfForm] a
	WHERE a.WfFormId = @WfFormId
END
GO

