CREATE PROCEDURE [dbo].[WfForm_Cbo]
@Id int = null,
@Term nvarchar(max) = null
AS
BEGIN
	SET NOCOUNT ON;

	SELECT a.[WfFormId]
		,a.[FormName]
		,a.[Initial]
	FROM [WfForm] a
	WHERE 1 = CASE WHEN @Id IS NULL THEN 1
			WHEN a.WfFormId = @Id THEN 1
			ELSE 0
			END AND
		1 = CASE WHEN @Term IS NULL THEN 1
			WHEN a.FormName LIKE '%' + @Term + '%' THEN 1
			WHEN a.Initial LIKE '%' + @Term + '%' THEN 1
			ELSE 0
			END
END
GO
