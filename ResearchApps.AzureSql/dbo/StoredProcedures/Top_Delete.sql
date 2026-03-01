CREATE PROCEDURE [dbo].[Top_Delete]
@TopId int = 1,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	DELETE
	FROM [Top]
	WHERE TopId = @TopId
END
GO

