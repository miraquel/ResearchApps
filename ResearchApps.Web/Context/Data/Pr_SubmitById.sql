--EXEC [Pr_SubmitById] 1, 'admin'
CREATE PROCEDURE [dbo].[Pr_SubmitById]
	@RecId int,
	@ModifiedBy nvarchar(20)
AS
BEGIN
	IF EXISTS (SELECT PrId FROM Pr WHERE RecId = @RecId)
	BEGIN	
		DECLARE @WfTransId int
		DECLARE @RefId nvarchar(20)
		DECLARE @Index int
	
		--Cari nomor dok yang mau diapprove
		SELECT @RefId = PrId FROM Pr 
		WHERE RecId = @RecId
	
		INSERT INTO WfTrans ([WfId],[WfFormId],[RefId],[Index], [UserId], [WfStatusActionId], [ActionDate], [CreatedDate], [Notes])
		SELECT WfId, [WfFormId], @RefId, [Index], [UserId], 0, '1900-01-01', GETDATE(), ''
			FROM [Wf]
			WHERE [WfFormId] = 1 --FormCo
				AND [Index] = 1
						
		SET @WfTransId = SCOPE_IDENTITY() 
					
		UPDATE Pr
		SET PrStatusId = 4
			,[WfTransId] = @WfTransId
		WHERE RecId = @RecId

		SELECT * FROM Pr WHERE RecId = @RecId
	END
END

go

