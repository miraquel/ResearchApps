CREATE PROCEDURE [dbo].[Pr_CloseById]
	@PrId nvarchar(20),
	@ModifiedBy nvarchar(20)
AS
BEGIN	
	UPDATE Pr
	SET PrStatusId = 3
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE PrId = @PrId
END

GO

