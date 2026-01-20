CREATE PROCEDURE [dbo].[Bpb_Update]
@RecId int, 
@Notes nvarchar(200)='', 
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN	
	UPDATE [Bpb]
	SET Notes = @Notes
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE RecId = @RecId
END

GO

