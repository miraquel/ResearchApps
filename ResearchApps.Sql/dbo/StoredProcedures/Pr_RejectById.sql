--EXEC [Pr_RejectById] 5,'budget di-hold','thomas'
CREATE PROCEDURE [dbo].[Pr_RejectById]
	@RecId int,
	@Notes nvarchar(50)='',
	@ModifiedBy nvarchar(20)
AS
DECLARE @RefId nvarchar(20), @Index int
DECLARE @NextIndex int, @NextUserId nvarchar(20)
DECLARE @WfTransId int, @NextWfTransId int
BEGIN
	IF EXISTS (SELECT PrId FROM Pr WHERE RecId = @RecId)
	BEGIN	
		SELECT @RefId = a.PrId
			,@WfTransId = a.WfTransId
			,@Index = w.[Index]
		FROM Pr a 
		JOIN [WfTrans] w ON w.WfTransId = a.WfTransId
		WHERE a.RecId = @RecId

		UPDATE WfTrans
		SET WfStatusActionId = 2
			,ActionDate = GETDATE()
			,Notes = @Notes
		WHERE WfTransId = @WfTransId

		UPDATE Pr
		SET PrStatusId = 5
		WHERE RecId = @RecId
	END
END

GO

