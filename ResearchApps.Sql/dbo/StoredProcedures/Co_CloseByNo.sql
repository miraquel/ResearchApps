CREATE PROCEDURE [dbo].[Co_CloseByNo]
	@CoId nvarchar(20),
	@ModifiedBy nvarchar(20)
AS
BEGIN	
	UPDATE Co
	SET CoStatusId = 2
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE CoId = @CoId
END

GO

