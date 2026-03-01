CREATE PROCEDURE [dbo].[Wf_Update]
@WfId int,
@WfFormId int,
@Index int,
@UserId nvarchar(20)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [Wf]
	SET WfFormId = @WfFormId
		,[Index] = @Index
		,UserId = @UserId
	WHERE [WfId] = @WfId

	SELECT a.[WfId]
		,a.[WfFormId]
		,a.[Index]
		,a.[UserId]
	FROM [Wf] a
	WHERE a.WfId = @WfId
END
GO

