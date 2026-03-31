CREATE PROCEDURE [dbo].[Ps_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @PsId nvarchar(20), @PsLineId int;
	DECLARE @LineDeleteResult TABLE (Result nvarchar(500));
	DECLARE @LineDeleteMsg nvarchar(500);

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
			DELETE FROM @LineDeleteResult;
			INSERT INTO @LineDeleteResult EXEC [PsLine_Delete] @PsLineId;
			SELECT TOP 1 @LineDeleteMsg = Result FROM @LineDeleteResult;
			IF LEFT(ISNULL(@LineDeleteMsg, ''), 2) = '-1'
			BEGIN
				CLOSE x_cursor;
				DEALLOCATE x_cursor;
				THROW 50002, 'Gagal menghapus baris: stock tidak mencukupi untuk membalik transaksi.', 1;
			END;

			FETCH NEXT FROM x_cursor
			INTO @PsLineId;
		END;
		CLOSE x_cursor;
		DEALLOCATE x_cursor;

		--* Ps Header *--
		DELETE FROM [Ps]
		WHERE RecId = @RecId;

		SELECT @PsId AS PsId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
