CREATE PROCEDURE [dbo].[Po_RejectById]
	@RecId int,
	@Notes nvarchar(50)='',
	@ModifiedBy nvarchar(20)
AS
DECLARE @RefId nvarchar(20), @Index int
DECLARE @NextIndex int, @NextUserId nvarchar(20)
DECLARE @WfTransId int, @NextWfTransId int
BEGIN
	IF EXISTS (SELECT PoId FROM Po WHERE RecId = @RecId)
	BEGIN	
		SELECT @RefId = a.PoId
			,@WfTransId = a.WfTransId
			,@Index = w.[Index]
		FROM Po a 
		JOIN [WfTrans] w ON w.WfTransId = a.WfTransId
		WHERE a.RecId = @RecId

		UPDATE WfTrans
		SET WfStatusActionId = 2
			,ActionDate = GETDATE()
			,Notes = @Notes
		WHERE WfTransId = @WfTransId

		UPDATE Po
		SET PoStatusId = 5
		WHERE RecId = @RecId
	END
END

GO

