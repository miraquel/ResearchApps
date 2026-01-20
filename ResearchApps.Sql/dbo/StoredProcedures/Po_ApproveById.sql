CREATE PROCEDURE [dbo].[Po_ApproveById]
	@RecId int,
	@Notes nvarchar(50)='',
	@ModifiedBy nvarchar(20)
AS
DECLARE @RefId nvarchar(20), @Index int
DECLARE @NextIndex int, @NextUserId nvarchar(20)
DECLARE @WfTransId int, @NextWfTransId int, @WfFormId int
SET @WfFormId = 3 --FormPo
BEGIN
	IF EXISTS (SELECT PoId FROM Po WHERE RecId = @RecId)
	BEGIN	
		--Cari next approval
		SELECT @RefId = a.PoId
			,@WfTransId = a.WfTransId
			,@Index = w.[Index]
		FROM Po a 
		JOIN [WfTrans] w ON w.WfTransId = a.WfTransId
		WHERE a.RecId = @RecId

		IF EXISTS (SELECT [Index] FROM Wf WHERE WfFormId = @WfFormId AND [Index] = @Index+1)
		--Jika masih ada  next aproval
		BEGIN
			SELECT @NextIndex = [Index], @NextUserId = [UserId] FROM Wf WHERE WfFormId = @WfFormId AND [Index] = @Index+1 --FormPo
						
			INSERT INTO WfTrans (WfId, [WfFormId], [RefId], [Index], [UserId], [WfStatusActionId], [ActionDate], [CreatedDate], [Notes])
			SELECT [WfId], [WfFormId], @RefId, [Index], [UserId], 0, '1900-01-01', GETDATE(), ''
				FROM [Wf]
				WHERE [WfFormId] = @WfFormId
					AND [Index] = @NextIndex

			SET @NextWfTransId = SCOPE_IDENTITY() 

			UPDATE WfTrans
				SET WfStatusActionId = 1, ActionDate = GETDATE()
					,Notes = @Notes
				WHERE WfTransId = @WfTransId
					
			UPDATE Po
				SET PoStatusId = 4
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

			UPDATE Po
			SET PoStatusId = 1
			WHERE RecId = @RecId
		END
	END
END

GO

