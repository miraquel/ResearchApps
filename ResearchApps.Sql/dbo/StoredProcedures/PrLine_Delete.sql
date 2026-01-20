CREATE PROCEDURE [dbo].[PrLine_Delete]
@PrLineId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @PrId nvarchar(20), @ItemName nvarchar(100), @UnitId int, @Total decimal(18,2)

	SELECT @PrId = PrId FROM PrLine WHERE PrLineId = @PrLineId

	--* Pr Line *--
	DELETE FROM [PrLine]
	WHERE PrLineId = @PrLineId

	--* Pr Header *--
	SELECT @Total = SUM(Qty * Price)
	FROM PrLine
	WHERE PrId = @PrId

	UPDATE [Pr]
	SET Total = isnull(@Total,0)
	WHERE PrId = @PrId
	
	SELECT @PrId
END

GO

