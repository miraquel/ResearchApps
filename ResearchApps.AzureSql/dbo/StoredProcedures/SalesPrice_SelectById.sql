CREATE PROCEDURE [dbo].[SalesPrice_SelectById]
@RecId int = 1
AS
BEGIN
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

