CREATE PROCEDURE [dbo].[Do_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @DoId nvarchar(20), @DoLineId int;

	BEGIN TRY
		IF NOT EXISTS (SELECT 1 FROM Do WHERE RecId = @RecId)
		BEGIN
			THROW 50001, 'Delivery Order not found.', 1;
		END;

		SELECT @DoId = DoId FROM Do WHERE RecId = @RecId;

		--* Do Line *--
		DECLARE x_cursor CURSOR FOR
		SELECT DoLineId
		FROM [DoLine]
		WHERE DoId = @DoId;

		OPEN x_cursor;

		FETCH NEXT FROM x_cursor
		INTO @DoLineId;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC [DoLine_Delete] @DoLineId;

			-- Get the next vendor.
			FETCH NEXT FROM x_cursor
			INTO @DoLineId;
		END;
		CLOSE x_cursor;
		DEALLOCATE x_cursor;

		--* Do Header *--
		DELETE FROM [Do]
		WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
