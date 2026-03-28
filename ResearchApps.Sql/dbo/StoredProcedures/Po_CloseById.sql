CREATE PROCEDURE [dbo].[Po_CloseById]
	@PoId nvarchar(20),
	@ModifiedBy nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
	    IF NOT EXISTS (SELECT 1 FROM Po WHERE PoId = @PoId)
	    BEGIN
	        THROW 50001, 'Purchase Order not found.', 1;
	    END;
		UPDATE Po
		SET PoStatusId = 3, ModifiedBy = @ModifiedBy, ModifiedDate = GETDATE()
		WHERE PoId = @PoId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END
GO
