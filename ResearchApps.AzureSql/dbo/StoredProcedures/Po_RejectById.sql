CREATE PROCEDURE [dbo].[Po_RejectById]
	@RecId int,
	@Notes nvarchar(50)='',
	@ModifiedBy nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @RefId nvarchar(20), @Index int;
    DECLARE @NextIndex int, @NextUserId nvarchar(20);
    DECLARE @WfTransId int, @NextWfTransId int;
    BEGIN TRY
	    IF NOT EXISTS (SELECT 1 FROM Po WHERE RecId = @RecId)
	    BEGIN
	        THROW 50001, 'Purchase Order not found.', 1;
	    END;
		SELECT @RefId = a.PoId, @WfTransId = a.WfTransId, @Index = w.[Index]
		FROM Po a JOIN [WfTrans] w ON w.WfTransId = a.WfTransId WHERE a.RecId = @RecId;
        IF @WfTransId IS NULL
        BEGIN
            THROW 50002, 'Purchase Order has no active workflow transaction.', 1;
        END;
		UPDATE WfTrans SET WfStatusActionId = 2, ActionDate = GETDATE(), Notes = @Notes WHERE WfTransId = @WfTransId;
		UPDATE Po SET PoStatusId = 5 WHERE RecId = @RecId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END
GO
