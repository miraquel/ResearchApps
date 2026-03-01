CREATE PROCEDURE [dbo].[WfForm_Insert]
@FormName nvarchar(50),
@Initial nvarchar(20)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @WfFormId int

	INSERT INTO [WfForm]
	([FormName], [Initial])
	VALUES
	(@FormName, @Initial)

	SET @WfFormId = SCOPE_IDENTITY()

	SELECT a.[WfFormId]
		,a.[FormName]
		,a.[Initial]
	FROM [WfForm] a
	WHERE a.WfFormId = @WfFormId
END
GO

