CREATE PROCEDURE [dbo].[StatusCbo]
@Id int = null,
@Term nvarchar(max) = null
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		[StatusId],
		[StatusName]
	FROM [dbo].[Status]
	WHERE	1 = CASE
				WHEN @Id IS NULL THEN 1
				WHEN StatusId = @Id THEN 1
				ELSE 0
			END AND
			1 = CASE 
				WHEN @Term IS NULL THEN 1
				WHEN StatusName LIKE @Term THEN 1
				ELSE 0
			END
END

GO

