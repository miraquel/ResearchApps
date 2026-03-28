CREATE PROCEDURE [dbo].[Gr_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @GrId nvarchar(20), @GrLineId int;

	BEGIN TRY
		IF NOT EXISTS (SELECT 1 FROM Gr WHERE RecId = @RecId)
		BEGIN
			THROW 50001, 'Goods Receipt not found.', 1;
		END;

		SELECT @GrId = GrId FROM Gr WHERE RecId = @RecId;

		--* Gr Line *--
		DECLARE x_cursor CURSOR FOR
		SELECT GrLineId
		FROM [GrLine]
		WHERE GrId = @GrId;

		OPEN x_cursor;

		FETCH NEXT FROM x_cursor
		INTO @GrLineId;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC [GrLine_Delete] @GrLineId;

			-- Get the next vendor.
			FETCH NEXT FROM x_cursor
			INTO @GrLineId;
		END;
		CLOSE x_cursor;
		DEALLOCATE x_cursor;

		--* Gr Header *--
		DELETE FROM [Gr]
		WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
