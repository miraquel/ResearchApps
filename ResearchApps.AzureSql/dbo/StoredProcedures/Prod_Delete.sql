CREATE PROCEDURE [dbo].[Prod_Delete]
@RecId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
		IF NOT EXISTS (SELECT 1 FROM Prod WHERE RecId = @RecId)
		BEGIN
			THROW 50001, 'Production not found.', 1;
		END;

		--* Prod Header *--
		DELETE FROM [Prod]
		WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
