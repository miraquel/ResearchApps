CREATE PROCEDURE [dbo].[Po_CloseById]
	@PoId nvarchar(20),
	@ModifiedBy nvarchar(20)
AS
BEGIN	
	UPDATE Po
	SET PoStatusId = 3
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE PoId = @PoId
END

GO

