CREATE PROCEDURE [dbo].[Co_ApproveById]
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
        SET @WfFormId = 2; --Form Co

        IF NOT EXISTS (SELECT 1 FROM Co WHERE RecId = @RecId)
            BEGIN
                THROW 50001, 'Customer Order not found.', 1;
            END;

        --Cari next approval
        SELECT @RefId = a.CoId, @WfTransId = a.WfTransId, @Index = w.[Index]
        FROM Co a
                 JOIN [WfTrans] w ON w.WfTransId = a.WfTransId
        WHERE a.RecId = @RecId;

        IF @WfTransId IS NULL
            BEGIN
                THROW 50002, 'Customer Order has no active workflow transaction.', 1;
            END;

        IF EXISTS (SELECT 1 FROM Wf WHERE WfFormId = @WfFormId AND [Index] = @Index+1)
            --Jika masih ada next aproval
            BEGIN
                SELECT @NextIndex = [Index], @NextUserId = [UserId] FROM Wf WHERE WfFormId = @WfFormId AND [Index] = @Index+1;

                SELECT @NextWfTransId = MAX(WfTransId) + 1 FROM WfTrans;

                IF @NextWfTransId IS NULL
                    SET @NextWfTransId = 1;

                INSERT INTO WfTrans (WfTransId, [WfFormId], [RefId], [Index], [UserId], [WfStatusActionId], [ActionDate], [CreatedDate], [Notes])
                SELECT @NextWfTransId, [WfFormId], @RefId, [Index], [UserId], 0, '1900-01-01', GETDATE(), ''
                FROM [Wf]
                WHERE [WfFormId] = @WfFormId
                  AND [Index] = @NextIndex;

                UPDATE WfTrans
                SET WfStatusActionId = 1, ActionDate = GETDATE()
                WHERE WfTransId = @WfTransId;

                UPDATE Co
                SET CoStatusId = 4
                  ,[WfTransId] = @NextWfTransId
                WHERE RecId = @RecId;
            END
        ELSE
            BEGIN
                --Jika tidak ada lagi next aproval
                UPDATE WfTrans
                SET WfStatusActionId = 1, ActionDate = GETDATE()
                  , Notes = @Notes
                WHERE WfTransId = @WfTransId;

                UPDATE Co
                SET CoStatusId = 1
                WHERE RecId = @RecId;
            END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END

GO

