CREATE PROCEDURE [dbo].[WfForm_Delete]
@WfFormId int
AS
BEGIN
	SET NOCOUNT ON;

	-- Delete associated approval steps first
	DELETE FROM [Wf]
	WHERE WfFormId = @WfFormId

	DELETE FROM [WfForm]
	WHERE WfFormId = @WfFormId
END
GO
