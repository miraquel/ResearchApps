CREATE PROCEDURE [dbo].[SalesPrice_Update]
@RecId int,
@ItemId int,
@CustomerId int,
@StartDate datetime,
@EndDate datetime,
@SalesPrice numeric(32,16),
@Notes nvarchar(100),
@StatusId int,
@ModifiedBy  nvarchar(20)
AS
BEGIN
	UPDATE [SalesPrice]
	SET ItemId = @ItemId
		, CustomerId = @CustomerId
		, StartDate = @StartDate
		, EndDate = @EndDate
		, SalesPrice = @SalesPrice
		, Notes = @Notes
		, StatusId = @StatusId
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE [RecId] = @RecId

	SELECT a.[RecId]
		,a.[ItemId]
		,i.[ItemName]
		,a.[CustomerId]
		,c.[CustomerName]
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
	JOIN [Item] i ON i.ItemId = a.ItemId
	JOIN [Customer] c ON c.CustomerId = a.CustomerId
	JOIN [Status] s ON s.StatusId = a.StatusId
	WHERE a.RecId = @RecId
END
GO

