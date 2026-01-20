CREATE PROCEDURE [dbo].[Co_ApproveById]
	@RecId int,
	@Notes nvarchar(50)='',
	@ModifiedBy nvarchar(20)
AS
DECLARE @RefId nvarchar(20), @Index int
DECLARE @NextIndex int, @NextUserId nvarchar(20)
DECLARE @WfTransId int, @NextWfTransId int, @WfFormId int
BEGIN
	SET @WfFormId = 2 --Form Co

	IF EXISTS (SELECT a.CoId FROM Co a JOIN CoLine b ON b.CoId = a.CoId WHERE a.RecId = @RecId)
	BEGIN	
		--Cari next approval
		SELECT @RefId = a.CoId, @WfTransId = a.WfTransId, @Index = w.[Index]
		FROM Co a 
		JOIN [WfTrans] w ON w.WfTransId = a.WfTransId
		WHERE a.RecId = @RecId

		IF EXISTS (SELECT [Index] FROM Wf WHERE WfFormId = @WfFormId AND [Index] = @Index+1)
		--Jika masih ada  next aproval
		BEGIN
			SELECT @NextIndex = [Index], @NextUserId = [UserId] FROM Wf WHERE WfFormId = @WfFormId AND [Index] = @Index+1 
			
			SELECT @NextWfTransId = max(WfTransId) + 1
			FROM WfTrans

			IF @NextWfTransId is null
				SET @NextWfTransId = 1
	
			INSERT INTO WfTrans (WfTransId, [WfFormId], [RefId], [Index], [UserId], [WfStatusActionId], [ActionDate], [CreatedDate], [Notes])
			SELECT @NextWfTransId, [WfFormId], @RefId, [Index], [UserId], 0, '1900-01-01', GETDATE(), ''
				FROM [Wf]
				WHERE [WfFormId] = @WfFormId
					AND [Index] = @NextIndex

			UPDATE WfTrans
			SET WfStatusActionId = 1, ActionDate = GETDATE()
			WHERE WfTransId = @WfTransId
					
			UPDATE Co
			SET CoStatusId = 4
				,[WfTransId] = @NextWfTransId
			WHERE RecId = @RecId
		END
		ELSE
		BEGIN
		--Jika tidak ada lagi next aproval
			UPDATE WfTrans
			SET WfStatusActionId = 1, ActionDate = GETDATE()
				, Notes = @Notes
			WHERE WfTransId = @WfTransId

			UPDATE Co
			SET CoStatusId = 1
			WHERE RecId = @RecId
		END
	END
END

GO

