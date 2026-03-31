--EXEC Pr_RecallById 5,'thomas'
CREATE PROCEDURE [dbo].[Pr_RecallById]
    @RecId int,
    @ModifiedBy nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @WfTransId int;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Pr WHERE RecId = @RecId)
            BEGIN
                THROW 50001, 'PR not found.', 1;
            END;

        IF NOT EXISTS (SELECT 1 FROM Pr WHERE RecId = @RecId AND PrStatusId IN (4, 5))
            BEGIN
                THROW 50002, 'Only submitted and rejected PR can be recalled.', 1;
            END;

        SELECT @WfTransId = WfTransId FROM Pr WHERE RecId = @RecId AND PrStatusId = 4;

        UPDATE [WfTrans]
        SET [WfStatusActionId] = 3
        WHERE [WfTransId] = @WfTransId;

        UPDATE Pr
        SET PrStatusId = 0
        WHERE RecId = @RecId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END

GO

