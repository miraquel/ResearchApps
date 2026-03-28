CREATE PROCEDURE [dbo].[Php_Update]
@RecId int,
@Notes nvarchar(100),
@ModifiedBy nvarchar(20)
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY
		UPDATE [Php]
		SET Notes = @Notes
			, ModifiedBy = @ModifiedBy
			, ModifiedDate = GETDATE()
		WHERE RecId = @RecId;
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH;
END

GO
