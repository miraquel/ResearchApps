--EXEC [Pr_SubmitById] 1, 'admin'
CREATE PROCEDURE [dbo].[Pr_SubmitById]
	@RecId int,
	@ModifiedBy nvarchar(20)
AS
BEGIN
	IF EXISTS (SELECT PrId FROM Pr WHERE RecId = @RecId)
	BEGIN			
		DECLARE @WfTransId int, @RefId nvarchar(20), @Index int, @UserId nvarchar(20), @WfFormId int
		SET @WfFormId = 1 --FormPr
	
		--Cari nomor dok yang mau diapprove
		SELECT @RefId = PrId FROM Pr 
		WHERE RecId = @RecId

		--Cari user yang harus mengaprove
		IF EXISTS (SELECT 1 FROM [Wf] WHERE [WfFormId] = @WfFormId AND [Index] = 1)
		BEGIN
			SELECT @UserId = [UserId]
			FROM [Wf]
			WHERE [WfFormId] = @WfFormId 
				AND [Index] = 1
	
			INSERT INTO WfTrans ([WfId],[WfFormId],[RefId],[Index], [UserId], [WfStatusActionId], [ActionDate], [CreatedDate], [Notes])
			SELECT WfId, @WfFormId, @RefId, [Index], [UserId], 0, '1900-01-01', GETDATE(), ''
				FROM [Wf]
				WHERE [WfFormId] = @WfFormId
					AND [Index] = 1
						
			SET @WfTransId = SCOPE_IDENTITY() 
					
			UPDATE Pr
			SET PrStatusId = 4
				,[WfTransId] = @WfTransId
			WHERE RecId = @RecId
		END
		ELSE
		BEGIN -- tidak ada workflow
			UPDATE Pr
			SET PrStatusId = 1
				,[WfTransId] = 0
			WHERE RecId = @RecId
		END
	END
END

GO

