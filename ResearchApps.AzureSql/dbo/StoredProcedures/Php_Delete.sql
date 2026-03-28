CREATE PROCEDURE [dbo].[Php_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PhpId nvarchar(20), @PhpLineId int;

	BEGIN TRY
		IF NOT EXISTS (SELECT 1 FROM Php WHERE RecId = @RecId)
		BEGIN
			THROW 50001, 'Production Output not found.', 1;
		END;

		SELECT @PhpId = PhpId FROM Php WHERE RecId = @RecId;

		--* Php Line *--
		DECLARE x_cursor CURSOR FOR
		SELECT PhpLineId
		FROM [PhpLine]
		WHERE PhpId = @PhpId;

		OPEN x_cursor;

		FETCH NEXT FROM x_cursor
		INTO @PhpLineId;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC [PhpLine_Delete] @PhpLineId;

			-- Get the next vendor.
			FETCH NEXT FROM x_cursor
			INTO @PhpLineId;
		END;
		CLOSE x_cursor;
		DEALLOCATE x_cursor;

		--* Php Header *--
		DELETE FROM [Php]
		WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
