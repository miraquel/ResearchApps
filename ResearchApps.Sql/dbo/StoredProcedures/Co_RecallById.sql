CREATE PROCEDURE [dbo].[Co_RecallById]
	@RecId int,
	@ModifiedBy nvarchar(20)
AS
DECLARE @RefId nvarchar(20)
DECLARE @CoStatusId int

BEGIN
	IF EXISTS (SELECT a.CoId FROM Co a JOIN CoLine b ON b.CoId = a.CoId WHERE a.RecId = @RecId)
	BEGIN
		IF EXISTS (SELECT CoStatusId FROM Co WHERE RecId = @RecId AND CoStatusId = 1)
		--Sudah pernah approved sebelumnya
		BEGIN
			UPDATE Co
			SET CoStatusId = 0, Revision = Revision + 1 
			WHERE RecId = @RecId
		END	
		ELSE
		BEGIN
			UPDATE Co
			SET CoStatusId = 0
			WHERE RecId = @RecId
		END		
	END
END

GO

