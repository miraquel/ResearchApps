CREATE PROCEDURE [dbo].[Gr_Update]
@RecId int, 
@GrDate date, 
@RefNo nvarchar(100),
@Notes nvarchar(100), 
@ModifiedBy nvarchar(20)
AS
BEGIN
	UPDATE [Gr]
	SET GrDate = @GrDate
		, RefNo = @RefNo
		, Notes = @Notes
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE RecId = @RecId
END

GO

