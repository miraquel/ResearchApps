CREATE PROCEDURE [dbo].[Co_RecallById]
	@RecId int,
	@ModifiedBy nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
	    IF NOT EXISTS (SELECT 1 FROM Co WHERE RecId = @RecId)
	    BEGIN
	        THROW 50001, 'Customer Order not found.', 1;
	    END;

        IF NOT EXISTS (SELECT 1 FROM CoLine b JOIN Co a ON b.CoId = a.CoId WHERE a.RecId = @RecId)
        BEGIN
            THROW 50002, 'Customer Order has no lines.', 1;
        END;

		IF EXISTS (SELECT 1 FROM Co WHERE RecId = @RecId AND CoStatusId = 1)
		--Sudah pernah approved sebelumnya
		BEGIN
			UPDATE Co
			SET CoStatusId = 0, Revision = Revision + 1 
			WHERE RecId = @RecId;
		END	
		ELSE
		BEGIN
			UPDATE Co
			SET CoStatusId = 0
			WHERE RecId = @RecId;
		END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END

GO

