CREATE PROCEDURE [dbo].[SalesPrice_Delete]
@RecId int,
@ModifiedBy nvarchar(20)
AS
BEGIN
	DELETE
	FROM [SalesPrice]
	WHERE RecId = @RecId
END
GO

