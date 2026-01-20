--EXEC [PrLine_Insert] 1, 1001, '5 Nov 2025', 2, 1500000, 'Kabel Utp', 'system'
CREATE PROCEDURE [dbo].[PrLine_Insert]
@RecId int, 
@ItemId int, 
@RequestDate date,
@Qty numeric(32,16) = 0, 
@Price numeric(32,16) = 0, 
@Notes nvarchar(100)='', 
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @PrId nvarchar(20), @ItemName nvarchar(100), @UnitId int, @Total decimal(18,2)

	SELECT @PrId = PrId FROM Pr WHERE RecId = @RecId
	SELECT @ItemName = ItemName, @UnitId = UnitId FROM Item WHERE ItemId = @ItemId

	--* Pr Line *--
	INSERT INTO [PrLine]
	([PrId], [ItemId], [ItemName], [RequestDate], [Qty], [UnitId], [Price], [Notes]
	  ,[CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy])
	VALUES
	(@PrId, @ItemId, @ItemName, CAST(@RequestDate as DATE), @Qty, @UnitId, @Price, @Notes
	,GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	--* Pr Header *--
	SELECT @Total = SUM(Qty * Price)
	FROM PrLine
	WHERE PrId = @PrId

	UPDATE [Pr]
	SET Total = @Total
	WHERE PrId = @PrId
	

	SELECT @PrId
END

GO

