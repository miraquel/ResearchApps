CREATE PROCEDURE [dbo].[SalesPrice_Insert]
@ItemId int,
@CustomerId int,
@StartDate datetime,
@EndDate datetime,
@SalesPrice numeric(32,16),
@Notes nvarchar(100),
@StatusId int = 1,
@CreatedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @RecId int
	
	INSERT INTO [SalesPrice]
	([ItemId],[CustomerId],[StartDate],[EndDate] ,[SalesPrice],[Notes],[StatusId]
		,[CreatedDate],[CreatedBy],[ModifiedDate],[ModifiedBy])
		VALUES
		(@ItemId,@CustomerId,@StartDate,@EndDate,@SalesPrice,@Notes
			,@StatusId, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy)

	SET @RecId = SCOPE_IDENTITY()

	SELECT a.[ItemId]
      ,a.[CustomerId]
      ,a.[StartDate]
      ,a.[EndDate]
      ,a.[SalesPrice]
      ,a.[Notes]
      ,a.[StatusId]
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [SalesPrice] a
  JOIN [Item] b ON b.ItemId = a.ItemId
  JOIN [Customer] c ON c.CustomerId = a.CustomerId
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.RecId = @RecId
END
GO

