CREATE PROCEDURE [dbo].[Mc_Update]
@RecId int, 
@Notes nvarchar(100), 
@ModifiedBy nvarchar(20)
AS
BEGIN	
	UPDATE [Mc]
	SET Notes = @Notes
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE RecId = @RecId
END

GO

