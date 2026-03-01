CREATE PROCEDURE [dbo].[Wf_Insert]
@WfFormId int,
@Index int,
@UserId nvarchar(20)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @WfId int

	INSERT INTO [Wf]
	([WfFormId], [Index], [UserId])
	VALUES
	(@WfFormId, @Index, @UserId)

	SET @WfId = SCOPE_IDENTITY()

	SELECT a.[WfId]
		,a.[WfFormId]
		,a.[Index]
		,a.[UserId]
	FROM [Wf] a
	WHERE a.WfId = @WfId
END
GO
