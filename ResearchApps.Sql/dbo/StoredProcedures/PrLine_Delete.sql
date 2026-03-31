CREATE PROCEDURE [dbo].[PrLine_Delete]
@PrLineId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PrId nvarchar(20), @ItemName nvarchar(100), @UnitId int, @Total decimal(18,2);

	BEGIN TRY
		SELECT @PrId = PrId FROM PrLine WHERE PrLineId = @PrLineId;

		--* Pr Line *--
		DELETE FROM [PrLine]
		WHERE PrLineId = @PrLineId;

		--* Pr Header *--
		SELECT @Total = SUM(Qty * Price)
		FROM PrLine
		WHERE PrId = @PrId;

		UPDATE [Pr]
		SET Total = isnull(@Total,0)
		WHERE PrId = @PrId;

		SELECT @PrId AS PrId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
