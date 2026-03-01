CREATE PROCEDURE [dbo].[ItemGroup01_Delete]
@ItemGroup01Id int = 1,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	DELETE
	FROM [ItemGroup01]
	WHERE ItemGroup01Id = @ItemGroup01Id
END
GO

