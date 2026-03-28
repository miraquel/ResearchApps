--EXEC [Co_SubmitById] 8, ''
CREATE PROCEDURE [dbo].[Co_SubmitById]
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
            THROW 50002, 'Customer Order has no lines and cannot be submitted.', 1;
        END;

		DECLARE @WfTransId int, @RefId nvarchar(20), @UserId nvarchar(20), @WfFormId int;
		SET @WfFormId = 2; --FormCo
	
		--Cari nomor dok yang mau diapprove
		SELECT @RefId = CoId FROM Co WHERE RecId = @RecId;

		--Cari user yang harus mengaprove
		IF EXISTS (SELECT 1 FROM [Wf] WHERE [WfFormId] = @WfFormId AND [Index] = 1)
		BEGIN
			SELECT @UserId = [UserId]
			FROM [Wf]
			WHERE [WfFormId] = @WfFormId 
				AND [Index] = 1;
	
			INSERT INTO WfTrans ([WfId],[WfFormId],[RefId],[Index], [UserId], [WfStatusActionId], [ActionDate], [CreatedDate], [Notes])
			SELECT WfId, @WfFormId, @RefId, [Index], [UserId], 0, '1900-01-01', GETDATE(), ''
				FROM [Wf]
				WHERE [WfFormId] = @WfFormId
					AND [Index] = 1;
						
			SET @WfTransId = SCOPE_IDENTITY();
					
			UPDATE Co
			SET CoStatusId = 4
				,[WfTransId] = @WfTransId
			WHERE RecId = @RecId;
		END
		ELSE
		BEGIN -- tidak ada workflow
			UPDATE Co
			SET CoStatusId = 1
				,[WfTransId] = 0
			WHERE RecId = @RecId;
		END
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END

GO

