CREATE PROCEDURE [dbo].[Item_Delete]
@ItemId int = 1,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	DELETE
	FROM [Item]
	WHERE ItemId = @ItemId
END
GO

