CREATE PROCEDURE [dbo].[Bpb_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @BpbId nvarchar(20), @BpbLineId int;

	BEGIN TRY
		IF NOT EXISTS (SELECT 1 FROM Bpb WHERE RecId = @RecId)
		BEGIN
			THROW 50001, 'BPB not found.', 1;
		END;

		SELECT @BpbId = BpbId FROM Bpb WHERE RecId = @RecId;

		--* Bpb Line *--
		DECLARE x_cursor CURSOR FOR
		SELECT BpbLineId
		FROM [BpbLine]
		WHERE BpbId = @BpbId;

		OPEN x_cursor;

		FETCH NEXT FROM x_cursor
		INTO @BpbLineId;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC [BpbLine_Delete] @BpbLineId;

			-- Get the next vendor.
			FETCH NEXT FROM x_cursor
			INTO @BpbLineId;
		END;
		CLOSE x_cursor;
		DEALLOCATE x_cursor;

		--* Bpb Header *--
		DELETE FROM [Bpb]
		WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
