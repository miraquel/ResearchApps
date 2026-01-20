CREATE PROCEDURE [dbo].[Ps_Update]
@RecId int, 
@Notes nvarchar(100)='', 
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN	
	UPDATE [Ps]
	SET Notes = @Notes
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE RecId = @RecId
END

GO

