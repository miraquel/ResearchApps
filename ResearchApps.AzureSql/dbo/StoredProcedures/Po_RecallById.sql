CREATE PROCEDURE [dbo].[Po_RecallById]
	@RecId int,
	@ModifiedBy nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
	    IF NOT EXISTS (SELECT 1 FROM Po WHERE RecId = @RecId)
	    BEGIN
	        THROW 50001, 'Purchase Order not found.', 1;
	    END;
        IF NOT EXISTS (SELECT 1 FROM PoLine b JOIN Po a ON b.PoId = a.PoId WHERE a.RecId = @RecId)
        BEGIN
            THROW 50002, 'Purchase Order has no lines.', 1;
        END;
		DECLARE @WfTransId int;
		SELECT @WfTransId = WfTransId FROM Po WHERE RecId = @RecId AND PoStatusId = 4;
		IF @WfTransId IS NOT NULL
		BEGIN
			UPDATE [WfTrans] SET [WfStatusActionId] = 3 WHERE [WfTransId] = @WfTransId;
			UPDATE Po SET PoStatusId = 0 WHERE RecId = @RecId;
		END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END
GO
