CREATE PROCEDURE [dbo].[ItemGroup02_Delete]
@ItemGroup02Id int = 1,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	DELETE
	FROM [ItemGroup02]
	WHERE ItemGroup02Id = @ItemGroup02Id
END
GO

