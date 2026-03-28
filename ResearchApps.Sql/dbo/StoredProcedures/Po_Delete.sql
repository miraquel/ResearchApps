CREATE PROCEDURE [dbo].[Po_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PoId nvarchar(20);

	BEGIN TRY
		IF NOT EXISTS (SELECT 1 FROM Po WHERE RecId = @RecId)
		BEGIN
			THROW 50001, 'Purchase Order not found.', 1;
		END;

		IF (SELECT PoStatusId FROM Po WHERE RecId = @RecId) <> 0
		BEGIN
			THROW 50002, 'Only Draft Purchase Orders can be deleted.', 1;
		END;

		SELECT @PoId = PoId FROM Po WHERE RecId = @RecId;

		--* Po Line *--
		DELETE FROM [PoLine]
		WHERE PoId = @PoId;

		--* Po Header *--
		DELETE FROM [Po]
		WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
