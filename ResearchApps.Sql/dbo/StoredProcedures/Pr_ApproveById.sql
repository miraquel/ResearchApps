--EXEC [Pr_ApproveById] 3,'ok','widi'
CREATE PROCEDURE [dbo].[Pr_ApproveById]
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
        IF NOT EXISTS (SELECT 1 FROM Pr WHERE RecId = @RecId)
            BEGIN
                THROW 50001, 'PR not found.', 1;
            END;

        --Cari next approval
        SELECT @RefId = a.PrId
             ,@WfTransId = a.WfTransId
             ,@Index = w.[Index]
        FROM Pr a
                 JOIN [WfTrans] w ON w.WfTransId = a.WfTransId
        WHERE a.RecId = @RecId;

        IF @WfTransId IS NULL
            BEGIN
                THROW 50002, 'PR has no active workflow transaction.', 1;
            END;

        IF EXISTS (SELECT 1 FROM Wf WHERE WfFormId = 1 AND [Index] = @Index+1)
            --Jika masih ada next aproval
            BEGIN
                SELECT @NextIndex = [Index], @NextUserId = [UserId] FROM Wf WHERE WfFormId = 1 AND [Index] = @Index+1; --FormPr

                INSERT INTO WfTrans (WfId, [WfFormId], [RefId], [Index], [UserId], [WfStatusActionId], [ActionDate], [CreatedDate], [Notes])
                SELECT [WfId], [WfFormId], @RefId, [Index], [UserId], 0, '1900-01-01', GETDATE(), ''
                FROM [Wf]
                WHERE [WfFormId] = 1 --FormPr
                  AND [Index] = @NextIndex;

                SET @NextWfTransId = SCOPE_IDENTITY();

                UPDATE WfTrans
                SET WfStatusActionId = 1, ActionDate = GETDATE()
                  ,Notes = @Notes
                WHERE WfTransId = @WfTransId;

                UPDATE Pr
                SET PrStatusId = 4
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

                UPDATE Pr
                SET PrStatusId = 1
                WHERE RecId = @RecId;
            END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END

GO

