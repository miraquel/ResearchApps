CREATE PROCEDURE [dbo].[Co_RejectById]
	@RecId int,
	@Notes nvarchar(50)='',
	@ModifiedBy nvarchar(20)
AS
DECLARE @RefId nvarchar(20), @Index int
DECLARE @NextIndex int, @NextUserId nvarchar(20)
DECLARE @WfTransId int, @NextWfTransId int
BEGIN
	IF EXISTS (SELECT a.CoId FROM Co a JOIN CoLine b ON b.CoId = a.CoId WHERE a.RecId = @RecId)
	BEGIN	
		SELECT @RefId = a.CoId, @WfTransId = a.WfTransId, @Index = w.[Index]
		FROM Co a 
		JOIN [WfTrans] w ON w.WfTransId = a.WfTransId
		WHERE a.RecId = @RecId

		UPDATE WfTrans
		SET WfStatusActionId = 2, ActionDate = GETDATE()
			, Notes = @Notes
		WHERE WfTransId = @WfTransId

		UPDATE Co
		SET CoStatusId = 5
		WHERE RecId = @RecId
	END
END

GO

