CREATE PROCEDURE [dbo].[Po_ApproveById]
    @RecId int,
    @Notes nvarchar(50)='',
    @ModifiedBy nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @RefId nvarchar(20), @Index int;
    DECLARE @NextIndex int, @NextUserId nvarchar(20);
    DECLARE @WfTransId int, @NextWfTransId int, @WfFormId int;

    BEGIN TRY
        SET @WfFormId = 3; --FormPo

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

        IF EXISTS (SELECT 1 FROM Wf WHERE WfFormId = @WfFormId AND [Index] = @Index+1)
            BEGIN
                SELECT @NextIndex = [Index], @NextUserId = [UserId] FROM Wf WHERE WfFormId = @WfFormId AND [Index] = @Index+1;
                INSERT INTO WfTrans (WfId, [WfFormId], [RefId], [Index], [UserId], [WfStatusActionId], [ActionDate], [CreatedDate], [Notes])
                SELECT [WfId], [WfFormId], @RefId, [Index], [UserId], 0, '1900-01-01', GETDATE(), ''
                FROM [Wf] WHERE [WfFormId] = @WfFormId AND [Index] = @NextIndex;
                SET @NextWfTransId = SCOPE_IDENTITY();
                UPDATE WfTrans SET WfStatusActionId = 1, ActionDate = GETDATE(), Notes = @Notes WHERE WfTransId = @WfTransId;
                UPDATE Po SET PoStatusId = 4, [WfTransId] = @NextWfTransId WHERE RecId = @RecId;
            END
        ELSE
            BEGIN
                UPDATE WfTrans SET WfStatusActionId = 1, ActionDate = GETDATE(), Notes = @Notes WHERE WfTransId = @WfTransId;
                UPDATE Po SET PoStatusId = 1 WHERE RecId = @RecId;
            END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END
GO
