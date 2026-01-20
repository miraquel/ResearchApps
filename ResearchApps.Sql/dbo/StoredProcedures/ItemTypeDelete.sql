CREATE PROCEDURE [dbo].[ItemTypeDelete]
@ItemTypeId int = 1,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	DELETE
	FROM [ItemType]
	WHERE ItemTypeId = @ItemTypeId
END

GO

