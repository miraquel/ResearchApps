CREATE PROCEDURE [dbo].[SiLine_Update]
@SiLineId int, 
@DoLineId int,
@DoId nvarchar(20),
@ItemId int, 
@Qty numeric(32,16) = 0,
@Price numeric(32,16),
@Notes nvarchar(100)='', 
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	UPDATE [SiLine]
	SET [DoLineId] = @DoLineId
		,[DoId] = @DoId
		,[ItemId] = @ItemId
		,[Qty] = @Qty
		,[Price] = @Price
		,[Notes] = @Notes
		,[ModifiedDate] = GETDATE()
		,[ModifiedBy] = @ModifiedBy
	WHERE SiLineId = @SiLineId
END
GO

