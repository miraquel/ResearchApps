CREATE PROCEDURE [dbo].[Si_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @SiId nvarchar(20);

	BEGIN TRY
		IF NOT EXISTS (SELECT 1 FROM Si WHERE RecId = @RecId)
		BEGIN
			THROW 50001, 'Sales Invoice not found.', 1;
		END;

		SELECT @SiId = SiId FROM Si WHERE RecId = @RecId;

		--* Si Line *--
		DELETE FROM [SiLine] WHERE SiId = @SiId;

		--* Si Header *--
		DELETE FROM [Si] WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
