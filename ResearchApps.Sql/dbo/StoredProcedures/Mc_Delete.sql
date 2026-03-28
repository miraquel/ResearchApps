CREATE PROCEDURE [dbo].[Mc_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @McId nvarchar(20), @McLineId int;

	BEGIN TRY
		IF NOT EXISTS (SELECT 1 FROM Mc WHERE RecId = @RecId)
		BEGIN
			THROW 50001, 'Material Customer not found.', 1;
		END;

		SELECT @McId = McId FROM Mc WHERE RecId = @RecId;

		--* Mc Line *--
		DECLARE x_cursor CURSOR FOR
		SELECT McLineId
		FROM [McLine]
		WHERE McId = @McId;

		OPEN x_cursor;

		FETCH NEXT FROM x_cursor
		INTO @McLineId;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC [McLine_Delete] @McLineId;

			-- Get the next vendor.
			FETCH NEXT FROM x_cursor
			INTO @McLineId;
		END;
		CLOSE x_cursor;
		DEALLOCATE x_cursor;

		--* Mc Header *--
		DELETE FROM [Mc]
		WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
