CREATE PROCEDURE [dbo].[Po_SubmitById]
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
            THROW 50002, 'Purchase Order has no lines and cannot be submitted.', 1;
        END;

		DECLARE @WfTransId int, @RefId nvarchar(20), @UserId nvarchar(20), @WfFormId int;
		SET @WfFormId = 3; --FormPo
		SELECT @RefId = PoId FROM Po WHERE RecId = @RecId;
		IF EXISTS (SELECT 1 FROM [Wf] WHERE [WfFormId] = @WfFormId AND [Index] = 1)
		BEGIN
			SELECT @UserId = [UserId] FROM [Wf] WHERE [WfFormId] = @WfFormId AND [Index] = 1;
			INSERT INTO WfTrans ([WfId],[WfFormId],[RefId],[Index], [UserId], [WfStatusActionId], [ActionDate], [CreatedDate], [Notes])
			SELECT WfId, @WfFormId, @RefId, [Index], [UserId], 0, '1900-01-01', GETDATE(), ''
				FROM [Wf] WHERE [WfFormId] = @WfFormId AND [Index] = 1;
			SET @WfTransId = SCOPE_IDENTITY();
			UPDATE Po SET PoStatusId = 4, [WfTransId] = @WfTransId WHERE RecId = @RecId;
		END
		ELSE
		BEGIN
			UPDATE Po SET PoStatusId = 1, [WfTransId] = 0 WHERE RecId = @RecId;
		END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END
GO
