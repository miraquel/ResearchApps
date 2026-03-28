CREATE PROCEDURE [dbo].[Ps_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PsId nvarchar(20), @PsLineId int;

	BEGIN TRY
		IF NOT EXISTS (SELECT 1 FROM Ps WHERE RecId = @RecId)
		BEGIN
			THROW 50001, 'Stock Adjustment not found.', 1;
		END;

		SELECT @PsId = PsId FROM Ps WHERE RecId = @RecId;

		--* Ps Line *--
		DECLARE x_cursor CURSOR FOR
		SELECT PsLineId
		FROM [PsLine]
		WHERE PsId = @PsId;

		OPEN x_cursor;

		FETCH NEXT FROM x_cursor
		INTO @PsLineId;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC [PsLine_Delete] @PsLineId;

			-- Get the next vendor.
			FETCH NEXT FROM x_cursor
			INTO @PsLineId;
		END;
		CLOSE x_cursor;
		DEALLOCATE x_cursor;

		--* Ps Header *--
		DELETE FROM [Ps]
		WHERE RecId = @RecId;

		SELECT @PsId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
